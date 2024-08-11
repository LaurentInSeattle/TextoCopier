namespace Lyt.WordRush.Messaging;

public sealed class WordClickMessage(WordBlockViewModel wordBlockViewModel, string word, Language language)
{
    public WordBlockViewModel WordBlockViewModel { get; set; } = wordBlockViewModel;

    public string Word { get; set; } = word;

    public Language Language { get; set; } = language;
}
