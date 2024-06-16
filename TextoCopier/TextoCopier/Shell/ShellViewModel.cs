namespace Lyt.TextoCopier.Shell;

using System.Xml.Linq;
using static ViewActivationMessage;

public sealed class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel() : this(ApplicationBase.GetRequiredService<TemplatesModel>()) { }

    private readonly TemplatesModel templatesModel;
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;

    public ShellViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();
        this.toaster = ApplicationBase.GetRequiredService<IToaster>();
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.profiler = ApplicationBase.GetRequiredService<IProfiler>();
        this.templatesModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);

        this.Groups = [];
        this.SettingsCommand = new Command(this.OnSettings);
        this.AboutCommand = new Command(this.OnAbout);
        this.ExitCommand = new Command(this.OnExit);
        this.NewGroupCommand = new Command(this.OnNewGroup);
        this.EditGroupCommand = new Command(this.OnEditGroup);
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
        this.OnViewActivation(ActivatedView.Group);

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        int groupCount = this.templatesModel.Groups.Count;
        if (groupCount > 0)
        {
            this.toaster.Show(
                localizer.Lookup("Shell.Ready"), localizer.Lookup("Shell.Greetings"), 5_000, InformationLevel.Info);
        }
        else
        {
            this.toaster.Show(
                "No groups defined", // localizer.Lookup("Shell.Ready"), 
                "Create a new group to get started...", // localizer.Lookup("Shell.Greetings"), 
                10_000, InformationLevel.Warning);
        }
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);

        if (message.PropertyName != nameof(this.templatesModel.SelectedGroup))
        {
            this.BindGroupIcons();
        }
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter);

    private void OnViewActivation(ActivatedView staticView, object? parameter = null)
    {
        if (staticView == ActivatedView.GoBack)
        {
            staticView = ActivatedView.Group;
        }

        this.DeleteGroupIsVisible = staticView == ActivatedView.Group;
        this.NewGroupIsVisible = staticView != ActivatedView.NewGroup;

        switch (staticView)
        {
            default:
            case ActivatedView.Group:
                this.Activate<GroupViewModel, GroupView>(null);
                break;

            case ActivatedView.NewGroup:
                this.Activate<NewEditGroupViewModel, NewEditGroupView>(null);
                break;

            case ActivatedView.EditGroup:
                this.Activate<NewEditGroupViewModel, NewEditGroupView>(this.templatesModel.SelectedGroup);
                break;

            case ActivatedView.Help:
                this.Activate<HelpViewModel, HelpView>(null);
                break;

            case ActivatedView.Settings:
                this.Activate<SettingsViewModel, SettingsView>(null);
                break;

            case ActivatedView.EditTemplate:
            case ActivatedView.NewTemplate:
                this.Activate<NewEditTemplateViewModel, NewEditTemplateView>(parameter);
                break;
        }
    }

    private void OnSettings(object? _) => this.OnViewActivation(ActivatedView.Settings);

    private void OnAbout(object? _) => this.OnViewActivation(ActivatedView.Help);

    private void OnNewGroup(object? _) => this.OnViewActivation(ActivatedView.NewGroup);

    private void OnEditGroup(object? _) => this.OnViewActivation(ActivatedView.EditGroup);

    private void OnExit(object? _) { }

    private void OnDeleteGroup(object? _)
    {
        var group = this.templatesModel.SelectedGroup;
        if (group is null)
        {
            return;
        }

        var confirmActionParameters = new ConfirmActionParameters
        {
            Title = "Delete Group ?",
            Message = "All entries in this group will be deleted. This operation cannot be undone.",
            ActionVerb = "Delete",
            OnConfirm = this.OnDeleteGroupConfirmed,
        };

        this.dialogService.Confirm(this.View.ToasterHost, confirmActionParameters );
    }

    private void OnDeleteGroupConfirmed(bool confirmed)
    {
        if ( ! confirmed)
        {
            return;
        }

        var group = this.templatesModel.SelectedGroup;
        if (group is null)
        {
            return;
        }

        if (this.templatesModel.CheckGroup(group.Name, out string message))
        {
            this.templatesModel.DeleteGroup(group.Name, out message);
        }

        if (string.IsNullOrEmpty(message))
        {
            this.toaster.Show(
                "Deleted", // localizer.Lookup("Shell.Ready"), 
                "Group has been deleted", // localizer.Lookup("Shell.Greetings"), 
                5_000, InformationLevel.Info);
        }
        else
        {
            this.toaster.Show(
                "Error", // localizer.Lookup("Shell.Ready"), 
                "Failed to delete group", // localizer.Lookup("Shell.Greetings"), 
                12_000, InformationLevel.Error);
        }

        this.BindGroupIcons(); 
    }

    private void Activate<TViewModel, TControl>(object? activationParameters)
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

        this.profiler.MemorySnapshot(newViewModel.View.GetType().Name + " Activated");
    }

    private void BindGroupIcons()
    {
        //Debugger.Break(); 

        this.Logger.Info("Binding groups");
        var selectedGroup = this.templatesModel.SelectedGroup; 
        if ( selectedGroup is null )
        {
            selectedGroup = this.templatesModel.Groups[0];
            this.templatesModel.SelectGroup(selectedGroup.Name, out string _);
        }

        SelectionGroup selectionGroup = this.View.SelectionGroup;
        int groupCount = this.templatesModel.Groups.Count;
        if (groupCount > 0)
        {
            var list = new List<GroupIconViewModel>();
            foreach (var group in this.templatesModel.Groups)
            {
                bool selected = selectedGroup == group;
                list.Add(new GroupIconViewModel(group, selectionGroup, selected));
            }

            this.Groups = list;
            this.Logger.Info("Groups: " + this.templatesModel.Groups.Count);
        }
        else
        {
            // Notify: Done elsewhere
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

        CreateAndBind<GroupViewModel, GroupView>();
        CreateAndBind<NewEditGroupViewModel, NewEditGroupView>();
        CreateAndBind<HelpViewModel, HelpView>();
        CreateAndBind<SettingsViewModel, SettingsView>();
        CreateAndBind<NewEditTemplateViewModel, NewEditTemplateView>();
    }

    public bool GroupsIsVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool DeleteGroupIsVisible 
    { 
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.GroupsIsVisible = this.DeleteGroupIsVisible || this.NewGroupIsVisible; 
        }
    }

    public bool NewGroupIsVisible 
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.GroupsIsVisible = this.DeleteGroupIsVisible || this.NewGroupIsVisible;
        }
    }

    public List<GroupIconViewModel> Groups { get => this.Get<List<GroupIconViewModel>>()!; set => this.Set(value); }

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AboutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NewGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand EditGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
