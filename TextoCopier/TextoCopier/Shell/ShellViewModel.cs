﻿namespace Lyt.TextoCopier.Shell;

using static ViewActivationMessage;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly TemplatesModel templatesModel;
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;
    private readonly LocalizerModel localizer;

    public ShellViewModel(
        TemplatesModel templatesModel, LocalizerModel localizer, 
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.templatesModel = templatesModel;
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;

        this.templatesModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
        this.messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);

        this.Groups = [];
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
        this.localizer.DetectAvailableLanguages();
        string preferredLanguage = this.templatesModel.Language;
        this.Logger.Debug("Language: " + preferredLanguage);
        this.localizer.SelectLanguage(preferredLanguage);

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
                this.localizer.Lookup("Shell.Ready"), this.localizer.Lookup("Shell.Greetings"), 
                5_000, InformationLevel.Info);
        }
        else
        {
            this.toaster.Show(
                this.localizer.Lookup("Shell.NoGroups.Title"), this.localizer.Lookup("Shell.NoGroups.Hint"), 
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

#pragma warning disable IDE0051 // Remove unused private members
    private void OnSettings(object? _) => this.OnViewActivation(ActivatedView.Settings);

    private void OnAbout(object? _) => this.OnViewActivation(ActivatedView.Help);

    private void OnNewGroup(object? _) => this.OnViewActivation(ActivatedView.NewGroup);

    private void OnEditGroup(object? _) => this.OnViewActivation(ActivatedView.EditGroup);

#pragma warning disable CA1822 // Mark members as static
    private void OnExit(object? _) { }
#pragma warning restore CA1822 

    private void OnDeleteGroup(object? _)
#pragma warning restore IDE0051 
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
                Title = this.localizer.Lookup("Shell.GroupDelete.Question"),
                Message = this.localizer.Lookup("Shell.GroupDelete.Hint"),
                ActionVerb = this.localizer.Lookup("Shell.Delete"),
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
                this.localizer.Lookup("Shell.Deleted"),  this.localizer.Lookup("Shell.GroupDeleted"), 
                5_000, InformationLevel.Info);
        }
        else
        {
            this.toaster.Show(
                this.localizer.Lookup("Shell.Error"), this.localizer.Lookup("Shell.FailGroupDelete"), 
                12_000, InformationLevel.Error);
        }

        this.BindGroupIcons(); 
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

        if( ! isFirstActivation)
        {
            this.profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
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
