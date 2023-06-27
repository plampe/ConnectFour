using System;

namespace ConnectFour
{
    // Enum para representar los diferentes estados del juego
    enum GameState
    {
        InProgress,
        Draw,
        Player1Wins,
        Player2Wins
    }

    // Clase base para representar un jugador
    abstract class Player
    {
        protected char symbol; // Símbolo del jugador
        protected string name; // Nombre del jugador

        public Player(char symbol)
        {
            this.symbol = symbol;
            this.name = "Player";
        }

        public Player(char symbol, string name)
        {
            this.symbol = symbol;
            this.name = name;
        }

        public abstract int GetNextMove(Board board); // Método abstracto para obtener el siguiente movimiento del jugador

        public char GetSymbol()
        {
            return symbol;
        }

        public string GetName()
        {
            return name;
        }
    }

    // Clase para representar un jugador humano
    class HumanPlayer : Player
    {
        public HumanPlayer(char symbol) : base(symbol)
        {
        }

        public HumanPlayer(char symbol, string name) : base(symbol, name)
        {
        }

        public override int GetNextMove(Board board)
        {
            Console.Write("{0}, enter your column (0-6): ", GetName() + " is the " + GetSymbol());
            string input = Console.ReadLine();
            int column;
            while (!int.TryParse(input, out column) || column < 0 || column >= Board.Columns || !board.IsValidMove(column))
            {
                Console.WriteLine("Invalid move. Please try again.");
                Console.Write("{0}, enter your column (0-6): ", GetName() + " is the " + GetSymbol());
                input = Console.ReadLine();
            }
            return column;
        }
    }

    // Clase para representar un jugador de la computadora
    class ComputerPlayer : Player
    {
        private Random random;

        public ComputerPlayer(char symbol) : base(symbol)
        {
            random = new Random();
        }

        public ComputerPlayer(char symbol, string name) : base(symbol, name)
        {
            random = new Random();
        }

        public override int GetNextMove(Board board)
        {
            int column;
            do
            {
                column = random.Next(0, Board.Columns);
            } while (!board.IsValidMove(column));

            return column;
        }
    }

    // Clase para representar el tablero del juego
    class Board
    {
        public const int Rows = 6;
        public const int Columns = 7;

        private char[,] grid; // Matriz para almacenar el estado del tablero

        public Board()
        {
            grid = new char[Rows, Columns];
        }

        public void Display()
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Console.Write(grid[row, col] == '\0' ? "." : grid[row, col].ToString());
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public bool IsFull()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (grid[Rows - 1, col] == '\0')
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsValidMove(int column)
        {
            return column >= 0 && column < Columns && grid[Rows - 1, column] == '\0';
        }

        public bool MakeMove(int column, char symbol)
        {
            for (int row = 0; row < Rows; row++)
            {
                if (grid[row, column] == '\0')
                {
                    grid[row, column] = symbol;
                    return true;
                }
            }
            return false;
        }

        public GameState GetGameState()
        {
            // Comprobar filas
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (grid[row, col] != '\0' &&
                        grid[row, col] == grid[row, col + 1] &&
                        grid[row, col] == grid[row, col + 2] &&
                        grid[row, col] == grid[row, col + 3])
                    {
                        return grid[row, col] == 'X' ? GameState.Player1Wins : GameState.Player2Wins;
                    }
                }
            }

            // Comprobar columnas
            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows - 3; row++)
                {
                    if (grid[row, col] != '\0' &&
                        grid[row, col] == grid[row + 1, col] &&
                        grid[row, col] == grid[row + 2, col] &&
                        grid[row, col] == grid[row + 3, col])
                    {
                        return grid[row, col] == 'X' ? GameState.Player1Wins : GameState.Player2Wins;
                    }
                }
            }

            // Comprobar diagonales ascendentes
            for (int row = 0; row < Rows - 3; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (grid[row, col] != '\0' &&
                        grid[row, col] == grid[row + 1, col + 1] &&
                        grid[row, col] == grid[row + 2, col + 2] &&
                        grid[row, col] == grid[row + 3, col + 3])
                    {
                        return grid[row, col] == 'X' ? GameState.Player1Wins : GameState.Player2Wins;
                    }
                }
            }

            // Comprobar diagonales descendentes
            for (int row = 3; row < Rows; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (grid[row, col] != '\0' &&
                        grid[row, col] == grid[row - 1, col + 1] &&
                        grid[row, col] == grid[row - 2, col + 2] &&
                        grid[row, col] == grid[row - 3, col + 3])
                    {
                        return grid[row, col] == 'X' ? GameState.Player1Wins : GameState.Player2Wins;
                    }
                }
            }

            if (IsFull())
            {
                return GameState.Draw;
            }

            return GameState.InProgress;
        }
    }

    class ConnectFourGame
    {
        private Player player1;
        private Player player2;
        private Board board;

        public ConnectFourGame(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            board = new Board();
        }

        public void Start()
        {
            Player currentPlayer = player1;
            GameState gameState = GameState.InProgress;

            while (gameState == GameState.InProgress)
            {
                board.Display();
                int column = currentPlayer.GetNextMove(board);

                if (board.MakeMove(column, currentPlayer.GetSymbol()))
                {
                    gameState = board.GetGameState();

                    if (gameState == GameState.InProgress)
                    {
                        currentPlayer = (currentPlayer == player1) ? player2 : player1;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid move. Please try again.");
                }
            }

            board.Display();
            Console.WriteLine(GetGameResultMessage(gameState));
        }

        private string GetGameResultMessage(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Draw:
                    return "The game ended in a draw.";
                case GameState.Player1Wins:
                    return $"{player1.GetName()} wins!";
                case GameState.Player2Wins:
                    return $"{player2.GetName()} wins!";
                default:
                    return "";
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connect Four Game");
            Console.WriteLine("-----------------");
            Console.WriteLine();

            Console.Write("Enter Player 1's name: ");
            string player1Name = Console.ReadLine();
            Player player1 = new HumanPlayer('X', player1Name);

            Console.WriteLine();

            Console.WriteLine("Select game mode:");
            Console.WriteLine("1. Two Players");
            Console.WriteLine("2. Player vs. Computer");

            Console.Write("Enter your choice (1-2): ");
            string input = Console.ReadLine();

            while (input != "1" && input != "2")
            {
                Console.WriteLine("Invalid choice. Please try again.");
                Console.Write("Enter your choice (1-2): ");
                input = Console.ReadLine();
            }

            Console.WriteLine();

            Player player2;
            if (input == "1")
            {
                Console.Write("Enter Player 2's name: ");
                string player2Name = Console.ReadLine();
                player2 = new HumanPlayer('O', player2Name);
            }
            else
            {
                player2 = new ComputerPlayer('O', "Computer");
            }

            ConnectFourGame game = new ConnectFourGame(player1, player2);
            game.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
