namespace Assignment
{
    public class Player
    {
        public string Name { get; }
        public char Disc { get; }
        public bool IsAI { get; }

        public int OrdinaryDiscs { get; private set; }
        public int BoringDiscs { get; private set; }
        public int ExplodingDiscs { get; private set; }

        // Initialize player with disc counts and AI flag
        public Player(string name, char disc, int totalDiscs, bool isAI = false)
        {
            Name = name;
            Disc = disc;
            BoringDiscs = 2;
            ExplodingDiscs = 2;
            OrdinaryDiscs = totalDiscs - (BoringDiscs + ExplodingDiscs);
            IsAI = isAI;
        }

        // Use a disc of specified type
        public void ConsumeDisc(Discs type)
        {
            switch (type)
            {
                case Discs.Ordinary:
                    OrdinaryDiscs--;
                    break;
                case Discs.Boring:
                    BoringDiscs--;
                    break;
                case Discs.Exploding:
                    ExplodingDiscs--;
                    break;
            }
        }

        // Return a disc to the player's available pool
        public void ReclaimDisc(Discs type)
        {
            switch (type)
            {
                case Discs.Ordinary:
                    OrdinaryDiscs++;
                    break;
                case Discs.Boring:
                    BoringDiscs++;
                    break;
                case Discs.Exploding:
                    ExplodingDiscs++;
                    break;
            }
        }

        // Reset all disc counts to specified values
        public void ResetDiscs(int ordinary, int boring, int exploding)
        {
            OrdinaryDiscs = ordinary;
            BoringDiscs = boring;
            ExplodingDiscs = exploding;
        }

        // Display current disc counts (for console output)
        public void ShowDiscStatus()
        {
            string playerLabel = IsAI ? "P2" : Name;
            Console.WriteLine($"\n--- {playerLabel}'s Discs ---");
            Console.WriteLine($"Ordinary: {OrdinaryDiscs}, Boring:   {BoringDiscs}, Exploding:{ExplodingDiscs}");
        }
    }
}
