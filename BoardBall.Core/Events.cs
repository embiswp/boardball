using System.Drawing;

namespace BoardBall.Core;

public abstract record Event;

public static class Events
{
    public record GameStarted(GameConfig Config)                                    : Event;
    public record BallPlaced(Point Location)                                       : Event;
    public record CurrentPlayerChanged(Player Player)                              : Event;
    public record StonePlaced(Point Location)                                      : Event;
    public record BallMoved(Point From, Point To)                                  : Event;
    public record StonesReturned(IReadOnlyList<Point> Locations)                   : Event;
    public record TurnSkipped(Player Player)                                       : Event;
    public record ChainContinuationRequired(IReadOnlyList<Direction> Available)    : Event;
    public record GameWon(Player Winner)                                           : Event;
    public record GameDrawn()                                                      : Event;
}
