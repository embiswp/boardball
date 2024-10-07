using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;

namespace BoardBall.Core;

public class Game {
    public const string WrongPlayer = "Not this player is on turn";
    public const string CannotPlaceOnBall = "Footballer cannot be placed on ball";
    public const string CannotPlaceOnFootballer = "Footballer cannot be placed on already placed footballer";
    public const string NoMoreFreeFootballers = "There are no more free footballers to place";
    public const string CannotPlaceOutOfBounds = "Footballer cannot be placed out of bounds";

    public State state = new State();

    public Game(params Event[] events)
    {
        state.ProcessEvents(events);
    }

    public IEnumerable<Event> Handle(Commands.StartGame command) {
        var ballLocation = new Point((command.Config.Rows / 2) + 1, (command.Config.Columns / 2) + 1);
        return state.ProcessEvents(
            new Events.GameStarted(command.Config),
            new Events.BallPlaced(ballLocation), 
            new Events.CurrentPlayerChanged(Player.One));
    }

    public IEnumerable<Event> Handle(Commands.PlaceFootballer command) {
        CheckCurrentPlayer(command.player);
        CheckAvailableFootballer();
        CheckOutOfBounds(command.location);
        CheckBallPosition(command.location);
        CheckAllPlacedFootballers(command.location);
        return state.ProcessEvents(
            new Events.FootballerPlaced(command.location),
            new Events.CurrentPlayerChanged(OtherPlayer(command.player))
        );
    }

    private void CheckCurrentPlayer(Player player) {
        if (state.CurrentPlayer == null || state.CurrentPlayer != player)
            throw new ArgumentException(WrongPlayer);
    }

    private void CheckOutOfBounds(Point location) {
        if (location.X < 1 
            || location.X > state.Config.Columns 
            || location.Y < 1 
            || location.Y > state.Config.Rows)
            throw new ArgumentException(CannotPlaceOutOfBounds);
    }

    private void CheckBallPosition(Point location) {
        if (state.Ball == location)
            throw new ArgumentException(CannotPlaceOnBall);
    }

    private void CheckAvailableFootballer() {
        if (state.Footballers.Count == state.Config!.Footballers)
            throw new ArgumentException(NoMoreFreeFootballers);
    }

    private void CheckAllPlacedFootballers(Point location) {
        if (state.Footballers.Any(f => f == location))
            throw new ArgumentException(CannotPlaceOnFootballer);
    }

    private Player OtherPlayer(Player player) {
        if (state.CurrentPlayer == Player.One) return Player.Two;
        return Player.One;
    }
}

public enum Player {
    One,
    Two,
}
