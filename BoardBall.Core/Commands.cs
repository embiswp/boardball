using System.Drawing;

namespace BoardBall.Core;

public static class Commands {
    public record StartGame(GameConfig Config);
    public record PlaceFootballer(Player player, Point location);
}