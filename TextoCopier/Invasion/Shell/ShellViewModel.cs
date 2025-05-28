namespace Lyt.Invasion.Shell;

using Lyt.Mvvm;
using static ViewActivationMessage;

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private readonly IDialogService dialogService;
    private readonly InvasionModel invasionModel;

    public ShellViewModel(InvasionModel invasionModel, IDialogService dialogService)
    {
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;

        this.invasionModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
        this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
    }

    public override void OnViewLoaded()
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

    private static async void OnExit()
    {
        static void Deactivate(Type type)
        {
            object? vm = App.GetRequiredService(type);
            if (vm is ViewModel viewModel)
            {
                viewModel.Deactivate();
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
        App.GetRequiredService<WelcomeViewModel>().CreateViewAndBind();
        App.GetRequiredService<PlayerSetupViewModel>().CreateViewAndBind();
        App.GetRequiredService<SetupViewModel>().CreateViewAndBind();
        App.GetRequiredService<PlayerViewModel>().CreateViewAndBind();
        App.GetRequiredService<GameViewModel>().CreateViewAndBind();
        App.GetRequiredService<GameOverViewModel>().CreateViewAndBind();
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        if (activatedView == ActivatedView.Exit)
        {
            OnExit();
            return;
        }

        if (activatedView == ActivatedView.GoBack)
        {
            // Nothing for now
            this.Activate<SetupViewModel, SetupView>(isFirstActivation, parameter);
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

            case ActivatedView.PlayerSetup:
                this.Activate<PlayerSetupViewModel, PlayerSetupView>(isFirstActivation, parameter);
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
        where TViewModel : ViewModel<TControl>
        where TControl : Control, IView, new()
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
        if (currentView is Control control && control.DataContext is ViewModel currentViewModel)
        {
            currentViewModel.Deactivate();
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
        newViewModel.Activate(activationParameters);
        this.View.ShellViewContent.Content = newViewModel.View;

        if (!isFirstActivation)
        {
            this.Profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
        }
    }
}
