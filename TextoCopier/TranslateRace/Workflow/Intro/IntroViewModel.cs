namespace Lyt.TranslateRace.Workflow.Intro;

using static AppMessagingExtensions;

public sealed partial class IntroViewModel : ViewModel<IntroView>
{
#pragma warning disable CA1822 // Mark members as static
    [RelayCommand]
    public void OnExit() => Exit();

    [RelayCommand]
    public void OnNext() => Select(ActivatedView.Setup);

#pragma warning restore CA1822 // Mark members as static
}
