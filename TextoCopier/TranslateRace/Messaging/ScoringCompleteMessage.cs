namespace Lyt.TranslateRace.Messaging;

public sealed class ScoringCompleteMessage
{
}

public sealed class ScoreUpdateMessage(int score)
{
    public int Score { get; private set; } = score;
}
