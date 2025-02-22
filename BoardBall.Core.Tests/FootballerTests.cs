using BoardBall.Core;
using System.Drawing;
using Shouldly;

namespace BoardBall.Core.Tests;

public class FootBallerTests
{
    [Fact]
    public void Test_for_error_if_GameState_Not_Playing()
    {
        //Given
        var game = new Game();
        //then
        try{
            //When
            game.PlaceFootballer("player",2,2);
            Assert.Fail();
        }
        catch(ArgumentException exception){
            exception.Message.ShouldBe("start");

        }
    }
    [Fact]
    public void Test_for_Placing_Footballer_On_Ball()
    {
        //Given
        var game = new Game();
        game.Start("Péter", "Balázs", 3,3,4);
        //When
        try{
            game.PlaceFootballer("Péter", 2,2);
            Assert.Fail();
        }
        catch (ArgumentException exception){
            exception.Message.ShouldBe("ball");
        }
        //then
    }


}

