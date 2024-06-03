namespace Lyt.TextoCopier.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel() : this(ApplicationBase.GetRequiredService<TemplatesModel>()) { }

    private readonly TemplatesModel templatesModel;
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;


    public ShellViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();
        this.toaster = ApplicationBase.GetRequiredService<IToaster>();
        this.Groups = [];
        this.SettingsCommand = new Command(this.OnSettings);
        this.AboutCommand = new Command(this.OnAbout);
        this.ExitCommand = new Command(this.OnExit);
        this.NewGroupCommand = new Command(this.OnNewGroup);
        this.DeleteGroupCommand = new Command(this.OnDeleteGroup);
    }

    private void OnExit(object? _) { }

    private void OnSettings(object? _) => this.Activate<SettingsViewModel, SettingsView>();

    private void OnAbout(object? _) => this.Activate<HelpViewModel, HelpView>();

    private void OnNewGroup(object? _) => this.Activate<NewGroupViewModel, NewGroupView>();

    private void OnDeleteGroup(object? _) { }

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
        this.SetupWorkflow();
        this.BindGroupIcons();
        this.Activate<GroupViewModel, GroupView>();

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

    private void Activate<TViewModel, TControl>()
        where TViewModel : Bindable<TControl>
        where TControl : Control, new()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        this.dialogService.Dismiss();
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

    private void SetupWorkflow()
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

    public List<GroupIconViewModel> Groups { get => this.Get<List<GroupIconViewModel>>()!; set => this.Set(value); }

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AboutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NewGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
