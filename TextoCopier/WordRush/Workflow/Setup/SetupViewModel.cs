namespace Lyt.WordRush.Workflow.Setup;

using static ViewActivationMessage;

public sealed partial class SetupViewModel : ViewModel<SetupView>
{
#pragma warning disable IDE0079
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnExit(object? _) =>
        new ViewActivationMessage(ActivatedView.Exit).Publish();

    [RelayCommand]
    public void OnPlayEasy(object? _) => this.Play(GameDifficulty.Easy);

    [RelayCommand]
    public void OnPlayMedium(object? _) => this.Play(GameDifficulty.Medium);

    [RelayCommand]
    public void OnPlayHard(object? _) => this.Play(GameDifficulty.Hard);

    private void Play(GameDifficulty difficulty)
        => new ViewActivationMessage(
                ActivatedView.Countdown,
                new GameViewModel.Parameters { Difficulty = difficulty })
            .Publish();

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079
}
