using System.Drawing;

namespace BoardBall.Core;

public class Game
{
    public const string GameAlreadyStarted = "started";
    public const string GameNotStarted     = "start";
    public const string WrongPlayer        = "player";
    public const string StackEmpty         = "stack";
    public const string LocationOccupied   = "occupied";
    public const string OutOfBounds        = "bounds";
    public const string InvalidJump        = "jump";
    public const string GameOver           = "gameover";
    public const string CannotSkip         = "skip";

    public State State { get; } = new();

    public IEnumerable<Event> Handle(Commands.StartGame cmd)
    {
        if (State.Status != GameStatus.NotStarted)
            throw new ArgumentException(GameAlreadyStarted);

        var events = new List<Event>
        {
            new Events.GameStarted(cmd.Config),
            new Events.BallPlaced(new Point(cmd.Config.CenterColumn, cmd.Config.CenterRow)),
            new Events.CurrentPlayerChanged(Player.One),
        };

        State.Apply(events);
        return events;
    }

    public IEnumerable<Event> Handle(Commands.PlaceStone cmd)
    {
        if (State.Status != GameStatus.InProgress)
            throw new ArgumentException(State.Status == GameStatus.NotStarted ? GameNotStarted : GameOver);
        if (cmd.Player != State.CurrentPlayer)
            throw new ArgumentException(WrongPlayer);
        if (State.ChainInProgress)
            throw new ArgumentException(WrongPlayer);
        if (State.StackCount == 0)
            throw new ArgumentException(StackEmpty);

        var config = State.Config!;
        var loc = cmd.Location;

        if (loc.X < 1 || loc.X > config.Columns || loc.Y < 1 || loc.Y > config.Rows)
            throw new ArgumentException(OutOfBounds);
        if (loc == State.Ball)
            throw new ArgumentException(LocationOccupied);
        if (State.Stones.Contains(loc))
            throw new ArgumentException(LocationOccupied);

        var events = new List<Event> { new Events.StonePlaced(loc) };
        State.Apply(events);

        var turnEvents = AdvanceTurn();
        State.Apply(turnEvents);
        events.AddRange(turnEvents);

        return events;
    }

    public IEnumerable<Event> Handle(Commands.JumpBall cmd)
    {
        if (State.Status != GameStatus.InProgress)
            throw new ArgumentException(State.Status == GameStatus.NotStarted ? GameNotStarted : GameOver);
        if (cmd.Player != State.CurrentPlayer)
            throw new ArgumentException(WrongPlayer);

        var hop = JumpEngine.TryHop(State, cmd.Direction)
            ?? throw new ArgumentException(InvalidJump);

        var events = new List<Event>
        {
            new Events.BallMoved(State.Ball, hop.Landing),
            new Events.StonesReturned(hop.JumpedStones),
        };
        State.Apply(events);

        if (hop.IsGoal)
        {
            // Determine winner by ball position
            var winner = DetermineWinner();
            var winEvent = new Events.GameWon(winner);
            State.Apply([winEvent]);
            events.Add(winEvent);
            return events;
        }

        var chainDirs = JumpEngine.GetAvailableDirections(State);
        if (chainDirs.Count > 0)
        {
            var chainEvent = new Events.ChainContinuationRequired(chainDirs);
            State.Apply([chainEvent]);
            events.Add(chainEvent);
        }
        else
        {
            var turnEvents = AdvanceTurn();
            State.Apply(turnEvents);
            events.AddRange(turnEvents);
        }

        return events;
    }

    public IEnumerable<Event> Handle(Commands.SkipTurn cmd)
    {
        if (State.Status != GameStatus.InProgress)
            throw new ArgumentException(State.Status == GameStatus.NotStarted ? GameNotStarted : GameOver);
        if (cmd.Player != State.CurrentPlayer)
            throw new ArgumentException(WrongPlayer);
        if (State.ChainInProgress)
            throw new ArgumentException(WrongPlayer);
        if (State.StackCount > 0 || JumpEngine.HasAnyValidJump(State))
            throw new ArgumentException(CannotSkip);

        var events = new List<Event> { new Events.TurnSkipped(cmd.Player) };
        State.Apply(events);

        if (State.ConsecutiveSkips >= 2)
        {
            var drawEvent = new Events.GameDrawn();
            State.Apply([drawEvent]);
            events.Add(drawEvent);
            return events;
        }

        var turnEvents = AdvanceTurn();
        State.Apply(turnEvents);
        events.AddRange(turnEvents);
        return events;
    }

    // Switches to the other player; auto-skips them if they have no moves.
    private List<Event> AdvanceTurn()
    {
        var next = State.CurrentPlayer == Player.One ? Player.Two : Player.One;
        var events = new List<Event> { new Events.CurrentPlayerChanged(next) };
        State.Apply(events);

        // Auto-skip if next player also has no moves
        if (State.StackCount == 0 && !JumpEngine.HasAnyValidJump(State))
        {
            var skipEvent = new Events.TurnSkipped(next);
            State.Apply([skipEvent]);
            events.Add(skipEvent);

            if (State.ConsecutiveSkips >= 2)
            {
                var drawEvent = new Events.GameDrawn();
                State.Apply([drawEvent]);
                events.Add(drawEvent);
            }
        }

        return events;
    }

    private Player DetermineWinner()
    {
        var config = State.Config!;
        // Ball at or beyond last column → Player1 wins
        // Ball at or beyond first column (<=1) → Player2 wins
        return State.Ball.X >= config.Player1GoalColumn ? Player.One : Player.Two;
    }
}
