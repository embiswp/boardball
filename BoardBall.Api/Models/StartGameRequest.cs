namespace BoardBall.Api.Models;

public record StartGameRequest(
    string Player1,
    string Player2,
    int    Rows,
    int    Columns,
    int    StackSize);
