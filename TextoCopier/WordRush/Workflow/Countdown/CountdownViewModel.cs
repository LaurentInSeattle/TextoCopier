using static Lyt.WordRush.Workflow.Game.GameViewModel;

namespace Lyt.WordRush.Workflow.Countdown;

public sealed class CountdownViewModel : Bindable<CountdownView>
{
    private Parameters? parameters;

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
        if (activationParameters is not Parameters parameters)
        {
            throw new ArgumentException("Invalid activation parameters.");
        }

        this.Profiler.FullGcCollect();
        this.parameters = parameters;
        this.Comment = string.Empty;
        this.StartCountdown();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.Profiler.FullGcCollect();
    }

    private void StartCountdown()
    {
        // pronti, partenza, via 
        Schedule.OnUiThread(200,
            () =>
            {
                this.Comment = "Pronti ?";
                this.CommentColor = ColorTheme.ValidUiText;
            }, DispatcherPriority.Normal);

        Schedule.OnUiThread(1_400,
            () =>
            {
                this.Comment = "Partenza...";
                this.CommentColor = ColorTheme.BoxPresent;
            }, DispatcherPriority.Normal);

        Schedule.OnUiThread(2_600,
            () =>
            {
                this.Comment = "Vai!!!";
                this.CommentColor = ColorTheme.UiText;
            }, DispatcherPriority.Normal);

        Schedule.OnUiThread(3_800,
            () =>
            {
                this.Messenger.Publish( ViewActivationMessage.ActivatedView.Game, this.parameters );
            }, DispatcherPriority.Normal);
    }

    public string Comment { get => this.Get<string>()!; set => this.Set(value); }

    public Brush CommentColor { get => this.Get<Brush>()!; set => this.Set(value); }

}
