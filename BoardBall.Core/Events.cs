using System.Drawing;

namespace BoardBall.Core;

public abstract record Event;

public static class Events {
    public record BallPlaced(Point Point) : Event;
    public record CurrentPlayerChanged(Player Player) : Event;
    public record GameStarted(GameConfig Config) : Event;
    public record FootballerPlaced(Point Point) : Event;
}