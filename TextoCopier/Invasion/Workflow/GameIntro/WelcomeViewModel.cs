namespace Lyt.Invasion.Workflow.GameIntro;

using static ViewActivationMessage; 

public sealed partial class WelcomeViewModel : ViewModel<WelcomeView>
{
    [RelayCommand]
    public void OnExit() => this.Messenger.Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnPlay() => this.Messenger.Publish(ActivatedView.Setup);
}