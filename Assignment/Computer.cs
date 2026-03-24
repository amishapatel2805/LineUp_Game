using System;
using System.Collections.Generic;

namespace Assignment
{
    public static class Computer
    {
        private static readonly Random _random = new Random();

        // Determines the AI's next move: column and disc type
        public static (int column, Discs type) DecideMove(Grid board, Player aiPlayer, int discsToWin)
        {
            // 1. Pick a disc type from available ones
            List<Discs> availableDiscs = new List<Discs>();
            if (aiPlayer.OrdinaryDiscs > 0) availableDiscs.Add(Discs.Ordinary);
            if (aiPlayer.BoringDiscs > 0) availableDiscs.Add(Discs.Boring);
            if (aiPlayer.ExplodingDiscs > 0) availableDiscs.Add(Discs.Exploding);

            if (availableDiscs.Count == 0)
                return (-1, Discs.Ordinary); // No discs available

            Discs chosenDisc = availableDiscs[_random.Next(availableDiscs.Count)];

            // 2. Check if AI can win in any column
            for (int col = 0; col < board.Cols; col++)
            {
                if (!board.IsColumnAvailable(col)) continue;

                var testGrid = board.Duplicate();
                testGrid.PlaceDisc(col, aiPlayer.Disc, Discs.Ordinary); // Only ordinary discs matter for win check
                if (testGrid.HasWinner(aiPlayer.Disc, discsToWin))
                    return (col, chosenDisc);
            }

            // 3. Pick a random valid column if no winning move
            List<int> validColumns = new List<int>();
            for (int col = 0; col < board.Cols; col++)
                if (board.IsColumnAvailable(col))
                    validColumns.Add(col);

            int selectedColumn = validColumns.Count > 0 ? validColumns[_random.Next(validColumns.Count)] : -1;
            return (selectedColumn, chosenDisc);
        }
    }
}
