using Assignment;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Let's LineUp!!!");
        Console.WriteLine("=*=*=*=*=*=*=*=*=\n");
        // Ask user if they want to load a saved game
        bool loadGame = AskYesNo("Would you like to play a saved game? Enter 'y' or 'n': ");
        if (loadGame)
        {
            var savedGame = GamePlay.Load("game.json");
            if (savedGame != null)
            {
                savedGame.Start();
                return;
            }
            else
            {
                Console.WriteLine("There was no game saved! Play a new one...");
            }
        }

        // Choose game mode
        int rows = 6, cols = 7;
        string modeChoice = SelectGameMode();

        if (modeChoice == "3")
        {
            // Testing mode with input sequence
            var testGame = new GamePlay(rows, cols, false);
            Console.WriteLine("Enter the testing sequence");
            string? sequence = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(sequence))
            {
                testGame.PlayTestSequence(sequence);
            }
            return;
        }

        // Normal game: ask for rows and columns
        rows = GetNumber("Enter grid rows: ", min: 6);
        cols = GetNumber("Enter grid columns: ", min: Math.Max(7, rows));

        bool vsAI = modeChoice == "2";
        var game = new GamePlay(rows, cols, vsAI);

        // Show initial disc counts
        int totalDiscs = (int)Math.Floor(rows * cols / 2.0);
        int specialDiscs = 2;
        int ordinaryDiscs = totalDiscs - (specialDiscs * 2);

        Console.WriteLine("\nPlayers have the below discs:");
        Console.WriteLine($"P1: Ordinary {ordinaryDiscs}, Boring {specialDiscs}, Exploding {specialDiscs}");
        Console.WriteLine(vsAI
            ? $"P2: Ordinary {ordinaryDiscs}, Boring {specialDiscs}, Exploding {specialDiscs}"
            : $"P2: Ordinary {ordinaryDiscs}, Boring {specialDiscs}, Exploding {specialDiscs}");

        Console.WriteLine("\nPress Enter key to start the game");
        Console.ReadLine();

        game.Start();
    }

    private static bool AskYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? "").Trim().ToLower();
            if (input == "y") return true;
            if (input == "n") return false;
            Console.WriteLine("Enter a valid input");
        }
    }

    private static string SelectGameMode()
    {
        while (true)
        {
            Console.WriteLine("\nWhich mode would you prefer for LineUp:");
            Console.WriteLine("1. Human vs Human");
            Console.WriteLine("2. Human vs Computer");
            Console.WriteLine("3. Testing Mode");
            string? choice = Console.ReadLine()?.Trim();
            if (choice == "1" || choice == "2" || choice == "3") return choice;
            Console.WriteLine("Enter a valid input. Please enter 1,2 or 3.");
        }
    }

    private static int GetNumber(string prompt, int min)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min)
                return value;
            Console.WriteLine("Enter a valid input");
        }
    }
}
