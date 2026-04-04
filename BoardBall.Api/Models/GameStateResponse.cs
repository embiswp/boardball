namespace BoardBall.Api.Models;

public record PointDto(int Row, int Column);

public record GameStateResponse(
    string         Status,
    string?        Winner,
    string?        CurrentPlayer,
    string?        Player1Name,
    string?        Player2Name,
    int            Rows,
    int            Columns,
    int            StackCount,
    PointDto       Ball,
    List<PointDto> Stones,
    bool           ChainInProgress,
    List<string>   AvailableJumpDirections,
    bool           CanPlaceStone,
    bool           CanJump,
    bool           MustSkip,
    int            ConsecutiveSkips);
