namespace Lyt.TextoCopier.Shell;

using static ViewActivationMessage;

public sealed class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel() : this(ApplicationBase.GetRequiredService<TemplatesModel>()) { }

    private readonly TemplatesModel templatesModel;
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;

    public ShellViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();
        this.toaster = ApplicationBase.GetRequiredService<IToaster>();
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();

        this.Groups = [];
        this.SettingsCommand = new Command(this.OnSettings);
        this.AboutCommand = new Command(this.OnAbout);
        this.ExitCommand = new Command(this.OnExit);
        this.NewGroupCommand = new Command(this.OnNewGroup);
        this.DeleteGroupCommand = new Command(this.OnDeleteGroup);
        this.messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Select default language 
        var localizer = App.GetRequiredService<LocalizerModel>();
        localizer.DetectAvailableLanguages();
        localizer.SelectLanguage("it-IT");

        // Create all statics views and bind them 
        ShellViewModel.SetupWorkflow();
        this.BindGroupIcons();
        this.OnViewActivation(StaticView.Group);

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        int groupCount = this.templatesModel.Groups.Count;
        if (groupCount > 0)
        {
            this.toaster.Show(
                localizer.Lookup("Shell.Ready"), localizer.Lookup("Shell.Greetings"), 4_000, ToastLevel.Info);
        }
        else
        {
            this.toaster.Show(
                "No groups defined", // localizer.Lookup("Shell.Ready"), 
                "Create a new group to get started...", // localizer.Lookup("Shell.Greetings"), 
                10_000, ToastLevel.Warning);
        }
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View);

    private void OnViewActivation(StaticView staticView)
    {
        if (staticView == StaticView.GoBack)
        {
            staticView = StaticView.Group;
        }

        this.DeleteGroupIsVisible = staticView == StaticView.Group;

        switch (staticView)
        {
            default:
            case StaticView.Group:
                this.Activate<GroupViewModel, GroupView>();
                break;

            case StaticView.NewGroup:
                this.Activate<NewGroupViewModel, NewGroupView>();
                break;

            case StaticView.Help:
                this.Activate<HelpViewModel, HelpView>();
                break;

            case StaticView.Settings:
                this.Activate<SettingsViewModel, SettingsView>();
                break;

            case StaticView.NewTemplate:
                this.Activate<NewTemplateViewModel, NewTemplateView>();
                break;
        }
    }

    private void OnSettings(object? _) => this.OnViewActivation(StaticView.Settings);

    private void OnAbout(object? _) => this.OnViewActivation(StaticView.Help);

    private void OnNewGroup(object? _) => this.OnViewActivation(StaticView.NewGroup);

    private void OnExit(object? _) { }

    private void OnDeleteGroup(object? _) { }

    private void Activate<TViewModel, TControl>()
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

        var currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && control.DataContext is Bindable currentViewModel)
        {
            currentViewModel.Deactivate();
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
        newViewModel.Activate();
        this.View.ShellViewContent.Content = newViewModel.View;
    }

    private void BindGroupIcons()
    {
        this.Logger.Info("Binding groups");

        SelectionGroup selectionGroup = this.View!.SelectionGroup;
        int groupCount = this.templatesModel.Groups.Count;
        if (groupCount > 0)
        {
            bool selected = true;
            var list = new List<GroupIconViewModel>();
            foreach (var group in this.templatesModel.Groups)
            {
                list.Add(new GroupIconViewModel(group, selectionGroup, selected));
                selected = false;
            }

            this.Groups = list;
            this.templatesModel.SelectGroup(this.templatesModel.Groups[0].Name, out string _);
            this.Logger.Info("Groups: " + this.templatesModel.Groups.Count);
        }
        else
        {
            // Notify: Done elsewhere
        }
    }

    private static void SetupWorkflow()
    {
        void CreateAndBind<TViewModel, TControl>()
            where TViewModel : Bindable<TControl>
            where TControl : Control, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
        }

        CreateAndBind<GroupViewModel, GroupView>();
        CreateAndBind<NewGroupViewModel, NewGroupView>();
        CreateAndBind<HelpViewModel, HelpView>();
        CreateAndBind<SettingsViewModel, SettingsView>();
        CreateAndBind<NewTemplateViewModel, NewTemplateView>();
    }

    public bool DeleteGroupIsVisible { get => this.Get<bool>(); set => this.Set(value); }

    public List<GroupIconViewModel> Groups { get => this.Get<List<GroupIconViewModel>>()!; set => this.Set(value); }

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AboutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NewGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
