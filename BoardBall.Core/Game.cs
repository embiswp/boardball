using System.Drawing;

namespace BoardBall.Core;

public class Game {
    public Point Ball;
    public List<Point> Footballers;
    
    private CurrentPlayer currentPlayer;
    private GameConfig config;

    public Game(GameConfig config) {
        this.Ball = new Point((config.Rows / 2) + 1, (config.Columns / 2) + 1);
        this.Footballers = new List<Point>();
        this.currentPlayer = CurrentPlayer.Player1;
        this.config = config;
    }

    public void Player1AddsFootballer(Point footballer)
    {
        if (this.Footballers.Count() == config.Footballers) throw new GameException("All the footballers are on the field");
        if (currentPlayer == CurrentPlayer.Player2) throw new GameException("Player2 should come");
        if (footballer == this.Ball) throw new GameException("Player1 cannot put footballer on the ball");
        this.Footballers.Add(footballer);
        this.currentPlayer = CurrentPlayer.Player2;
    }

    public void Player2AddsFootballer(Point footballer)
    {
        if (this.Footballers.Count() == config.Footballers) throw new GameException("All the footballers are on the field");
        if (currentPlayer == CurrentPlayer.Player1) throw new GameException("Player1 should come");
        if (footballer == this.Ball) throw new GameException("Player2 cannot place footballer on the ball");
        if (this.Footballers.Any(f => f == footballer)) throw new GameException("Player2 cannot place on existing footballer");
        this.Footballers.Add(footballer);
        this.currentPlayer = CurrentPlayer.Player1;
    }
}

public enum CurrentPlayer {
    Player1,
    Player2,
}
