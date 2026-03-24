using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Assignment
{
    public class GamePlay
    {
        private Grid _board;
        private Player _player1;
        private Player _player2;
        private int _currentPlayerIndex;
        private int _discsToWin;

        public Player Player1 => _player1;
        public Player Player2 => _player2;
        public Grid Grid => _board;

        public GamePlay(int rows, int cols, bool vsAI)
        {
            _board = new Grid(rows, cols);
            _discsToWin = (int)Math.Floor(rows * cols * 0.1);

            int totalDiscs = (int)Math.Floor(rows * cols / 2.0);
            _player1 = new Player("Player 1", '@', totalDiscs);
            _player2 = new Player(vsAI ? "Computer" : "Player 2", '#', totalDiscs, vsAI);

            _currentPlayerIndex = 0;
        }

        // Main gameplay loop
        public void Start()
        {
            while (true)
            {
                Console.WriteLine($"You need {_discsToWin} consecutive discs for winning the game");
                _board.Display();

                var currentPlayer = _currentPlayerIndex == 0 ? _player1 : _player2;
                Console.WriteLine($"{currentPlayer.Name}'s turn");

                int chosenColumn;
                Discs chosenType = Discs.Ordinary;

                if (!currentPlayer.IsAI)
                {
                    HandlePlayerOptions(currentPlayer);
                    chosenType = SelectDiscs(currentPlayer);
                    chosenColumn = SelectColumn(currentPlayer);
                }
                else
                {
                    // AI turn
                    var move = Computer.DecideMove(_board, currentPlayer, _discsToWin);
                    chosenColumn = move.column;
                    chosenType = move.type;
                }

                DropDiscAndApplyEffects(currentPlayer, chosenColumn, chosenType);

                // Check for win or draw
                if (_board.HasWinner(currentPlayer.Disc, _discsToWin))
                {
                    _board.Display();
                    Console.WriteLine($"{currentPlayer.Name} wins the game!");
                    return;
                }

                if (_board.IsGridFull())
                {
                    _board.Display();
                    Console.WriteLine("It's a tie!");
                    return;
                }

                _currentPlayerIndex = 1 - _currentPlayerIndex;
            }
        }

        private void HandlePlayerOptions(Player currentPlayer)
        {
            while (true)
            {
                Console.WriteLine("Press 'H' for help, 'S' to save the game, or Enter key to play:");
                var input = (Console.ReadLine() ?? "").Trim().ToUpper();

                if (input == "H") ShowHelp(currentPlayer);
                else if (input == "S") { Save("game.json"); Console.WriteLine("Your game has been saved successfully!"); }
                else if (input == "") break;
                else Console.WriteLine("Enter valid input");
            }
        }

        private void ShowHelp(Player player)
        {
            Console.WriteLine("\nOrdinary disc forms alignments");
            Console.WriteLine("Boring disc clears the discs above it in the column, returns them, then become ordinary");
            Console.WriteLine("Exploding disc destroys itself and adjacent discs.");
            Console.WriteLine("=*=*=*=*=*=*=*=*=\n");
        }

        private Discs SelectDiscs(Player player)
        {
            while (true)
            {
                Console.WriteLine($"Choose your disc: 1.Ordinary/2.Boring/3.Exploding");
                Console.WriteLine($"Available - Ordinary: {player.OrdinaryDiscs}, Boring: {player.BoringDiscs}, Exploding: {player.ExplodingDiscs}");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (player.OrdinaryDiscs > 0) return Discs.Ordinary;
                        Console.WriteLine("No ordinary discs left."); break;
                    case "2":
                        if (player.BoringDiscs > 0) return Discs.Boring;
                        Console.WriteLine("No boring discs left."); break;
                    case "3":
                        if (player.ExplodingDiscs > 0) return Discs.Exploding;
                        Console.WriteLine("No exploding discs left."); break;
                    default:
                        Console.WriteLine("Enter valid input"); break;
                }
            }
        }

        private int SelectColumn(Player player)
        {
            while (true)
            {
                Console.Write($"Enter grid column to play: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int col) && col >= 1 && col <= _board.Cols)
                {
                    col--; // convert to 0-based index
                    if (_board.IsColumnAvailable(col)) return col;
                    Console.WriteLine("Column is full, choose another.");
                }
                else
                {
                    Console.WriteLine("Enter valid input");
                }
            }
        }

        private void DropDiscAndApplyEffects(Player player, int column, Discs type)
        {
            char discChar = player.Disc;
            if (type == Discs.Boring) discChar = (discChar == '@') ? 'B' : 'b';
            else if (type == Discs.Exploding) discChar = (discChar == '@') ? 'E' : 'e';

            _board.PlaceDiscWithEffects(column, discChar, type, _player1, _player2);

            // Consume disc
            player.ConsumeDisc(type);
        }

        public void Save(string path)
        {
            var data = new Save
            {
                Rows = _board.Rows,
                Cols = _board.Cols,
                Grid = _board.ToJagged(),
                NeededToWin = _discsToWin,
                CurrentPlayerIndex = _currentPlayerIndex,
                VsAI = _player2.IsAI,

                P1BoringLeft = _player1.BoringDiscs,
                P1ExplodingLeft = _player1.ExplodingDiscs,
                P1OrdinaryLeft = _player1.OrdinaryDiscs,

                P2BoringLeft = _player2.BoringDiscs,
                P2ExplodingLeft = _player2.ExplodingDiscs,
                P2OrdinaryLeft = _player2.OrdinaryDiscs
            };

            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }

        public static GamePlay? Load(string path)
        {
            if (!File.Exists(path)) return null;

            var data = JsonSerializer.Deserialize<Save>(File.ReadAllText(path));
            if (data == null) return null;

            var game = new GamePlay(data.Rows, data.Cols, data.VsAI)
            {
                _discsToWin = data.NeededToWin,
                _currentPlayerIndex = data.CurrentPlayerIndex
            };

            game._board = new Grid(data.Rows, data.Cols, data.Grid);
            game._player1.ResetDiscs(data.P1OrdinaryLeft, data.P1BoringLeft, data.P1ExplodingLeft);
            game._player2.ResetDiscs(data.P2OrdinaryLeft, data.P2BoringLeft, data.P2ExplodingLeft);

            return game;
        }

        // For automated testing sequences
        public void PlayTestSequence(string sequence)
        {
            var moves = sequence.Split(',', StringSplitOptions.RemoveEmptyEntries);
            _currentPlayerIndex = 0;

            foreach (var move in moves)
            {
                if (string.IsNullOrWhiteSpace(move)) continue;

                char discChar = char.ToUpper(move[0]);
                Discs type;
                switch (discChar)
                {
                    case 'O': type = Discs.Ordinary; break;
                    case 'B': type = Discs.Boring; break;
                    case 'E': type = Discs.Exploding; break;
                    default:
                        Console.WriteLine("Enter valid input");
                        continue;
                }

                if (!int.TryParse(move.Substring(1), out int col) || col < 1 || col > _board.Cols)
                {
                    Console.WriteLine("Enter valid input");
                    continue;
                }

                col--; // zero-based index
                var currentPlayer = _currentPlayerIndex == 0 ? _player1 : _player2;

                if (!_board.IsColumnAvailable(col))
                {
                    Console.WriteLine($"Column {col + 1} full. Skipping move.");
                    continue;
                }

                char discToDrop = currentPlayer.Disc;
                if (type == Discs.Boring) discToDrop = (discToDrop == '@') ? 'B' : 'b';
                else if (type == Discs.Exploding) discToDrop = (discToDrop == '@') ? 'E' : 'e';

                _board.PlaceDiscWithEffects(col, discToDrop, type, _player1, _player2);
                currentPlayer.ConsumeDisc(type);

                Console.WriteLine($"{currentPlayer.Name} played {type} disc in column {col + 1}");
                _board.Display();
                
                if (_board.HasWinner(currentPlayer.Disc, _discsToWin))
                {
                    Console.WriteLine($"{currentPlayer.Name} wins the game!");
                    return;
                }

                if (_board.IsGridFull())
                {
                    Console.WriteLine("It's a tie!");
                    return;
                }

                _currentPlayerIndex = 1 - _currentPlayerIndex;
            }

            Console.WriteLine("Sequence completed... Verify the grid position!");
        }
    }
}
