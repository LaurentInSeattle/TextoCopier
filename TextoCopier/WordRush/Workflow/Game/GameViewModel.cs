namespace Lyt.WordRush.Workflow.Game;

public sealed class GameViewModel : Bindable<GameView>
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
        "Merda!",  "Cazzo!" , "Cazzata!" , "Accidenti!", "Maledizione" , 
        "Errore" , "Sbaglio" , "Mancanza", "Fallo", "Pecca", 
        "Deficiente!" , "Stupido!" , "Cretino!" , "Scemo" ,  "Ottuso...", 
        "Terribile"
    ];

    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IRandomizer randomizer;
    private readonly LocalizerModel localizer;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;

    public GameViewModel(
        LocalizerModel localizer, IDialogService dialogService, IToaster toaster, IRandomizer randomizer)
    {
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.randomizer = randomizer;
        this.beNice = new Chooser<string>(this.randomizer, GameViewModel.beingNice);
        this.beMean = new Chooser<string>(this.randomizer, GameViewModel.beingMean);
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("GameViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.Logger.Debug("GameViewModel: OnViewLoaded complete");
    }
}
