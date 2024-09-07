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
    Love, 

}


public sealed class Phrase
{
    public static readonly string PhrasesFilename = "translaterace_phrases";

    public int Id { get; set; } = 0; 

    public PhraseDifficulty Difficulty { get; set; } = PhraseDifficulty.Easy;

    public PhraseTheme Theme { get; set; } = PhraseTheme.Music;

    public string English { get; set; } = string.Empty;

    public string Italian { get; set; } = string.Empty;

    public static List<Phrase> DefaultPhrases =
    [
        new() { 
            Id= 1 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Nature,
            Italian = "Mi piacciono gli uccelli e mi piace guardarli.",
            English = "I like birds and I like watching them." , 
        },
        new() {
            Id= 2 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Nature,
            Italian = "I dinosauri sono gli antenati degli uccelli.",
            English = "Dinosaurs are the ancestors of birds." ,
        },
        new() {
            Id= 3 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Nature,
            Italian = "Secondo la ricerca genomica i dinosauri sono davvero gli antenati degli uccelli.",
            English = "According to genomic research, dinosaurs are truly the ancestors of birds." ,
        },
        new() {
            Id= 4 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Nature,
            Italian = "Se non ti fidi della ricerca genomica, penso che tu sia un po' stupido.",
            English = "If you do not trust genomic research, I think you are a bit stupid." ,
        },

        new() {
            Id= 11 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Love,
            Italian = "Pronto? Sono io, Annalisa.",
            English = "Hello? It's me, Annalisa." ,
        },
        new() {
            Id= 12 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Love,
            Italian = "Pronto? Sono io, Annalisa. Ti ricordi di me?",
            English = "Hello? It's me, Annalisa. Do you remember me?" ,
        },
        new() {
            Id= 13 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Love,
            Italian = "Pronto? Sono io, Annalisa. Ti sono mancata?",
            English = "Hello? It's me, Annalisa. Did you miss me?" ,
        },
        new() {
            Id= 14 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Love,
            Italian = "Pronto? Sono io, Annalisa. Ti sono mancata? Mi sei mancato tanto, piccolo.",
            English = "Hello? It's me, Annalisa. Did you miss me? I missed you so much, baby..." ,
        },

        //new() {
        //    Id= 1 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Nature,
        //    Italian = ".",
        //    English = "." ,
        //},
        //new() {
        //    Id= 2 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Nature,
        //    Italian = ".",
        //    English = "." ,
        //},
        //new() {
        //    Id= 3 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Nature,
        //    Italian = ".",
        //    English = "." ,
        //},
        //new() {
        //    Id= 4 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Nature,
        //    Italian = ".",
        //    English = "." ,
        //},
    ];
}
