namespace Lyt.TranslateRace.Messaging;

public sealed class ScoringCompleteMessage(int scoreUpdate)
{
    public int ScoreUpdate { get; private set; } = scoreUpdate;
}

public sealed class ScoreUpdateMessage(int score)
{
    public int Score { get; private set; } = score;
}
