namespace BoardBall.Core;

public class GameConfig
{
    public string Player1  { get; }
    public string Player2  { get; }
    public int    Rows     { get; }
    public int    Columns  { get; }
    public int    StackSize { get; }

    public int CenterRow    => (Rows    / 2) + 1;
    public int CenterColumn => (Columns / 2) + 1;
    public int Player1GoalColumn => Columns;  // Player1 attacks the last column
    public int Player2GoalColumn => 1;        // Player2 attacks the first column

    public GameConfig(string player1, string player2, int rows, int columns, int stackSize)
    {
        if (string.IsNullOrEmpty(player1)) throw new ArgumentException("player1");
        if (string.IsNullOrEmpty(player2)) throw new ArgumentException("player2");
        if (rows < 1 || rows % 2 == 0)    throw new ArgumentException("rows");
        if (columns < 5 || columns % 2 == 0) throw new ArgumentException("columns");
        if (stackSize <= 0)                throw new ArgumentException("stack");

        Player1   = player1;
        Player2   = player2;
        Rows      = rows;
        Columns   = columns;
        StackSize = stackSize;
    }
}
