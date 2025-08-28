namespace Lyt.Invasion.Workflow.GameOver;

using static ViewActivationMessage;
using static AppMessagingExtensions; 

public sealed partial class GameOverViewModel : ViewModel<GameOverView>
{
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnExit() => Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnPlay() => Publish(ActivatedView.Setup);

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression
}
