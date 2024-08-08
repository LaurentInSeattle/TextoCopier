namespace Lyt.WordRush.Workflow.Setup;

public sealed class SetupViewModel : Bindable<SetupView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;

    public SetupViewModel(
        LocalizerModel localizer,
        IDialogService dialogService, IToaster toaster)
    {
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("SetupViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.Logger.Debug("SetupViewModel: OnViewLoaded complete");
    }
}
