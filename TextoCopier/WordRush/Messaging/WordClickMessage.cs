namespace Lyt.WordRush.Messaging;

public sealed record class WordClickMessage(
    WordBlockViewModel WordBlockViewModel, string Word, Language Language); 