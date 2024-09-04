namespace Lyt.TranslateRace.Model;

public enum PhraseDifficulty
{
    Easy,
    Medium,
    Hard,
    Insane,
}

public enum PhraseTheme
{
    Nature,
    Science,
    Music,
    Culture,
    Sports,
    
    Geography, 
    Cities, 
    Work, 
    Leisure, 
    Food,

    News, 
    History,
}


public sealed class Phrase
{
    public int Id { get; set; } = 0; 

    public PhraseDifficulty Difficulty { get; set; } = PhraseDifficulty.Easy;

    public PhraseTheme Theme { get; set; } = PhraseTheme.Music;

    public string English { get; set; } = string.Empty;

    public string Italian { get; set; } = string.Empty;
}
