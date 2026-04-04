using System.Drawing;
using BoardBall.Core;
using Shouldly;

namespace BoardBall.Core.Tests;

public class GameStartTests
{
    private static GameConfig DefaultConfig(int rows = 15, int columns = 19, int stack = 30)
        => new("Alice", "Bob", rows, columns, stack);

    [Fact]
    public void Test_StartGame_Sets_Status_To_InProgress()
    {
        //Given
        var game = new Game();
        //When
        game.Handle(new Commands.StartGame(DefaultConfig()));
        //Then
        game.State.Status.ShouldBe(GameStatus.InProgress);
    }

    [Fact]
    public void Test_StartGame_Places_Ball_At_Center_Of_15x19_Board()
    {
        //Given
        var game = new Game();
        //When
        game.Handle(new Commands.StartGame(DefaultConfig(15, 19)));
        //Then
        // Center of 15x19: col=10, row=8 → Point(X=10, Y=8)
        game.State.Ball.ShouldBe(new Point(10, 8));
    }

    [Fact]
    public void Test_StartGame_Places_Ball_At_Center_Of_1x5_Board()
    {
        //Given
        var game = new Game();
        //When
        game.Handle(new Commands.StartGame(DefaultConfig(1, 5)));
        //Then
        // Center of 1x5: col=3, row=1 → Point(X=3, Y=1)
        game.State.Ball.ShouldBe(new Point(3, 1));
    }

    [Fact]
    public void Test_StartGame_Sets_Current_Player_To_Player1()
    {
        //Given
        var game = new Game();
        //When
        game.Handle(new Commands.StartGame(DefaultConfig()));
        //Then
        game.State.CurrentPlayer.ShouldBe(Player.One);
    }

    [Fact]
    public void Test_StartGame_Sets_Stack_Count()
    {
        //Given
        var game = new Game();
        //When
        game.Handle(new Commands.StartGame(DefaultConfig(stack: 30)));
        //Then
        game.State.StackCount.ShouldBe(30);
    }

    [Fact]
    public void Test_StartGame_Board_Has_No_Stones()
    {
        //Given
        var game = new Game();
        //When
        game.Handle(new Commands.StartGame(DefaultConfig()));
        //Then
        game.State.Stones.ShouldBeEmpty();
    }

    [Fact]
    public void Test_StartGame_Cannot_Start_Twice()
    {
        //Given
        var game = new Game();
        game.Handle(new Commands.StartGame(DefaultConfig()));
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.StartGame(DefaultConfig())))
              .Message.ShouldBe(Game.GameAlreadyStarted);
    }

    [Fact]
    public void Test_StartGame_Emits_GameStarted_BallPlaced_CurrentPlayerChanged()
    {
        //Given
        var game = new Game();
        //When
        var events = game.Handle(new Commands.StartGame(DefaultConfig())).ToList();
        //Then
        events.OfType<Events.GameStarted>().ShouldHaveSingleItem();
        events.OfType<Events.BallPlaced>().ShouldHaveSingleItem();
        events.OfType<Events.CurrentPlayerChanged>().ShouldHaveSingleItem();
    }
}
