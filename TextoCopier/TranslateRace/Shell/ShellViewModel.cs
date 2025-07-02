namespace Lyt.TranslateRace.Shell;

using static MessagingExtensions; 

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private ViewSelector<ActivatedView>? viewSelector;
    public bool isFirstActivation;

    public override void OnViewLoaded()
    {
        this.Logger.Debug("OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Create all statics views and bind them 
        this.SetupWorkflow();
        Select(ActivatedView.Intro);

        // Ready 
        this.Logger.Debug("OnViewLoaded complete");
    }

    public static async void OnExit(object? _)
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private void SetupWorkflow()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        var selectableViews = new List<SelectableView<ActivatedView>>();

        void SetupNoToolbar<TViewModel, TControl>(
                ActivatedView activatedView, Control? control = null)
            where TViewModel : ViewModel<TControl>
            where TControl : Control, IView, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
            selectableViews.Add(
                new SelectableView<ActivatedView>(activatedView, vm, control, null));
        }

        SetupNoToolbar<IntroViewModel, IntroView>(ActivatedView.Intro);
        SetupNoToolbar<SetupViewModel, SetupView>(ActivatedView.Setup);
        SetupNoToolbar<NewParticipantViewModel, NewParticipantView>(ActivatedView.NewParticipant);
        SetupNoToolbar<GameViewModel, GameView>(ActivatedView.Game);
        SetupNoToolbar<GameOverViewModel, GameOverView>(ActivatedView.GameOver);

        // Needs to be kept alive as a class member, or else callbacks will die (and wont work) 
        this.viewSelector =
            new ViewSelector<ActivatedView>(
                this.Messenger,
                this.View.ShellViewContent,
                null, // no secondary container 
                null, // no selector to update
                selectableViews,
                null // no call back on view change
                );
    }
}
