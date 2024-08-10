namespace Lyt.WordRush.Messaging;

public sealed class WordClickMessage(string word, Language language)
{
    public string Word { get; set; } = word;

    public Language Language { get; set; } = language;
}
