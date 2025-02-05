using System.Drawing;
using Shouldly;

namespace BoardBall.Core.Tests;

public class GameTests
{
    [Fact]
    public void Test()
    {
        var test = 1;
        test.ShouldBe(1);
    }

    [Fact]
    public void Test_Game_Start_Sets_Properties()
    {
        //Given
        var game = new Game();
        //When
        game.Start("Péter", "Balázs", 7, 9, 10);
        //Then
        game.Player1.ShouldBe("Péter");
        game.Player2.ShouldBe("Balázs");
        game.Rows.ShouldBe(7);
        game.Columns.ShouldBe(9);
        game.Footballers.ShouldBe(10);
    }

    [Fact]
    public void Test_After_Successful_Game_Start_Game_State_Is_Playing()
    {
        //Given
        var game = new Game();
        //When
        game.Start("Péter", "Balázs", 7, 9, 10);
        //Then
        game.State.ShouldBe(GameState.Playing);
    }

    [Fact]
    public void Test_Player1_Name_Is_Empty()
    {
        //Given
        var game = new Game();
        //When
        try
        {
            game.Start("", "Balázs", 7, 9, 10);
            Assert.Fail();
        }
        catch (ArgumentException exception)
        {
            System.Console.WriteLine(exception);
            //Then
            exception.Message.ShouldBe("player1");
        }
    }

    [Fact]
    public void Test_Player2_Name_Is_Empty()
    {
        //Given
        var game = new Game();
        //When
        try
        {
            game.Start("Péter", "", 0, 0, 0);
            Assert.Fail();
        }
        catch (ArgumentException exception)
        {
            System.Console.WriteLine(exception);
            //Then
            exception.Message.ShouldBe("player2");
        }
    }

    //Test for error case when Rows are even
    [Fact]
    public void Test_Rows_are_even()
    {
        var game = new Game();
        try
        {
            game.Start("Péter", "Balázs", 2, 0, 0);
            Assert.Fail();
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine("SKILL ISSEU?!");
            Console.WriteLine(exception);
            exception.Message.ShouldBe("rows");
        }

    }
    //Test for error case when Columns are even
    //Test for error case when Columns are 0 or less than 3
    //Test for error case when Rows are 0 or less than 1
    //Test for error case when number of footballers is odd

}

public enum GameState
{
    NotStarted,
    Playing,
}

public class Game
{
    public string? Player1 { get; private set; }
    public string? Player2 { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public int Footballers { get; private set; }
    public GameState State { get; internal set; }

    public void Start(string player1, string player2, int rows, int columns, int footballers)
    {
        if (string.IsNullOrEmpty(player1)) throw new ArgumentException(nameof(player1));
        Player1 = player1;
        if (string.IsNullOrEmpty(player2)) throw new ArgumentException(nameof(player2));
        Player2 = player2;
        if (columns % 2 != 0) {
            Rows = rows;
        }
        else{
            throw new ArgumentException(nameof(rows));
        }
        Columns = columns;
        Footballers = footballers;
        State = GameState.Playing;
    }
}
