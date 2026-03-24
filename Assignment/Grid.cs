using System;
using System.Text;

namespace Assignment
{
    public class Grid
    {
        private char[,] _grid;

        public int Rows { get; }
        public int Cols { get; }

        // Initialize empty board
        public Grid(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            _grid = new char[rows, cols];
            ClearGrid();
        }

        // Initialize from jagged array
        public Grid(int rows, int cols, char[][] jaggedGrid)
        {
            Rows = rows;
            Cols = cols;
            _grid = new char[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    _grid[r, c] = jaggedGrid[r][c];
        }

        private void ClearGrid()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _grid[r, c] = ' ';
        }

        // Display the board to console
        public void Display()
        {
            Console.Write(GetGridString());
        }

        private string GetGridString()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            for (int r = 0; r < Rows; r++)
            {
                sb.Append("|");
                for (int c = 0; c < Cols; c++)
                    sb.Append($" {_grid[r, c]} |");
                sb.AppendLine();
            }

            sb.AppendLine(new string('-', Cols * 4 + 1));
            sb.Append(" ");
            for (int c = 1; c <= Cols; c++)
                sb.Append($" {c}  ");
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        // Check if a column has space for a disc
        public bool IsColumnAvailable(int col) => _grid[0, col] == ' ';

        // Drop a disc without animations/effects
        public bool PlaceDisc(int col, char disc, Discs type)
        {
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (_grid[r, col] == ' ')
                {
                    _grid[r, col] = disc;
                    return true;
                }
            }
            return false;
        }

        // Drop a disc with visual frames and special effects
        public bool PlaceDiscWithEffects(int col, char disc, Discs type, Player p1, Player p2)
        {
            string playerName = (disc == '@' || disc == 'B' || disc == 'E')
                ? p1.Name
                : p2.IsAI ? "Computer" : p2.Name;

            Console.WriteLine($"\n>>> {playerName} played {type} disc in column {col + 1} <<<");
            Console.WriteLine("Grid before playing:");
            Display();
            Console.WriteLine("Press Enter");
            Console.ReadLine();

            int landingRow = GetLandingRow(col);
            if (landingRow == -1) return false;

            _grid[landingRow, col] = disc;
            Console.WriteLine("Grid after playing:");
            Display();
            Console.WriteLine("Press Enter");
            Console.ReadLine();

            if (type == Discs.Boring)
            {
                ActivateBoringDisc(col, landingRow, disc, p1, p2);
                Console.WriteLine("Boring Disc:");
                Display();
                Console.WriteLine("Press Enter");
                Console.ReadLine();
            }
            else if (type == Discs.Exploding)
            {
                TriggerExplosion(col, landingRow);
                Console.WriteLine("Exploding Disc:");
                Display();
                Console.WriteLine("Press Enter");
                Console.ReadLine();

                ApplyGravity();
                Console.WriteLine("Final Grid:");
                Display();
                Console.WriteLine("Press Enter");
                Console.ReadLine();
            }

            return true;
        }

        private int GetLandingRow(int col)
        {
            for (int r = Rows - 1; r >= 0; r--)
                if (_grid[r, col] == ' ')
                    return r;
            return -1;
        }

        private void ActivateBoringDisc(int col, int row, char discChar, Player p1, Player p2)
        {
            for (int r = row + 1; r < Rows; r++)
            {
                char cell = _grid[r, col];
                if (cell != ' ')
                {
                    if (cell == '@') p1.ReclaimDisc(Discs.Ordinary);
                    else if (cell == '#') p2.ReclaimDisc(Discs.Ordinary);
                    else if (cell == 'B') p1.ReclaimDisc(Discs.Boring);
                    else if (cell == 'b') p2.ReclaimDisc(Discs.Boring);
                    else if (cell == 'E') p1.ReclaimDisc(Discs.Exploding);
                    else if (cell == 'e') p2.ReclaimDisc(Discs.Exploding);
                }
                _grid[r, col] = ' ';
            }

            _grid[row, col] = ' ';
            _grid[Rows - 1, col] = discChar;
        }

        private void TriggerExplosion(int col, int row)
        {
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    int r = row + dr;
                    int c = col + dc;
                    if (r >= 0 && r < Rows && c >= 0 && c < Cols)
                        _grid[r, c] = ' ';
                }
        }

        private void ApplyGravity()
        {
            for (int c = 0; c < Cols; c++)
            {
                int writeRow = Rows - 1;
                for (int r = Rows - 1; r >= 0; r--)
                {
                    if (_grid[r, c] != ' ')
                    {
                        char temp = _grid[r, c];
                        _grid[r, c] = ' ';
                        _grid[writeRow, c] = temp;
                        writeRow--;
                    }
                }
            }
        }

        public bool IsGridFull()
        {
            for (int c = 0; c < Cols; c++)
                if (_grid[0, c] == ' ') return false;
            return true;
        }

        public bool HasWinner(char disc, int neededToWin)
        {
            char ordinary = disc;
            char boring = ordinary == '@' ? 'B' : 'b';

            // Horizontal, vertical, diagonal (\ and /)
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    if (CheckDirection(r, c, 0, 1, neededToWin, ordinary, boring)) return true; // horizontal
                    if (CheckDirection(r, c, 1, 0, neededToWin, ordinary, boring)) return true; // vertical
                    if (CheckDirection(r, c, 1, 1, neededToWin, ordinary, boring)) return true; // diagonal \
                    if (CheckDirection(r, c, 1, -1, neededToWin, ordinary, boring)) return true; // diagonal /
                }

            return false;
        }

        private bool CheckDirection(int startRow, int startCol, int rowInc, int colInc, int needed, char ordinary, char boring)
        {
            int count = 0;
            for (int i = 0; i < needed; i++)
            {
                int r = startRow + i * rowInc;
                int c = startCol + i * colInc;
                if (r < 0 || r >= Rows || c < 0 || c >= Cols) return false;

                if (_grid[r, c] == ordinary || _grid[r, c] == boring) count++;
            }
            return count == needed;
        }

        public Grid Duplicate()
        {
            var clone = new Grid(Rows, Cols);
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    clone._grid[r, c] = _grid[r, c];
            return clone;
        }

        public char[][] ToJagged()
        {
            var result = new char[Rows][];
            for (int r = 0; r < Rows; r++)
            {
                result[r] = new char[Cols];
                for (int c = 0; c < Cols; c++)
                    result[r][c] = _grid[r, c];
            }
            return result;
        }
    }
}
