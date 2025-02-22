namespace BoardBall.Core
{
    public class Game
    {
        public string? Player1 { get; private set; }
        public string? Player2 { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public int Footballers { get; private set; }
        public GameState State { get; internal set; }

        public void Start(string player1, string player2, int rows, int columns, int footballers)
        {
            //player1
            if (string.IsNullOrEmpty(player1)) throw new ArgumentException(nameof(player1));
            Player1 = player1;

            //player2
            if (string.IsNullOrEmpty(player2)) throw new ArgumentException(nameof(player2));
            Player2 = player2;

            //Rows
            if (rows % 2 != 0 & rows >= 1) {
                Rows = rows;
            }
            else {
                throw new ArgumentException(nameof(rows));
            }
            //Columns
            if (columns %2 != 0 & columns != 0 & columns > 3 ) {
                Columns = columns;
            }
            else{
                throw new ArgumentException(nameof(columns));
            }

            //footballers
            if (footballers % 2 == 0){
                Footballers = footballers;
            }
            else{
                throw new ArgumentException(nameof(footballers));
            }
            State = GameState.Playing;
        }
    }
}
