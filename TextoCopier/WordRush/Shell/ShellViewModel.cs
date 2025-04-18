﻿namespace Lyt.WordRush.Shell;

using static Lyt.WordRush.Messaging.ViewActivationMessage;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;

    public ShellViewModel(
        IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;

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


    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);

        //if (message.PropertyName != nameof( < some model property > ))
        //{
        //}
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        if (activatedView == ActivatedView.Exit)
        {
            this.OnExit(null);
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

    private async void OnExit(object? _) 
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private void Activate<TViewModel, TControl>(bool isFirstActivation, object? activationParameters)
        where TViewModel : Bindable<TControl>
        where TControl : Control, new()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        object? currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && control.DataContext is Bindable currentViewModel)
        {
            currentViewModel.Deactivate();
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
        newViewModel.Activate(activationParameters);
        this.View.ShellViewContent.Content = newViewModel.View;

        if( ! isFirstActivation)
        {
            this.profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
        }
    }

    private static void SetupWorkflow()
    {
        static void CreateAndBind<TViewModel, TControl>()
             where TViewModel : Bindable<TControl>
             where TControl : Control, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
        }

        CreateAndBind<SetupViewModel, SetupView>();
        CreateAndBind<CountdownViewModel, CountdownView>();
        CreateAndBind<GameViewModel, GameView>();
        CreateAndBind<GameOverViewModel, GameOverView>();
    }
}
