namespace Lyt.WordRush.Model.History;

public sealed class GameEntry
{
    public GameEntry() { /* Req'ed for XML serialization */ }

    public GameEntry(DateTime started, TimeSpan duration, string word, int steps, bool isWon)
    {
        this.Started = started;
        this.Duration = duration;
        this.Word = word;
        this.Steps = steps;
        this.IsWon = isWon;
    }

    public DateTime Started { get; set; }

    public TimeSpan Duration { get; set; }

    public string Word { get; set; } = string.Empty;

    public int Steps { get; set; }

    public bool IsWon { get; set; }
}
