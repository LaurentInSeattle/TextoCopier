namespace Lyt.WordRush.Workflow.Game;

public sealed class WordBlockViewModel : Bindable<GameView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;

    public WordBlockViewModel( LocalizerModel localizer, IDialogService dialogService, IToaster toaster)
    {
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("WordBlockViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.Logger.Debug("WordBlockViewModel: OnViewLoaded complete");
    }
}
