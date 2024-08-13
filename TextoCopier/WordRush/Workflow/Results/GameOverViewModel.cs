namespace Lyt.WordRush.Workflow.Results;

using static Lyt.WordRush.Messaging.ViewActivationMessage;

public sealed class GameOverViewModel : Bindable<GameOverView>
{
    private GameResults? results;

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

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not GameResults results)
        {
            throw new ArgumentException("Invalid activation parameters.");
        }

        this.Profiler.FullGcCollect();
        this.results = results;
        this.ShowResults();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.Profiler.FullGcCollect();
    }

    private void ShowResults()
    {
        this.GameOver = "Fine dei Giochi";
        this.GameOverColor = ColorTheme.UiText;

        // TODO 
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    private void OnPlayAgain(object? _) => this.Messenger.Publish(ActivatedView.Setup);

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public string GameOver { get => this.Get<string>()!; set => this.Set(value); }

    public Brush GameOverColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public ICommand PlayAgainCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
