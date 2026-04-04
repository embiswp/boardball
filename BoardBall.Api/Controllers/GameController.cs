using System.Drawing;
using BoardBall.Api.Models;
using BoardBall.Core;
using Microsoft.AspNetCore.Mvc;

namespace BoardBall.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController(Game game) : ControllerBase
{
    [HttpPost("start")]
    public IActionResult Start([FromBody] StartGameRequest req)
    {
        try
        {
            var config = new GameConfig(req.Player1, req.Player2, req.Rows, req.Columns, req.StackSize);
            game.Handle(new Commands.StartGame(config));
            return Ok(BuildResponse());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        return Ok(BuildResponse());
    }

    [HttpPost("place-stone")]
    public IActionResult PlaceStone([FromBody] PlaceStoneRequest req)
    {
        try
        {
            game.Handle(new Commands.PlaceStone(req.Player, new Point(req.Column, req.Row)));
            return Ok(BuildResponse());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("jump")]
    public IActionResult Jump([FromBody] JumpRequest req)
    {
        try
        {
            game.Handle(new Commands.JumpBall(req.Player, req.Direction));
            return Ok(BuildResponse());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("skip")]
    public IActionResult Skip([FromBody] SkipRequest req)
    {
        try
        {
            game.Handle(new Commands.SkipTurn(req.Player));
            return Ok(BuildResponse());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private GameStateResponse BuildResponse()
    {
        var state = game.State;
        var config = state.Config;

        var availableDirs = state.Status == GameStatus.InProgress
            ? JumpEngine.GetAvailableDirections(state).Select(d => d.ToString()).ToList()
            : [];

        bool canPlaceStone = state.Status == GameStatus.InProgress
            && !state.ChainInProgress
            && state.StackCount > 0;

        bool canJump = state.Status == GameStatus.InProgress
            && availableDirs.Count > 0;

        bool mustSkip = state.Status == GameStatus.InProgress
            && !state.ChainInProgress
            && state.StackCount == 0
            && !canJump;

        return new GameStateResponse(
            Status:                  state.Status.ToString(),
            Winner:                  state.Winner?.ToString(),
            CurrentPlayer:           state.CurrentPlayer?.ToString(),
            Player1Name:             config?.Player1,
            Player2Name:             config?.Player2,
            Rows:                    config?.Rows ?? 0,
            Columns:                 config?.Columns ?? 0,
            StackCount:              state.StackCount,
            Ball:                    new PointDto(state.Ball.Y, state.Ball.X),
            Stones:                  state.Stones.Select(p => new PointDto(p.Y, p.X)).ToList(),
            ChainInProgress:         state.ChainInProgress,
            AvailableJumpDirections: availableDirs,
            CanPlaceStone:           canPlaceStone,
            CanJump:                 canJump,
            MustSkip:                mustSkip,
            ConsecutiveSkips:        state.ConsecutiveSkips);
    }
}
