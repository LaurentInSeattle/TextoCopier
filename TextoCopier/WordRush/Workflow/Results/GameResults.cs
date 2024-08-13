namespace Lyt.WordRush.Workflow.Results;

public sealed class GameResults (GameViewModel.Parameters parameters)
{
    public GameViewModel.Parameters Parameters { get; set; } = parameters;

    public List<string> Words { get; set; } = [];

    public TimeSpan GameDuration { get; set; }

    public int WordCount { get; set; }

    public int MatchedWordsCount { get; set; }

    public int MissedWordsCount { get; set; }

    public int ClicksCount { get; set; }
}
