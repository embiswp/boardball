using System;
using Shouldly;

namespace BoardBall.Core.Tests;

public class GameConfigTests
{
    [Fact]
    public void GameConfig_Should_Not_Accept_Empty_Player1()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("", "Player2", 3, 4, 4)).Message.ShouldBe(GameConfig.Player1NameEmpty);
    }

    [Fact]
    public void GameConfig_Should_Not_Accept_Empty_Player2()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Player1", "", 3, 4, 4)).Message.ShouldBe(GameConfig.Player2NameEmpty);
    }

    [Fact]
    public void GameConfig_Should_Not_Accept_Even_Column_Number()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Player1", "Player", 10, 4, 4)).Message.ShouldBe(GameConfig.ColumnNotOdd);
    }

    [Fact]
    public void GameConfig_Should_Not_Accept_Even_Row_Number()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Player1", "Player", 5, 4, 4)).Message.ShouldBe(GameConfig.RowNotOdd);
    }

    [Fact]
    public void GameConfig_Should_Accept_Column_Higher_Than_2()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Player1", "Player", 1, 5, 4)).Message.ShouldBe(GameConfig.ColumnsCountAtLeastThree);
    }

    [Fact]
    public void GameConfig_Should_Not_Accept_Odd_Footballers()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Player1", "Player", 5, 3, 5)).Message.ShouldBe(GameConfig.EvenFootballers);
    }
}
