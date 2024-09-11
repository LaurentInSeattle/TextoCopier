namespace Lyt.TranslateRace.Workflow.Game;

public sealed class ScoreViewModel : Bindable<ScoreView>
{
    // TODO: Add more !
    private static readonly string[] beingNice =
    [
        "Che Bello!" , "Dunque!",  "Potente!" , "Strabiliante!" , "Stupefacente!",
        "Ben Fatto" , "Bene Bene.." , "Grande" , "Imponente", "Maestoso",
        "Eccezionale", "Stupendo!" , "Assordante" , "Bello..." , "Grandioso" ,
        "Spaventoso!" , "Forza!"
    ];

    // TODO: Add more !
    private static readonly string[] beingMean =
    [
        // "Deficiente!" , "Stupido!" , "Cretino!" ,  // Maybe too mean...
        "Merda!",  "Cazzo!" , "Cazzata!" , "Accidenti!", "Maledizione" ,
        "Maledetta!",
        "Errore" , "Che Sbaglio" , "Mancanza", "Fallo", "Pecca",
        "Scemo" ,  "Ottuso...",
        "Terribile"
    ];

    private readonly IRandomizer randomizer;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;

    public ScoreViewModel(IRandomizer randomizer)
    {
        this.randomizer = randomizer;
        this.beNice = new Chooser<string>(this.randomizer, ScoreViewModel.beingNice);
        this.beMean = new Chooser<string>(this.randomizer, ScoreViewModel.beingMean);
        this.Visible = true;
    }

    public void Show()
    {
        this.Visible = true;
    }

    private void PopMessage(string text, Brush brush)
    {
        // TODO: Fade in and out
        this.Comment = text;
        this.CommentColor = brush;

        Schedule.OnUiThread(
            1500,
            () =>
            {
                this.Comment = string.Empty;
            }, DispatcherPriority.Background);
    }


    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }

    public string Comment { get => this.Get<string>()!; set => this.Set(value); }

    public Brush CommentColor { get => this.Get<Brush>()!; set => this.Set(value); }

}
