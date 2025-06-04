namespace Lyt.TranslateRace.Model;

public sealed class Phrase
{
    public static readonly string PhrasesFilename = "translaterace_phrases";

    public int Id { get; set; } = 0; 

    public PhraseDifficulty Difficulty { get; set; } = PhraseDifficulty.Easy;

    public PhraseTheme Theme { get; set; } = PhraseTheme.Music;

    public string English { get; set; } = string.Empty;

    public string Italian { get; set; } = string.Empty;

#pragma warning disable CA2211 // Non-constant fields should not be visible
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
            Italian = "Se non ti fidi della ricerca genomica, penso che tu sia un po' contro la scienza.",
            English = "If you do not trust genomic research, I think you are a bit against science." ,
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

        new() {
            Id= 31 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Life,
            Italian = "Vorrei andare in vacanza.",
            English = "I'd like to take a vacation." ,
        },
        new() {
            Id= 32 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Life,
            Italian = "Ho voglia di andarmene a Ibiza.",
            English = "I am in the mood to go to Ibiza." ,
        },
        new() {
            Id= 33 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Life,
            Italian = "Questo lavoro è troppo stressante, devo partire per una lunga vacanza.",
            English = "This job is too stressful, I must leave for a long vacation." ,
        },
        new() {
            Id= 34 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Life,
            Italian = "Questo lavoro stupido è davvero troppo stressante, non ne posso più!",
            English = "This stupid job is really too stressful, I can't take it anymore!" ,
        },


        new() {
            Id= 41 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Cities,
            Italian = "La città di Venizia è conosciuta come la Serenissima.",
            English = "The city of Venice is known as the Serenissima." ,
        },
        new() {
            Id= 42 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Cities,
            Italian = "Venezia è un comune italiano di circa duecentomila abitanti.",
            English = "Venice is an Italian municipality of about 250 000 habitants." ,
        },
        new() {
            Id= 43 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Cities,
            Italian = "Dal punto di vista geografico, il comune di Venezia è diviso in due parti: la zona insulare e la zona di terraferma.",
            English = "From a geographical point of view, the municipality of Venice is divided into two parts: the insular area and the mainland area." ,
        },
        new() {
            Id= 44 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Cities,
            Italian = "Con il termine di 'acqua alta' sono indicati nella laguna di Venezia picchi di marea.",
            English = "The term 'acqua alta' refers to tidal peaks in the Venice lagoon." ,
        },

        new() {
            Id= 45 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Cities,
            Italian = "Per la sua conformazione Venezia dispone di 435 ponti tra pubblici e privati che collegano le 118 isolette su cui è edificata, attraversando 176 canali.",
            English = "Due to its conformation, Venice has 435 bridges, both public and private, that connect the 118 islands on which it is built, crossing 176 canals." ,
        },
        new() {
            Id= 46 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Cities,
            Italian = "Venezia è un'importante sede universitaria italiana, infatti possiede più di un'università. La più celebre è l'Università Ca' Foscari.",
            English = "Venice is an important Italian university location, in fact it has more than one university. The most famous is the Ca' Foscari University." ,
        },

        new() {
            Id= 51 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Food,
            Italian = "Bernie! Ho molta fame! Due pizze per favore!",
            English = "Bernie! I am very hungry! Two pizzas please!" ,
        },
        new() {
            Id= 54 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Food,
            Italian = "Il cuoco Bernie è in sciopero. Niente pizze. Allora dove si va?",
            English = "Chef Bernie is on strike. No pizzas. So where do we go?" ,
        },
        new() {
            Id= 52 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Food,
            Italian = "La mia pizzeria preferita in Old Forge è chiusa il martedì. Peccato!",
            English = "My favorite pizza place in Old Forge is closed on Tuesdays. Too bad!" ,
        },
        new() {
            Id= 53 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Food,
            Italian = "Mettere l'ananas sulla pizza è un reato che dovrebbe essere punito severamente.",
            English = "Putting pineapple on pizza is a crime that should be severely punished." ,
        },


        new() {
            Id= 61 , Difficulty = PhraseDifficulty.Easy, Theme = PhraseTheme.Life,
            Italian = "Ciao! Mi chiamo Enzo. E tu?",
            English = "Hi! My name is Enzo. And you are?" ,
        },
        new() {
            Id= 62 , Difficulty = PhraseDifficulty.Medium, Theme = PhraseTheme.Life,
            Italian = "Lavoro e vivo in una piccola città nella periferia di San Francisco.",
            English = "I work and live in a small town on the outskirts of San Francisco." ,
        },
        new() {
            Id= 63 , Difficulty = PhraseDifficulty.Hard, Theme = PhraseTheme.Life,
            Italian = "Vivere in una piccola città è piacevole, ma a volte può anche risultare un po' noioso.",
            English = "Living in a small town is nice, but sometimes it can also be a bit boring." ,
        },
        new() {
            Id= 64 , Difficulty = PhraseDifficulty.Insane, Theme = PhraseTheme.Life,
            Italian = "Uno dei maggiori problemi di vivere in una piccola città americana è che hai bisogno di guidare molto, e questa è una vera seccatura.",
            English = "One of the biggest problems with living in a small American town is that you have to drive a lot, and that's a real hassle." ,
        },

    ];
#pragma warning restore CA2211 // Non-constant fields should not be visible
}
