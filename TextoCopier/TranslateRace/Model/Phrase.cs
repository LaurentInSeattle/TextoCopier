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

        new() {
            Id= 21 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Science,
            Italian = "Hai mai sentito parlare del gatto di Schrödinger?",
            English = "Have you ever heard about Schrodinger's cat?" ,
        },
        new() {
            Id= 22 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Science,
            Italian = "Come può il gatto di Schrödinger essere vivo e morto allo stesso tempo? Non ci posso credere!",
            English = "How can Schrodinger's cat be alive and dead at the same time? I can't believe it!" ,
        },
        new() {
            Id= 23 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Science,
            Italian = "Il paradosso del gatto di Schrödinger è molto difficile da spiegare alla gente comune.",
            English = "The Schrodinger's cat paradox is very difficult to explain to regular people." ,
        },
        new() {
            Id= 24 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Science,
            Italian = "La meccanica quantistica è molto difficile da comprendere in modo approfondito, anche per i laureati universitari.",
            English = "Quantum mechanics is very hard to understand in depth, even for university graduates." ,
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
