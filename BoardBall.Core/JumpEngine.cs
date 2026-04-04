using System.Drawing;

namespace BoardBall.Core;

public record HopResult(Point Landing, IReadOnlyList<Point> JumpedStones, bool IsGoal);

public static class JumpEngine
{
    private static readonly Dictionary<Direction, (int dr, int dc)> Vectors = new()
    {
        [Direction.Up]        = (-1,  0),
        [Direction.Down]      = ( 1,  0),
        [Direction.Left]      = ( 0, -1),
        [Direction.Right]     = ( 0,  1),
        [Direction.UpLeft]    = (-1, -1),
        [Direction.UpRight]   = (-1,  1),
        [Direction.DownLeft]  = ( 1, -1),
        [Direction.DownRight] = ( 1,  1),
    };

    public static HopResult? TryHop(State state, Direction direction)
    {
        var config = state.Config!;
        var (dr, dc) = Vectors[direction];

        // Collect consecutive stones from ball in the given direction
        var stones = new List<Point>();
        var cursor = new Point(state.Ball.X + dc, state.Ball.Y + dr);

        while (IsOnBoard(cursor, config) && state.Stones.Contains(cursor))
        {
            stones.Add(cursor);
            cursor = new Point(cursor.X + dc, cursor.Y + dr);
        }

        if (stones.Count == 0)
            return null; // must jump at least one stone

        var landing = cursor;

        if (IsOnBoard(landing, config))
        {
            if (state.Stones.Contains(landing))
                return null; // landing square occupied
            return new HopResult(landing, stones, IsGoal(landing, config));
        }
        else
        {
            // Off-board: only valid past left/right goal lines, not top/bottom
            if (dc == 0)
                return null; // pure vertical — exited top or bottom

            if (landing.Y < 1 || landing.Y > config.Rows)
                return null; // diagonal exit through top or bottom wall

            // Horizontal or diagonal exit past left/right edge → goal
            return new HopResult(landing, stones, true);
        }
    }

    public static IReadOnlyList<Direction> GetAvailableDirections(State state)
    {
        var available = new List<Direction>();
        foreach (Direction dir in Enum.GetValues<Direction>())
        {
            if (TryHop(state, dir) != null)
                available.Add(dir);
        }
        return available;
    }

    public static bool HasAnyValidJump(State state)
        => GetAvailableDirections(state).Count > 0;

    private static bool IsOnBoard(Point p, GameConfig config)
        => p.X >= 1 && p.X <= config.Columns && p.Y >= 1 && p.Y <= config.Rows;

    // Position-based goal detection: col<=1 means Player2 wins, col>=Columns means Player1 wins
    private static bool IsGoal(Point p, GameConfig config)
        => p.X <= config.Player2GoalColumn || p.X >= config.Player1GoalColumn;
}
