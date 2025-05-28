namespace Lyt.WordRush.Workflow.Setup;

using static Lyt.WordRush.Messaging.ViewActivationMessage;

public sealed partial class SetupViewModel : ViewModel<SetupView>
{
    private void Play(GameDifficulty difficulty)
        => this.Messenger.Publish(
            ActivatedView.Countdown, new GameViewModel.Parameters { Difficulty = difficulty });

    [RelayCommand]
    public void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnPlayEasy(object? _) => this.Play(GameDifficulty.Easy);

    [RelayCommand]
    public void OnPlayMedium(object? _)  => this.Play(GameDifficulty.Medium);

    [RelayCommand]
    public void OnPlayHard(object? _)  => this.Play(GameDifficulty.Hard );
}
