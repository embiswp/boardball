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
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        game.Handle(new Commands.PlaceFootballer(Player.One, new Point(2, 2)));
        var command = new Commands.PlaceFootballer(Player.One, new Point(2, 2));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.WrongPlayer);
    }

    [Fact]
    public void Player2_Cannot_Place_Footballer_On_Placed_Footballer() {
        var game = new Game();
        game.Handle(new Commands.StartGame(new GameConfig("p1", "p2", 5, 5, 4)));
        game.Handle(new Commands.PlaceFootballer(Player.One, new Point(2, 2)));
        var command = new Commands.PlaceFootballer(Player.Two, new Point(2, 2));
        Should.Throw<ArgumentException>(() => game.Handle(command)).Message.ShouldBe(Game.CannotPlaceOnFootballer);
    }


    // [Fact]
    // public void Player1_Should_Not_Place_Footballer_On_Ball()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
    //     Should.Throw<GameException>(() => game.Player1AddsFootballer(new Point(3,3)));
    // }

    // [Fact]
    // public void Player1_Can_Place_First_Footballer_On_Field()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
    //     game.Player1AddsFootballer(new Point(2,3));
    //     game.Footballers.Count().ShouldBe(1);
    //     game.Footballers[0].X.ShouldBe(2);
    //     game.Footballers[0].Y.ShouldBe(3);
    // }

    // [Fact]
    // public void After_Player1_Places_Footballer_Player1_Cannot_Continue()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
    //     game.Player1AddsFootballer(new Point(2,3));
    //     Should.Throw<GameException>(() => game.Player1AddsFootballer(new Point(4,3)));
    // }

    // [Fact]
    // public void Player2_Cannot_Place_Footballer_On_Existing_Footballer()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
    //     game.Player1AddsFootballer(new Point(2,3));
    //     Should.Throw<GameException>(() => game.Player2AddsFootballer(new Point(2,3)));
    // }

    // [Fact]
    // public void Player2_Cannot_Place_Footballer_On_Ball()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
    //     game.Player1AddsFootballer(new Point(2,3));
    //     Should.Throw<GameException>(() => game.Player2AddsFootballer(new Point(3,3)));
    // }

    // [Fact]
    // public void Player2_Can_Place_Footballer_On_Field()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
    //     game.Player1AddsFootballer(new Point(2,3));
    //     game.Player2AddsFootballer(new Point(4,3));
    //     game.Footballers.Count().ShouldBe(2);
    //     game.Footballers[0].X.ShouldBe(2);
    //     game.Footballers[0].Y.ShouldBe(3);
    //     game.Footballers[1].X.ShouldBe(4);
    //     game.Footballers[1].Y.ShouldBe(3);
    // }

    // [Fact]
    // public void After_Player2_Places_Footballer_Player2_Cannot_Continue()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
    //     game.Player1AddsFootballer(new Point(2,3));
    //     game.Player2AddsFootballer(new Point(4,3));
    //     Should.Throw<GameException>(() => game.Player2AddsFootballer(new Point(4,2)));
    // }

    // [Fact]
    // public void Players_Cannot_Place_More_Footballers_On_Field_Than_The_Maximum()
    // {
    //     var game = new Game(new GameConfig("p1", "p2", 5, 5, 2));
    //     game.Player1AddsFootballer(new Point(2,3));
    //     game.Player2AddsFootballer(new Point(4,3));
    //     Should.Throw<GameException>(() => game.Player1AddsFootballer(new Point(4,2)));
    // }

    
}
