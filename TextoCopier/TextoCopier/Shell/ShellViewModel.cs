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

    public ShellViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();
        this.toaster = ApplicationBase.GetRequiredService<IToaster>();
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
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
        this.OnViewActivation(StaticView.Group);

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        int groupCount = this.templatesModel.Groups.Count;
        if (groupCount > 0)
        {
            this.toaster.Show(
                localizer.Lookup("Shell.Ready"), localizer.Lookup("Shell.Greetings"), 4_000, InformationLevel.Info);
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

        if (message.PropertyName == nameof(this.templatesModel.SelectedGroup))
        {
            this.UpdateGroupIconsSelection();
        }
        else
        {
            this.BindGroupIcons();
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
        this.NewGroupIsVisible = staticView != StaticView.NewGroup;

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

    private void OnEditGroup(object? _) => this.OnViewActivation(StaticView.NewGroup);

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

    private void UpdateGroupIconsSelection()
    {
        //this.Logger.Debug("Update group selection");
        //var selectedGroup = this.templatesModel.SelectedGroup;
        //if (selectedGroup is null)
        //{
        //    return; 
        //}

        //var newSelection = 
        //    (from groupVm in this.Groups where groupVm.IconText == selectedGroup.Name select groupVm)
        //    .FirstOrDefault();
        //if ( newSelection is null)
        //{
        //    return;
        //}

        //newSelection.IsSelected = true; 
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
                GroupIconViewModel groupIconViewModel = new GroupIconViewModel(group, selectionGroup, selected);
                list.Add(groupIconViewModel);
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
