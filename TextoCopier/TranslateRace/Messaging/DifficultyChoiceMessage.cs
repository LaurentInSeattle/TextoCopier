namespace Lyt.TranslateRace.Messaging;

public sealed class DifficultyChoiceMessage(PhraseDifficulty phraseDifficulty)
{
    public PhraseDifficulty PhraseDifficulty { get; private set; } = phraseDifficulty;
}
