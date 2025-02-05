using System.Drawing;
using Shouldly;

namespace BoardBall.Core.Tests;

public class GameTests
{
    [Fact]
    public void Game_GameStart_Sets_Ball_And_CurrentPlayer()
    {
        var game = new Game();
        var command = new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4));
        var events = game.Handle(command);
        events.ShouldContain(e => e is Events.BallPlaced);
        var ballPlaced = events.OfType<Events.BallPlaced>().First();
        ballPlaced.Point.ShouldBe(new Point(3, 3));
        events.ShouldContain(e => e is Events.CurrentPlayerChanged);
        var currentPlayerChanged = events.OfType<Events.CurrentPlayerChanged>().First();
        currentPlayerChanged.Player.ShouldBe(Player.One);
    }

    [Fact]
    public void Player1_Cannot_Come_Before_The_Game_Starts() {
        var game = new Game();
        var command = new Commands.PlaceFootballer(Player.Two, new Point(2, 3));
        Should.Throw<ArgumentException>(() => game.Handle(command));
    }

    [Fact]
    public void Player2_Cannot_Come_When_Player1_Comes() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.Two, new Point(2, 3));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.WrongPlayer);
    }

    [Fact]
    public void Player1_Cannot_Place_Footballer_Where_The_Ball_Is() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(3, 3));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOnBall);
    }

    [Fact]
    public void Player_Cannot_Place_Footballer_Outside_Left_Boundary() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(0, 3));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOutOfBounds);
    }

    [Fact]
    public void Player_Cannot_Place_Footballer_Outside_Right_Boundary() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(6, 3));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOutOfBounds);
    }

    [Fact]
    public void Player_Cannot_Place_Footballer_Outside_Upper_Boundary() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(3, 0));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOutOfBounds);
    }

    [Fact]
    public void Player_Cannot_Place_Footballer_Outside_Lower_Boundary() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(3, 6));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOutOfBounds);
    }


    [Fact]
    public void Player1_Cannot_Place_Footballer_If_There_Are_No_Footballers_In_Stack() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(3, 3));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOnBall);
    }

    //TODO: This test is lying
    [Fact]
    public void Player1_Can_Place_Footballer_On_Empty_Space() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(2, 2));
        var events = game.Handle(command);
        events.ShouldContain(e => e is Events.FootballerPlaced);
        var footballerPlaced = events.OfType<Events.FootballerPlaced>().First();
        footballerPlaced.Point.ShouldBe(new Point(2, 2));
        events.ShouldContain(e => e is Events.CurrentPlayerChanged);
        var currentPlayerChanged = events.OfType<Events.CurrentPlayerChanged>().First();
        currentPlayerChanged.Player.ShouldBe(Player.Two);
    }

    [Fact]
    public void Player1_Cannot_Place_Footballer_After_His_Step() {
        //Given
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        game.Handle(new Commands.PlaceFootballer(Player.One, new Point(2, 2)));
        //When
        var command = new Commands.PlaceFootballer(Player.One, new Point(2, 2));
        //Then
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.WrongPlayer);
    }

    [Fact]
    public void Player2_Cannot_Place_Footballer_On_Placed_Footballer() {
        //Given
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        game.Handle(new Commands.PlaceFootballer(Player.One, new Point(2, 2)));
        //When
        var command = new Commands.PlaceFootballer(Player.Two, new Point(2, 2));
        //Then
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOnFootballer);
    }

    //Ball placing tests
    //TODO: this test is lying!
    [Fact]
    public void Player2_Cannot_Place_Ball_On_Footballer() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        game.Handle(new Commands.PlaceFootballer(Player.One, new Point(2, 2)));
        var command = new Commands.PlaceFootballer(Player.Two, new Point(2, 2));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOnFootballer);
    }
}
