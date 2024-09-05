namespace Lyt.TranslateRace.Workflow.Intro;

using static Lyt.TranslateRace.Messaging.ViewActivationMessage;

public sealed class IntroViewModel : Bindable<IntroView>
{
    protected override void OnViewLoaded()
    {
        this.Logger.Debug("IntroViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.Logger.Debug("IntroViewModel: OnViewLoaded complete");
    }

    private void Play(GameDifficulty difficulty)
        => this.Messenger.Publish( ActivatedView.Game, null );

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    private void OnNext(object? _) => this.Messenger.Publish(ActivatedView.Setup);

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    /*
    private void OnPlayEasy(object? _) => this.Play(GameDifficulty.Easy);

    private void OnPlayMedium(object? _)  => this.Play(GameDifficulty.Medium);

    private void OnPlayHard(object? _)  => this.Play(GameDifficulty.Hard );

    public ICommand PlayEasyCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand PlayMediumCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand PlayHardCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
    */

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
