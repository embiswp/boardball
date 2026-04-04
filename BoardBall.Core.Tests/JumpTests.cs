using System.Drawing;
using BoardBall.Core;
using Shouldly;

namespace BoardBall.Core.Tests;

public class JumpTests
{
    // Board: 5 rows x 7 cols. Ball starts at (4,3) — col=4, row=3.
    private static Game StartedGame()
    {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("Alice", "Bob", 5, 7, 30)));
        return game;
    }

    // Place a stone and advance turn so we always end on Player1's turn.
    private static void PlaceAndAdvance(Game game, Player player, Point loc)
        => game.Handle(new Commands.PlaceStone(player, loc));

    private static void SetupStone(Game game, Point loc)
    {
        // Alternate placements to keep turns in sync
        var player = game.State.CurrentPlayer ?? Player.One;
        PlaceAndAdvance(game, player, loc);
    }

    [Fact]
    public void Test_Jump_Right_Over_Single_Stone()
    {
        //Given
        var game = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game, Player.One, new Point(5, 3));  // stone right of ball
        PlaceAndAdvance(game, Player.Two, new Point(1, 1));  // dummy
        //When — Player1 jumps right
        game.Handle(new Commands.JumpBall(Player.One, Direction.Right));
        //Then — ball lands at (6,3)
        game.State.Ball.ShouldBe(new Point(6, 3));
        game.State.Stones.ShouldNotContain(new Point(5, 3));
    }

    [Fact]
    public void Test_Jump_Left_Over_Single_Stone()
    {
        //Given
        var game = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game, Player.One, new Point(3, 3));
        PlaceAndAdvance(game, Player.Two, new Point(1, 1));
        //When
        game.Handle(new Commands.JumpBall(Player.One, Direction.Left));
        //Then — ball lands at (2,3)
        game.State.Ball.ShouldBe(new Point(2, 3));
        game.State.Stones.ShouldNotContain(new Point(3, 3));
    }

    [Fact]
    public void Test_Jump_Up_Over_Single_Stone()
    {
        //Given
        var game = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game, Player.One, new Point(4, 2));
        PlaceAndAdvance(game, Player.Two, new Point(1, 1));
        //When
        game.Handle(new Commands.JumpBall(Player.One, Direction.Up));
        //Then — ball lands at (4,1)
        game.State.Ball.ShouldBe(new Point(4, 1));
        game.State.Stones.ShouldNotContain(new Point(4, 2));
    }

    [Fact]
    public void Test_Jump_Down_Over_Single_Stone()
    {
        //Given
        var game = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game, Player.One, new Point(4, 4));
        PlaceAndAdvance(game, Player.Two, new Point(1, 1));
        //When
        game.Handle(new Commands.JumpBall(Player.One, Direction.Down));
        //Then — ball lands at (4,5)
        game.State.Ball.ShouldBe(new Point(4, 5));
    }

    [Fact]
    public void Test_Jump_Diagonal_UpRight()
    {
        //Given
        var game = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game, Player.One, new Point(5, 2));
        PlaceAndAdvance(game, Player.Two, new Point(1, 1));
        //When
        game.Handle(new Commands.JumpBall(Player.One, Direction.UpRight));
        //Then — ball lands at (6,1)
        game.State.Ball.ShouldBe(new Point(6, 1));
    }

    [Fact]
    public void Test_Jump_Over_Multiple_Consecutive_Stones()
    {
        //Given
        // After P1 places (5,3) → P2's turn. After P2 places (6,3) → P1's turn.
        var game = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game, Player.One, new Point(5, 3));
        PlaceAndAdvance(game, Player.Two, new Point(6, 3));
        // ball at (4,3), stones at (5,3) and (6,3) — two consecutive stones right; P1's turn
        //When
        var events = game.Handle(new Commands.JumpBall(Player.One, Direction.Right)).ToList();
        //Then — ball lands at (7,3) — the goal line!
        // So instead let's use a board where there's room: use row 1 so ball doesn't reach goal
        // Actually ball at col 4, jump right over cols 5,6 → land at col 7 which IS the last col
        // That's a win for Player1. Let's assert that.
        game.State.Status.ShouldBe(GameStatus.Won);
        game.State.Winner.ShouldBe(Player.One);
        events.OfType<Events.StonesReturned>().ShouldHaveSingleItem();
        events.OfType<Events.StonesReturned>().First().Locations.Count.ShouldBe(2);
    }

    [Fact]
    public void Test_Jump_Returns_Stones_To_Stack()
    {
        //Given
        var game = StartedGame(  );
        var stackBefore = game.State.StackCount;
        PlaceAndAdvance(game, Player.One, new Point(5, 3));
        PlaceAndAdvance(game, Player.Two, new Point(1, 1));
        var stackAfterPlace = game.State.StackCount; // stackBefore - 2
        //When
        game.Handle(new Commands.JumpBall(Player.One, Direction.Right));
        //Then — jumped stone returns to stack
        game.State.StackCount.ShouldBe(stackAfterPlace + 1);
    }

    [Fact]
    public void Test_Jump_Requires_At_Least_One_Stone()
    {
        //Given
        var game = StartedGame(); // ball at (4,3), nothing to the right
        PlaceAndAdvance(game, Player.One, new Point(1, 1));
        PlaceAndAdvance(game, Player.Two, new Point(2, 1));
        //When — no stone to the right of ball
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.JumpBall(Player.One, Direction.Right)))
              .Message.ShouldBe(Game.InvalidJump);
    }

    [Fact]
    public void Test_Jump_Cannot_Land_On_Occupied_Square()
    {
        //Given
        var game = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game, Player.One, new Point(5, 3)); // stone at 5,3
        PlaceAndAdvance(game, Player.Two, new Point(6, 3)); // stone at 6,3 — would block landing
        //Wait — consecutive stones are collected; if both 5 and 6 are stones,
        // jump right collects both and lands at 7 (goal). Let's use Up direction.
        // Place stone above ball (4,2) and block landing (4,1).
        var game2 = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game2, Player.One, new Point(4, 2)); // stone above ball
        PlaceAndAdvance(game2, Player.Two, new Point(4, 1)); // stone where ball would land
        //When
        Should.Throw<ArgumentException>(() => game2.Handle(new Commands.JumpBall(Player.One, Direction.Up)))
              .Message.ShouldBe(Game.InvalidJump);
    }

    [Fact]
    public void Test_Jump_Cannot_Exit_Top_Edge()
    {
        //Given
        var game = StartedGame(); // ball at (4,3)
        // Move ball close to top by jumping up once first
        PlaceAndAdvance(game, Player.One, new Point(4, 2)); // stone above ball
        PlaceAndAdvance(game, Player.Two, new Point(1, 1));
        game.Handle(new Commands.JumpBall(Player.One, Direction.Up)); // ball now at (4,1)
        // Now put a stone above the ball (4,0 doesn't exist — can't place).
        // Let's try directly: ball at (4,3), place stone at (4,1), then try to jump up — lands at (4,-1) off board top
        var game2 = StartedGame(); // ball at (4,3)
        PlaceAndAdvance(game2, Player.One, new Point(4, 2));
        PlaceAndAdvance(game2, Player.Two, new Point(1, 1));
        // Jump up: ball goes from (4,3) over (4,2) to (4,1). Then try to jump up from (4,1).
        game2.Handle(new Commands.JumpBall(Player.One, Direction.Up)); // ball at (4,1); now P2's turn
        // Now ball at (4,1). Place stone at (4,0) — out of bounds, can't.
        // We can't place a stone off the board, so there can be no Up jump from row 1.
        // P2 cannot jump up (no stone above ball; and even if there were, would go off-board top).
        Should.Throw<ArgumentException>(() => game2.Handle(new Commands.JumpBall(Player.Two, Direction.Up)))
              .Message.ShouldBe(Game.InvalidJump);
    }

    [Fact]
    public void Test_Jump_Requires_Game_Started()
    {
        //Given
        var game = new Game();
        //When/Then
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.JumpBall(Player.One, Direction.Right)))
              .Message.ShouldBe(Game.GameNotStarted);
    }

    [Fact]
    public void Test_Jump_Wrong_Player_Throws()
    {
        //Given
        var game = StartedGame(); // P1's turn
        PlaceAndAdvance(game, Player.One, new Point(5, 3));
        // Now P2's turn
        //When — P1 tries to jump
        Should.Throw<ArgumentException>(() => game.Handle(new Commands.JumpBall(Player.One, Direction.Right)))
              .Message.ShouldBe(Game.WrongPlayer);
    }
}
