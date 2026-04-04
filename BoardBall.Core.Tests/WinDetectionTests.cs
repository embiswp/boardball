using System.Drawing;
using BoardBall.Core;
using Shouldly;

namespace BoardBall.Core.Tests;

public class WinDetectionTests
{
    // 3x7 board. Ball starts at (4,2).
    private static Game StartedGame()
    {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("Alice", "Bob", 3, 7, 30)));
        return game;
    }

    private static void Place(Game game, Player player, Point loc)
        => game.Handle(new Commands.PlaceStone(player, loc));

    [Fact]
    public void Test_Win_Player1_Ball_Reaches_Last_Column()
    {
        //Given
        var game = StartedGame(); // ball at (4,2), last col = 7
        Place(game, Player.One, new Point(5, 2));
        Place(game, Player.Two, new Point(6, 2));
        // P1's turn: jump right over (5,2),(6,2) → land at (7,2) = last col
        //When
        game.Handle(new Commands.JumpBall(Player.One, Direction.Right));
        //Then
        game.State.Status.ShouldBe(GameStatus.Won);
        game.State.Winner.ShouldBe(Player.One);
    }

    [Fact]
    public void Test_Win_Player1_Ball_Exits_Right_Off_Board()
    {
        //Given
        var game = StartedGame(); // ball at (4,2)
        Place(game, Player.One, new Point(5, 2));
        Place(game, Player.Two, new Point(6, 2));
        Place(game, Player.One, new Point(7, 2)); // stone ON the last col
        Place(game, Player.Two, new Point(1, 1));
        // P1's turn: jump right over (5,2),(6,2),(7,2) → land off-board (right of col 7)
        //When
        game.Handle(new Commands.JumpBall(Player.One, Direction.Right));
        //Then
        game.State.Status.ShouldBe(GameStatus.Won);
        game.State.Winner.ShouldBe(Player.One);
    }

    [Fact]
    public void Test_Win_Player2_Ball_Reaches_First_Column()
    {
        //Given
        var game = StartedGame(); // ball at (4,2), first col = 1
        Place(game, Player.One, new Point(1, 1));  // filler
        Place(game, Player.Two, new Point(3, 2));  // stone left of ball
        Place(game, Player.One, new Point(2, 1));  // filler
        // Now P2's turn: jump left over (3,2) → land at (2,2)? Not goal.
        // We need stone at (3,2) and (2,2) for P2 to reach col 1.
        var game2 = StartedGame(); // fresh game, ball at (4,2)
        Place(game2, Player.One, new Point(3, 2));
        Place(game2, Player.Two, new Point(2, 2));
        // P1's turn now: need to get to P2's turn
        Place(game2, Player.One, new Point(1, 1)); // filler
        // P2's turn: jump left over (3,2),(2,2) → land at (1,2) = first col = P2 wins
        //When
        game2.Handle(new Commands.JumpBall(Player.Two, Direction.Left));
        //Then
        game2.State.Status.ShouldBe(GameStatus.Won);
        game2.State.Winner.ShouldBe(Player.Two);
    }

    [Fact]
    public void Test_Win_Player2_Ball_Exits_Left_Off_Board()
    {
        //Given — P2 jumps so ball exits left of col 1
        var game = StartedGame(); // ball at (4,2)
        Place(game, Player.One, new Point(3, 2));
        Place(game, Player.Two, new Point(2, 2));
        Place(game, Player.One, new Point(1, 2)); // stone ON first col
        // Now P2's turn: jump left over (3,2),(2,2),(1,2) → off-board left
        //When
        game.Handle(new Commands.JumpBall(Player.Two, Direction.Left));
        //Then
        game.State.Status.ShouldBe(GameStatus.Won);
        game.State.Winner.ShouldBe(Player.Two);
    }

    [Fact]
    public void Test_Win_During_Chain_Stops_Chain()
    {
        //Given — 3x7, ball at (4,2)
        var game = StartedGame();
        Place(game, Player.One, new Point(5, 2));
        Place(game, Player.Two, new Point(6, 2));
        // After P1 jumps right over (5,2),(6,2) → lands at (7,2) = goal → win immediately
        //When
        var events = game.Handle(new Commands.JumpBall(Player.One, Direction.Right)).ToList();
        //Then
        game.State.Status.ShouldBe(GameStatus.Won);
        events.OfType<Events.ChainContinuationRequired>().ShouldBeEmpty();
    }

    [Fact]
    public void Test_Cannot_Act_After_Game_Won()
    {
        //Given
        var game = StartedGame(); // ball at (4,2)
        Place(game, Player.One, new Point(5, 2));
        Place(game, Player.Two, new Point(6, 2));
        game.Handle(new Commands.JumpBall(Player.One, Direction.Right)); // P1 wins
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.PlaceStone(Player.Two, new Point(1, 1))))
              .Message.ShouldBe(Game.GameOver);
    }
}
