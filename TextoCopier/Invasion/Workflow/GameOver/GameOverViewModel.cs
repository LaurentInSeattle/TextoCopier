namespace Lyt.Invasion.Workflow.GameOver;

using static ViewActivationMessage;

public sealed partial class GameOverViewModel : ViewModel<GameOverView>
{
    [RelayCommand]
    public void OnExit() => this.Messenger.Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnPlay() => this.Messenger.Publish(ActivatedView.Setup);
}
