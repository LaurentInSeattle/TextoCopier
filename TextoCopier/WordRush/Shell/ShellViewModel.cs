namespace Lyt.WordRush.Shell;

using static Lyt.WordRush.Messaging.ViewActivationMessage;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;
    private readonly LocalizerModel localizer;

    public ShellViewModel(
        LocalizerModel localizer, 
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;

        // this.templatesModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
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

        // Create all statics views and bind them 
        ShellViewModel.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        this.Logger.Debug("OnViewLoaded BindGroupIcons complete");

        this.OnViewActivation(ActivatedView.Setup, parameter: null, isFirstActivation: true);
        this.Logger.Debug("OnViewLoaded OnViewActivation complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        //if (this.templatesModel.Groups.Count > 0)
        //{
            this.toaster.Show(
                this.localizer.Lookup("Shell.Ready"), this.localizer.Lookup("Shell.Greetings"), 
                5_000, InformationLevel.Info);
        //}
        //else
        //{
        //    this.toaster.Show(
        //        this.localizer.Lookup("Shell.NoGroups.Title"), this.localizer.Lookup("Shell.NoGroups.Hint"), 
        //        10_000, InformationLevel.Warning);
        //}

        //this.Logger.Debug("OnViewLoaded SetupAvailableIcons begins");
        //this.Logger.Debug("OnViewLoaded SetupAvailableIcons complete");
        this.Logger.Debug("OnViewLoaded complete");

        Schedule.OnUiThread(
            5000, () => { this.Messenger.Publish(ActivatedView.Game); }, DispatcherPriority.Background);
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

            case ActivatedView.Game:
                var parameters = 
                    new GameViewModel.Parameters { Difficulty = GameViewModel.GameDifficulty.Medium };
                this.Activate<GameViewModel, GameView>(isFirstActivation, parameters);
                break;
        }
    }

    //private void OnSettings(object? _) => this.OnViewActivation(ActivatedView.Settings);

    //private void OnAbout(object? _) => this.OnViewActivation(ActivatedView.Help);

    //private void OnNewGroup(object? _) => this.OnViewActivation(ActivatedView.NewGroup);

    //private void OnEditGroup(object? _) => this.OnViewActivation(ActivatedView.EditGroup);

    private void OnExit(object? _) { }

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
        CreateAndBind<GameViewModel, GameView>();
    }

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AboutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NewGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand EditGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
