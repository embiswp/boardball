using System.Drawing;

namespace BoardBall.Core;

public class State {
    public Point Ball;
    public List<Point> Footballers = new List<Point>();
    
    public Player? CurrentPlayer;
    public GameConfig? Config;

    public IEnumerable<Event> ProcessEvents(params Event[] events) {
        foreach (var @event in events) {
            switch(@event)
            {
                case Events.GameStarted gs: Config = gs.Config; break;
                case Events.CurrentPlayerChanged cpc: CurrentPlayer = cpc.Player; break;
                case Events.BallPlaced bp: Ball = bp.Point; break;
                case Events.FootballerPlaced fp: Footballers.Add(fp.Point); break;
                default: throw new NotImplementedException();
            };
        }
        return events;
    }

}