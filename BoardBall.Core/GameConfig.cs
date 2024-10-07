namespace BoardBall.Core;

public record GameConfig
{
    public const string Player1NameEmpty = "Player1 should not be empty";
    public const string Player2NameEmpty = "Player2 should not be empty";
    public const string ColumnNotOdd = "Column count should be odd";
    public const string RowNotOdd = "Rows count should be odd";
    public const string ColumnsCountAtLeastThree = "Columns count should be higher than 2";
    public const string EvenFootballers = "Footballers should be even";


    

    public GameConfig(string Player1, string Player2, int Columns, int Rows, int Footballers)
    {
        if (string.IsNullOrEmpty(Player1)) throw new ArgumentException(Player1NameEmpty);
        if (string.IsNullOrEmpty(Player2)) throw new ArgumentException(Player2NameEmpty);
        if (Columns % 2 == 0) throw new ArgumentException(ColumnNotOdd);
        if (Columns < 3) throw new ArgumentException(ColumnsCountAtLeastThree);
        if (Rows % 2 == 0) throw new ArgumentException(RowNotOdd);
        if (Footballers % 2 == 1) throw new ArgumentException(EvenFootballers);
        this.Player1 = Player1;
        this.Player2 = Player2;
        this.Columns = Columns;
        this.Rows = Rows;
        this.Footballers = Footballers;
    }

    public string Player1 { get; }
    public string Player2 { get; }
    public int Columns { get; }
    public int Rows { get; }
    public int Footballers { get; }
}