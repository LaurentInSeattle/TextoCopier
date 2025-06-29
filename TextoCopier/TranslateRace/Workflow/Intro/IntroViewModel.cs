namespace Lyt.TranslateRace.Workflow.Intro;

using static MessagingExtensions;

public sealed partial class IntroViewModel : ViewModel<IntroView>
{
    [RelayCommand]
    public void OnExit() => MessagingExtensions.Exit();

    [RelayCommand]
    public void OnNext() => Select(ActivatedView.Setup);
}
