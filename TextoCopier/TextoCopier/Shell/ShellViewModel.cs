namespace Lyt.TextoCopier.Shell;

using static ViewActivationMessage;

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private readonly TemplatesModel templatesModel;
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;

    [ObservableProperty]
    private List<GroupIconViewModel> groups;

    [ObservableProperty]
    private bool groupsIsVisible;

    [ObservableProperty]
    private bool deleteGroupIsVisible;

    [ObservableProperty]
    private bool newGroupIsVisible;

    public ShellViewModel(
        TemplatesModel templatesModel, IDialogService dialogService, IToaster toaster)
    {
        this.templatesModel = templatesModel;
        this.dialogService = dialogService;
        this.toaster = toaster;

        this.templatesModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
        this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);

        this.Groups = [];
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
        // this.Localizer.DetectAvailableLanguages();
        string preferredLanguage = this.templatesModel.Language;
        this.Logger.Debug("Language: " + preferredLanguage);
        this.Localizer.SelectLanguage(preferredLanguage);

        this.Logger.Debug("OnViewLoaded language loaded");

        // Create all statics views and bind them 
        ShellViewModel.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        this.BindGroupIcons();

        this.Logger.Debug("OnViewLoaded BindGroupIcons complete");

        this.OnViewActivation(ActivatedView.Group, parameter: null, isFirstActivation: true);
        this.Logger.Debug("OnViewLoaded OnViewActivation complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        if (this.templatesModel.Groups.Count > 0)
        {
            this.toaster.Show(
                this.Localizer.Lookup("Shell.Ready"), this.Localizer.Lookup("Shell.Greetings"), 
                5_000, InformationLevel.Info);
        }
        else
        {
            this.toaster.Show(
                this.Localizer.Lookup("Shell.NoGroups.Title"), this.Localizer.Lookup("Shell.NoGroups.Hint"), 
                10_000, InformationLevel.Warning);
        }

        this.Logger.Debug("OnViewLoaded SetupAvailableIcons begins");

        this.SetupAvailableIcons();

        this.Logger.Debug("OnViewLoaded SetupAvailableIcons complete");
        this.Logger.Debug("OnViewLoaded complete");
    }

    private void SetupAvailableIcons()
    {
        //_ = Task.Run(async () => 
        // {
            // Detect Available Icons in asset file and pass that to the model 
            List<string> icons = ShellViewModel.DetectAvailableIcons();
            this.Logger.Debug(icons.Count + " icons available.");
            this.templatesModel.AvailableIcons = icons;
            // await Task.Delay(250); 
            // this.profiler.MemorySnapshot("Shell View Loaded", withGCCollect: true);
        //}); 
    }

    private static List<string> DetectAvailableIcons()
    {
        var icons = new List<string>(2200);
        const string uriString = "avares://TextoCopier/Assets/Icons/FluentSVGResourceDictionary.axaml";
        var uri = new Uri(uriString);
        var resourceInclude = new ResourceInclude(uri) { Source = uri };
        foreach (object? keyObject in resourceInclude.Loaded.Keys)
        {
            if (keyObject is string keyString)
            {
                if (keyString.Contains("DrawingGroup") || keyString.Contains("ic_"))
                {
                    continue;
                }

                icons.Add(keyString);
            }
        }

        icons.Sort();
        return icons;
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
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        if (activatedView == ActivatedView.GoBack)
        {
            // We always go back to the Group View 
            activatedView = ActivatedView.Group;
        }

        this.DeleteGroupIsVisible = activatedView == ActivatedView.Group;
        this.NewGroupIsVisible = activatedView != ActivatedView.NewGroup;

        switch (activatedView)
        {
            default:
            case ActivatedView.Group:
                this.Activate<GroupViewModel, GroupView>(isFirstActivation, null);
                break;

            case ActivatedView.NewGroup:
                this.Activate<NewEditGroupViewModel, NewEditGroupView>(isFirstActivation, null);
                break;

            case ActivatedView.EditGroup:
                this.Activate<NewEditGroupViewModel, NewEditGroupView>(isFirstActivation, this.templatesModel.SelectedGroup);
                break;

            case ActivatedView.Help:
                this.Activate<HelpViewModel, HelpView>(isFirstActivation, null);
                break;

            case ActivatedView.Settings:
                this.Activate<SettingsViewModel, SettingsView>(isFirstActivation, null);
                break;

            case ActivatedView.EditTemplate:
            case ActivatedView.NewTemplate:
                this.Activate<NewEditTemplateViewModel, NewEditTemplateView>(isFirstActivation, parameter);
                break;
        }
    }

    [RelayCommand]
    public void OnSettings() => this.OnViewActivation(ActivatedView.Settings);

    [RelayCommand]
    public void OnAbout() => this.OnViewActivation(ActivatedView.Help);

    [RelayCommand]
    public void OnNewGroup() => this.OnViewActivation(ActivatedView.NewGroup);

    [RelayCommand]
    public void OnEditGroup() => this.OnViewActivation(ActivatedView.EditGroup);

    [RelayCommand]
    public void OnExit() { }

    private void OnDeleteGroup()
    {
        var group = this.templatesModel.SelectedGroup;
        if (group is null)
        {
            return;
        }

        if (group.Templates.Count > 0)
        {
            var confirmActionParameters = new ConfirmActionParameters
            {
                Title = this.Localizer.Lookup("Shell.GroupDelete.Question"),
                Message = this.Localizer.Lookup("Shell.GroupDelete.Hint"),
                ActionVerb = this.Localizer.Lookup("Shell.Delete"),
                OnConfirm = this.OnDeleteGroupConfirmed,
            };

            this.dialogService.Confirm(this.View.ToasterHost, confirmActionParameters);
        } 
        else
        {
            // No UI confirmation needed if no templates created yet
            this.OnDeleteGroupConfirmed(confirmed: true);
        }
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
                this.Localizer.Lookup("Shell.Deleted"),  this.Localizer.Lookup("Shell.GroupDeleted"), 
                5_000, InformationLevel.Info);
        }
        else
        {
            this.toaster.Show(
                this.Localizer.Lookup("Shell.Error"), this.Localizer.Lookup("Shell.FailGroupDelete"), 
                12_000, InformationLevel.Error);
        }

        this.BindGroupIcons(); 
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

        if( ! isFirstActivation)
        {
            this.Profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
        }
    }

    private void BindGroupIcons()
    {
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
        App.GetRequiredService<GroupViewModel>().CreateViewAndBind();
        App.GetRequiredService<NewEditGroupViewModel>().CreateViewAndBind();
        App.GetRequiredService<HelpViewModel>().CreateViewAndBind();
        App.GetRequiredService<SettingsViewModel>().CreateViewAndBind();
        App.GetRequiredService<NewEditTemplateViewModel>().CreateViewAndBind();
    }

    partial void OnDeleteGroupIsVisibleChanged(bool value)
        => this.GroupsIsVisible = this.DeleteGroupIsVisible || this.NewGroupIsVisible;

    partial void OnNewGroupIsVisibleChanged(bool value)
        => this.GroupsIsVisible = this.DeleteGroupIsVisible || this.NewGroupIsVisible;

}
