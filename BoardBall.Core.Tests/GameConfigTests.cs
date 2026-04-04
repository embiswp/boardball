using BoardBall.Core;
using Shouldly;

namespace BoardBall.Core.Tests;

public class GameConfigTests
{
    [Fact]
    public void Test_Config_Rejects_Empty_Player1()
    {
        //Given/When/Then
        Should.Throw<ArgumentException>(() => new GameConfig("", "Bob", 3, 5, 10))
              .Message.ShouldBe("player1");
    }

    [Fact]
    public void Test_Config_Rejects_Null_Player1()
    {
        Should.Throw<ArgumentException>(() => new GameConfig(null!, "Bob", 3, 5, 10))
              .Message.ShouldBe("player1");
    }

    [Fact]
    public void Test_Config_Rejects_Empty_Player2()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Alice", "", 3, 5, 10))
              .Message.ShouldBe("player2");
    }

    [Fact]
    public void Test_Config_Rejects_Even_Rows()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Alice", "Bob", 4, 5, 10))
              .Message.ShouldBe("rows");
    }

    [Fact]
    public void Test_Config_Rejects_Zero_Rows()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Alice", "Bob", 0, 5, 10))
              .Message.ShouldBe("rows");
    }

    [Fact]
    public void Test_Config_Rejects_Even_Columns()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Alice", "Bob", 3, 6, 10))
              .Message.ShouldBe("columns");
    }

    [Fact]
    public void Test_Config_Rejects_Columns_Less_Than_5()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Alice", "Bob", 3, 3, 10))
              .Message.ShouldBe("columns");
    }

    [Fact]
    public void Test_Config_Rejects_Zero_StackSize()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Alice", "Bob", 3, 5, 0))
              .Message.ShouldBe("stack");
    }

    [Fact]
    public void Test_Config_Rejects_Negative_StackSize()
    {
        Should.Throw<ArgumentException>(() => new GameConfig("Alice", "Bob", 3, 5, -1))
              .Message.ShouldBe("stack");
    }

    [Fact]
    public void Test_Config_Accepts_Minimum_Valid_Board()
    {
        //Given/When
        var config = new GameConfig("Alice", "Bob", 1, 5, 1);
        //Then
        config.Rows.ShouldBe(1);
        config.Columns.ShouldBe(5);
    }

    [Fact]
    public void Test_Config_CenterColumn_Computed_Correctly()
    {
        //Given
        var config = new GameConfig("Alice", "Bob", 15, 19, 30);
        //Then
        config.CenterColumn.ShouldBe(10);
    }

    [Fact]
    public void Test_Config_CenterRow_Computed_Correctly()
    {
        //Given
        var config = new GameConfig("Alice", "Bob", 15, 19, 30);
        //Then
        config.CenterRow.ShouldBe(8);
    }

    [Fact]
    public void Test_Config_GoalColumns_Correct()
    {
        //Given
        var config = new GameConfig("Alice", "Bob", 3, 7, 10);
        //Then
        config.Player1GoalColumn.ShouldBe(7);
        config.Player2GoalColumn.ShouldBe(1);
    }
}
