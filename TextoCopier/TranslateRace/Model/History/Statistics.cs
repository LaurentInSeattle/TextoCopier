namespace Lyt.TranslateRace.Model.History;

public sealed class Statistics
{
    public TimeSpan Duration { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int WinRate { get; set; }

    public int CurrentStreak { get; set; }

    public int BestStreak { get; set; }

    public int MatchedWordsCount { get; set; }

    public int MissedWordsCount { get; set; }

    public int ClicksCount { get; set; }
}
