namespace Lyt.WordRush.Shell;

using static Lyt.WordRush.Messaging.ViewActivationMessage;

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private readonly IToaster toaster;

    public ShellViewModel(IToaster toaster)
    {
        this.toaster = toaster;
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

        this.Logger.Debug("OnViewLoaded language loaded");

        // Create all statics views and bind them 
        ShellViewModel.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        this.Logger.Debug("OnViewLoaded BindGroupIcons complete");

        this.OnViewActivation(ActivatedView.Setup, parameter: null, isFirstActivation: true);
        this.Logger.Debug("OnViewLoaded OnViewActivation complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
            this.toaster.Show(
                "Benvenuto/a!", 
                "Benvenuto/a a 'Parole in Fretta! Sei pronto/a per una sfida?", 
                3_000, InformationLevel.Info);
        this.Logger.Debug("OnViewLoaded complete");
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        if (activatedView == ActivatedView.Exit)
        {
            OnExit(null);
        }

        if (activatedView == ActivatedView.GoBack)
        {
            // We always go back to the Setup View 
            activatedView = ActivatedView.Setup;
        }

        switch (activatedView)
        {
            default:
            case ActivatedView.Setup:
                this.Activate<SetupViewModel, SetupView>(isFirstActivation, null);
                break;

            case ActivatedView.Countdown:
                if (parameter is GameViewModel.Parameters parametersSetup)
                {
                    this.Activate<CountdownViewModel, CountdownView>(isFirstActivation, parametersSetup);
                }
                else
                {
                    throw new Exception("No game parameters");
                }
                break;

            case ActivatedView.Game:
                if (parameter is GameViewModel.Parameters parametersGame)
                {
                    this.Activate<GameViewModel, GameView>(isFirstActivation, parametersGame);
                } 
                else
                {
                    throw new Exception("No game parameters");
                }
                break;

            case ActivatedView.GameOver:
                if (parameter is GameResult gameResults)
                {
                    this.Activate<GameOverViewModel, GameOverView>(isFirstActivation, gameResults);
                }
                else
                {
                    throw new Exception("No game results");
                }
                break;
        }
    }

    private static async void OnExit(object? _) 
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private void Activate<TViewModel, TControl>(bool isFirstActivation, object? activationParameters)
        where TViewModel : ViewModel<TControl>
        where TControl : Control, IView, new()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        object? currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && control.DataContext is ViewModel currentViewModel)
        {
            currentViewModel.Deactivate();
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
        newViewModel.Activate(activationParameters);
        this.View.ShellViewContent.Content = newViewModel.View;

        if( ! isFirstActivation)
        {
            this.Profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
        }
    }

    private static void SetupWorkflow()
    {
        App.GetRequiredService<SetupViewModel>().CreateViewAndBind();
        App.GetRequiredService<CountdownViewModel>().CreateViewAndBind();
        App.GetRequiredService<GameViewModel>().CreateViewAndBind();
        App.GetRequiredService<GameOverViewModel>().CreateViewAndBind();
    }
}
