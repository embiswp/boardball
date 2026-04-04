using System.Drawing;

namespace BoardBall.Core;

public static class Commands
{
    public record StartGame(GameConfig Config);
    public record PlaceStone(Player Player, Point Location);
    public record JumpBall(Player Player, Direction Direction);
    public record SkipTurn(Player Player);
}
