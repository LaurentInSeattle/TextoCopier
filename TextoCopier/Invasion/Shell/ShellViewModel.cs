namespace Lyt.Invasion.Shell;

using static ViewActivationMessage;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

    public ShellViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;

        this.invasionModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
        this.messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Select default language 
        //this.localizer.DetectAvailableLanguages();
        //string preferredLanguage = this.templatesModel.Language;
        //this.Logger.Debug("Language: " + preferredLanguage);
        //this.localizer.SelectLanguage(preferredLanguage);

        this.Logger.Debug("OnViewLoaded language loaded");

        ShellViewModel.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        this.OnViewActivation(ActivatedView.Welcome, parameter: null, isFirstActivation: true);
        this.Logger.Debug("OnViewLoaded OnViewActivation complete");

        // Schedule.OnUiThread(500, this.UpdateUi, DispatcherPriority.Background);
        this.Logger.Debug("OnViewLoaded complete");
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    private async void OnExit()
    {
        void Deactivate(Type type)
        {
            object? vm = App.GetRequiredService(type);
            if (vm is Bindable bindable)
            {
                bindable.Deactivate();
            }
        }

        Type[] types = [typeof(WelcomeViewModel), typeof(SetupViewModel), typeof(GameViewModel), typeof(GameOverViewModel)];

        foreach (var type in types)
        {
            Deactivate(type);
        }

        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private static void SetupWorkflow()
    {
        static void CreateAndBind<TViewModel, TControl>()
             where TViewModel : Bindable<TControl>
             where TControl : Control, new()
            => App.GetRequiredService<TViewModel>().CreateViewAndBind();

        CreateAndBind<WelcomeViewModel, WelcomeView>();
        CreateAndBind<SetupViewModel, SetupView>();
        CreateAndBind<GameViewModel, GameView>();
        CreateAndBind<GameOverViewModel, GameOverView>();
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        if (activatedView == ActivatedView.Exit)
        {
            this.OnExit();
            return;
        }

        if (activatedView == ActivatedView.GoBack)
        {
            // Nothing for now
            return;
        }

        switch (activatedView)
        {
            default:
            case ActivatedView.Welcome:
                this.Activate<WelcomeViewModel, WelcomeView>(isFirstActivation, parameter);
                break;

            case ActivatedView.Setup:
                this.Activate<SetupViewModel, SetupView>(isFirstActivation, parameter);
                break;

            case ActivatedView.Game:
                this.Activate<GameViewModel, GameView>(isFirstActivation, parameter);
                break;

            case ActivatedView.GameOver:
                this.Activate<GameOverViewModel, GameOverView>(isFirstActivation, parameter);
                break;
        }
    }

    private void Activate<TViewModel, TControl>(bool isFirstActivation, object? activationParameters)
        where TViewModel : Bindable<TControl>
        where TControl : Control, new()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        if (this.dialogService.IsModal)
        {
            this.dialogService.Dismiss();
        }

        object? currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && control.DataContext is Bindable currentViewModel)
        {
            currentViewModel.Deactivate();
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
        newViewModel.Activate(activationParameters);
        this.View.ShellViewContent.Content = newViewModel.View;

        if (!isFirstActivation)
        {
            this.profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
        }
    }
}
