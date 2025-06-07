namespace Lyt.TranslateRace.Messaging;

public sealed record class ScoringCompleteMessage(int ScoreUpdate);

public sealed record class ScoreUpdateMessage(int Score);
