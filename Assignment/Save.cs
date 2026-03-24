namespace Assignment;

public class Save
{
    public int Rows { get; set; }
    public int Cols { get; set; }
    public char[][] Grid { get; set; } = Array.Empty<char[]>();
    public int NeededToWin { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public bool VsAI { get; set; }

    public int P1BoringLeft { get; set; }
    public int P1ExplodingLeft { get; set; }
    public int P1OrdinaryLeft { get; set; }

    public int P2BoringLeft { get; set; }
    public int P2ExplodingLeft { get; set; }
    public int P2OrdinaryLeft { get; set; }
}
