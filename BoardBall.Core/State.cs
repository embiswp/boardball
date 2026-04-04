using System.Drawing;

namespace BoardBall.Core;

public class State
{
    public GameConfig?    Config           { get; private set; }
    public Player?        CurrentPlayer    { get; private set; }
    public Point          Ball             { get; private set; }
    public HashSet<Point> Stones           { get; private set; } = new();
    public int            StackCount       { get; private set; }
    public GameStatus     Status           { get; private set; } = GameStatus.NotStarted;
    public Player?        Winner           { get; private set; }
    public int            ConsecutiveSkips { get; private set; }
    public bool           ChainInProgress  { get; private set; }

    public void Apply(IEnumerable<Event> events)
    {
        foreach (var e in events)
        {
            switch (e)
            {
                case Events.GameStarted gs:
                    Config     = gs.Config;
                    StackCount = gs.Config.StackSize;
                    Status     = GameStatus.InProgress;
                    break;

                case Events.BallPlaced bp:
                    Ball = bp.Location;
                    break;

                case Events.CurrentPlayerChanged cpc:
                    CurrentPlayer   = cpc.Player;
                    ChainInProgress = false;
                    break;

                case Events.StonePlaced sp:
                    Stones.Add(sp.Location);
                    StackCount--;
                    ConsecutiveSkips = 0;
                    break;

                case Events.BallMoved bm:
                    Ball             = bm.To;
                    ConsecutiveSkips = 0;
                    break;

                case Events.StonesReturned sr:
                    foreach (var loc in sr.Locations)
                        Stones.Remove(loc);
                    StackCount += sr.Locations.Count;
                    break;

                case Events.TurnSkipped:
                    ConsecutiveSkips++;
                    break;

                case Events.ChainContinuationRequired:
                    ChainInProgress = true;
                    break;

                case Events.GameWon gw:
                    Status = GameStatus.Won;
                    Winner = gw.Winner;
                    break;

                case Events.GameDrawn:
                    Status = GameStatus.Draw;
                    break;
            }
        }
    }
}
