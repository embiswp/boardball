using System.Drawing;
using Shouldly;

namespace BoardBall.Core.Tests;

public class GameTests
{
    [Fact]
    public void Game_Player1_Should_Not_Place_Footballer_On_Ball()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        game.Ball.X.ShouldBe(3);
        game.Ball.Y.ShouldBe(3);
    }

    [Fact]
    public void Player1_Should_Not_Place_Footballer_On_Ball()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        Should.Throw<GameException>(() => game.Player1AddsFootballer(new Point(3,3)));
    }

    [Fact]
    public void Player1_Can_Place_First_Footballer_On_Field()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        game.Player1AddsFootballer(new Point(2,3));
        game.Footballers.Count().ShouldBe(1);
        game.Footballers[0].X.ShouldBe(2);
        game.Footballers[0].Y.ShouldBe(3);
    }

    [Fact]
    public void After_Player1_Places_Footballer_Player1_Cannot_Continue()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        game.Player1AddsFootballer(new Point(2,3));
        Should.Throw<GameException>(() => game.Player1AddsFootballer(new Point(4,3)));
    }

    [Fact]
    public void Player2_Cannot_Place_Footballer_On_Existing_Footballer()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        game.Player1AddsFootballer(new Point(2,3));
        Should.Throw<GameException>(() => game.Player2AddsFootballer(new Point(2,3)));
    }

    [Fact]
    public void Player2_Cannot_Place_Footballer_On_Ball()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        game.Player1AddsFootballer(new Point(2,3));
        Should.Throw<GameException>(() => game.Player2AddsFootballer(new Point(3,3)));
    }

    [Fact]
    public void Player2_Can_Place_Footballer_On_Field()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        game.Player1AddsFootballer(new Point(2,3));
        game.Player2AddsFootballer(new Point(4,3));
        game.Footballers.Count().ShouldBe(2);
        game.Footballers[0].X.ShouldBe(2);
        game.Footballers[0].Y.ShouldBe(3);
        game.Footballers[1].X.ShouldBe(4);
        game.Footballers[1].Y.ShouldBe(3);
    }

    [Fact]
    public void After_Player2_Places_Footballer_Player2_Cannot_Continue()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 4));
        game.Player1AddsFootballer(new Point(2,3));
        game.Player2AddsFootballer(new Point(4,3));
        Should.Throw<GameException>(() => game.Player2AddsFootballer(new Point(4,2)));
    }

    [Fact]
    public void Players_Cannot_Place_More_Footballers_On_Field_Than_The_Maximum()
    {
        var game = new Game(new GameConfig("p1", "p2", 5, 5, 2));
        game.Player1AddsFootballer(new Point(2,3));
        game.Player2AddsFootballer(new Point(4,3));
        Should.Throw<GameException>(() => game.Player1AddsFootballer(new Point(4,2)));
    }

    
}
