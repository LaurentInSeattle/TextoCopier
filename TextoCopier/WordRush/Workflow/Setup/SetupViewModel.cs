namespace Lyt.WordRush.Workflow.Setup;

using static Lyt.WordRush.Messaging.ViewActivationMessage;

public sealed class SetupViewModel : Bindable<SetupView>
{
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

    private void Play(GameViewModel.GameDifficulty difficulty)
        => this.Messenger.Publish(
            ActivatedView.Countdown, new GameViewModel.Parameters { Difficulty = difficulty });

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    private void OnPlayEasy(object? _) => this.Play(GameViewModel.GameDifficulty.Easy);

    private void OnPlayMedium(object? _)  => this.Play(GameViewModel.GameDifficulty.Medium);

    private void OnPlayHard(object? _)  => this.Play(GameViewModel.GameDifficulty.Hard );

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public ICommand PlayEasyCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand PlayMediumCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand PlayHardCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
