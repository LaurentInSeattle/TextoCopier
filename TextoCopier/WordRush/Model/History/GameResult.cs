namespace Lyt.WordRush.Model.History;

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard,
}

public sealed class GameResult 
{
    public GameResult() { /* Required for serialization */ }

    public GameDifficulty Difficulty { get; set; }

    public bool IsWon { get; set; }

    public List<string> Words { get; set; } = [];

    public TimeSpan GameDuration { get; set; }

    public int WordCount { get; set; }

    public int MatchedWordsCount { get; set; }

    public int MissedWordsCount { get; set; }

    public int ClicksCount { get; set; }
}
