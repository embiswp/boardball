using System.Drawing;
using BoardBall.Core;
using Shouldly;

namespace BoardBall.Core.Tests;

public class PlaceStoneTests
{
    private static Game StartedGame(int rows = 5, int columns = 7, int stack = 20)
    {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("Alice", "Bob", rows, columns, stack)));
        return game;
    }

    [Fact]
    public void Test_PlaceStone_Player1_Can_Place_On_Empty_Cell()
    {
        //Given
        var game = StartedGame();
        //When
        game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1)));
        //Then
        game.State.Stones.ShouldContain(new Point(1, 1));
    }

    [Fact]
    public void Test_PlaceStone_Decrements_Stack()
    {
        //Given
        var game = StartedGame(stack: 10);
        //When
        game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1)));
        //Then
        game.State.StackCount.ShouldBe(9);
    }

    [Fact]
    public void Test_PlaceStone_Alternates_Player()
    {
        //Given
        var game = StartedGame();
        //When
        game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1)));
        //Then
        game.State.CurrentPlayer.ShouldBe(Player.Two);
    }

    [Fact]
    public void Test_PlaceStone_Player2_Cannot_Go_Before_Player1()
    {
        //Given
        var game = StartedGame();
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.Two, new Point(1, 1))))
              .Message.ShouldBe(Game.WrongPlayer);
    }

    [Fact]
    public void Test_PlaceStone_Wrong_Player_After_Turn_Change()
    {
        //Given
        var game = StartedGame();
        game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1)));
        //When/Then — now it's Player2's turn, Player1 cannot go
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(2, 1))))
              .Message.ShouldBe(Game.WrongPlayer);
    }

    [Fact]
    public void Test_PlaceStone_Cannot_Place_On_Ball()
    {
        //Given
        var game = StartedGame(rows: 5, columns: 7); // ball at (4, 3)
        var ball = game.State.Ball;
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, ball)))
              .Message.ShouldBe(Game.LocationOccupied);
    }

    [Fact]
    public void Test_PlaceStone_Cannot_Place_On_Existing_Stone()
    {
        //Given
        var game = StartedGame();
        game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1)));
        game.Handle(new Commands.PlaceStone(Player.Two, new Point(2, 1)));
        //When/Then — Player1 tries to place on (1,1) again
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1))))
              .Message.ShouldBe(Game.LocationOccupied);
    }

    [Fact]
    public void Test_PlaceStone_Cannot_Place_Out_Of_Bounds_Left()
    {
        //Given
        var game = StartedGame();
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(0, 1))))
              .Message.ShouldBe(Game.OutOfBounds);
    }

    [Fact]
    public void Test_PlaceStone_Cannot_Place_Out_Of_Bounds_Right()
    {
        //Given
        var game = StartedGame(columns: 7);
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(8, 1))))
              .Message.ShouldBe(Game.OutOfBounds);
    }

    [Fact]
    public void Test_PlaceStone_Cannot_Place_Out_Of_Bounds_Top()
    {
        //Given
        var game = StartedGame();
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 0))))
              .Message.ShouldBe(Game.OutOfBounds);
    }

    [Fact]
    public void Test_PlaceStone_Cannot_Place_Out_Of_Bounds_Bottom()
    {
        //Given
        var game = StartedGame(rows: 5);
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 6))))
              .Message.ShouldBe(Game.OutOfBounds);
    }

    [Fact]
    public void Test_PlaceStone_Cannot_Place_When_Stack_Empty()
    {
        //Given
        var game = StartedGame(stack: 2);
        game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1)));
        game.Handle(new Commands.PlaceStone(Player.Two, new Point(2, 1)));
        // Stack is now empty
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(3, 1))))
              .Message.ShouldBe(Game.StackEmpty);
    }

    [Fact]
    public void Test_PlaceStone_Can_Place_On_Goal_Line_First_Column()
    {
        //Given
        var game = StartedGame();
        //When — column 1 is Player2's goal line
        game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 2)));
        //Then
        game.State.Stones.ShouldContain(new Point(1, 2));
    }

    [Fact]
    public void Test_PlaceStone_Can_Place_On_Goal_Line_Last_Column()
    {
        //Given
        var game = StartedGame(columns: 7);
        //When — column 7 is Player1's goal line
        game.Handle(new Commands.PlaceStone(Player.One, new Point(7, 2)));
        //Then
        game.State.Stones.ShouldContain(new Point(7, 2));
    }

    [Fact]
    public void Test_PlaceStone_Requires_Game_Started()
    {
        //Given
        var game = new Game();
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.One, new Point(1, 1))))
              .Message.ShouldBe(Game.GameNotStarted);
    }
}
