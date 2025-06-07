namespace Lyt.TranslateRace.Workflow.Intro;

using static ViewActivationMessage;

public sealed partial class IntroViewModel : ViewModel<IntroView>
{
    [RelayCommand]
    public void OnExit() => this.Messenger.Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnNext() => this.Messenger.Publish(ActivatedView.Setup);
}
