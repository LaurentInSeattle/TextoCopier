namespace Lyt.TranslateRace.Model.History;

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard,
    Insane,
}

public sealed class GameResult 
{
    public GameResult() { /* Required for serialization */ }

    public GameDifficulty Difficulty { get; set; }

    public bool IsWon { get; set; }

    public TimeSpan GameDuration { get; set; }
}
