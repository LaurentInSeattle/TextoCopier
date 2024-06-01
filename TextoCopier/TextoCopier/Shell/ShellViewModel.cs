
namespace Lyt.TextoCopier.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel() : this(ApplicationBase.GetRequiredService<TemplatesModel>()) { }

    private readonly TemplatesModel templatesModel;
    private readonly IToaster toaster;


    public ShellViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.toaster = ApplicationBase.GetRequiredService<IToaster>(); 
        this.Groups = [];
        this.SettingsCommand = new Command(this.OnSettings);
        this.AboutCommand = new Command(this.OnAbout);
        this.ExitCommand = new Command(this.OnExit);
        this.NewGroupCommand = new Command(this.OnNewGroup);
        this.DeleteGroupCommand = new Command(this.OnDeleteGroup);
    }

    private void OnSettings(object? _) { }

    private void OnAbout(object? _) {}

    private void OnExit(object? _) { }

    private void OnNewGroup(object? _) { }

    private void OnDeleteGroup(object? _) { }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        if( this.View is null)
        {
            throw new Exception("Failed to startup..."); 
        }

        // Select default language 
        var localizer = App.GetRequiredService<LocalizerModel>();
        localizer.DetectAvailableLanguages();
        localizer.SelectLanguage("it-IT");

        var vm = App.GetRequiredService<GroupViewModel>();
        vm.CreateViewAndBind();
        this.View.ShellViewContent.Content = vm.View; 
        this.Bind();

        this.toaster.Host = this.View.ToasterHost;
        this.toaster.Show(
            localizer.Lookup("Shell.Ready") , localizer.Lookup("Shell.Greetings"), 4_000, ToastLevel.Info); 
    }

    private void Bind()
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
            // TODO: Notify 
        }
    }

    public List<GroupIconViewModel> Groups { get => this.Get<List<GroupIconViewModel>>()!; set => this.Set(value); }

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AboutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
    
    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
    
    public ICommand NewGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
    
    public ICommand DeleteGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    #region WORKFLOW ~ Maybe later 

    //protected override async void OnViewLoaded()
    //{
    //    base.OnViewLoaded();
    //    //this.SetupWorkflow();
    //    //if (this.Workflow is not null)
    //    //{
    //    //    await this.Workflow.Initialize();
    //    //    _ = this.Workflow.Start();
    //    //}
    //}

    //private void SetupWorkflow()
    //{
    //    StateDefinition<WorkflowState, WorkflowTrigger, Bindable> Create<TViewModel, TView>(
    //        WorkflowState state, WorkflowTrigger trigger, WorkflowState target)
    //        where TViewModel : Bindable, new()
    //        where TView : Control, new()
    //    {
    //        var vm = App.GetRequiredService<TViewModel>();
    //        vm.Bind(new TView());
    //        if (vm is WorkflowPage<WorkflowState, WorkflowTrigger> page)
    //        {
    //            page.State = state;
    //            page.Title = state.ToString();
    //            return
    //                new StateDefinition<WorkflowState, WorkflowTrigger, Bindable>(
    //                    state, page, null, null, null, null,
    //                    [
    //                        new TriggerDefinition<WorkflowState, WorkflowTrigger> ( trigger, target , null )
    //                    ]);
    //        }
    //        else
    //        {
    //            string msg = "View is not a Workflow Page";
    //            this.Logger.Error(msg);
    //            throw new Exception(msg);
    //        }
    //    }

    //    var startup = Create<StartupViewModel, StartupView>(WorkflowState.Startup, WorkflowTrigger.Ready, WorkflowState.Login);
    //    var login = Create<LoginViewModel, LoginView>(WorkflowState.Login, WorkflowTrigger.LoggedIn, WorkflowState.Select);
    //    var select = Create<SelectViewModel, SelectView>(WorkflowState.Select, WorkflowTrigger.Selected, WorkflowState.Process);
    //    var process = Create<ProcessViewModel, ProcessView>(WorkflowState.Process, WorkflowTrigger.Complete, WorkflowState.Login);

    //    var stateMachineDefinition =
    //        new StateMachineDefinition<WorkflowState, WorkflowTrigger, Bindable>(
    //            WorkflowState.Startup, // Initial state
    //            [ 
    //                // List of state definitions
    //                startup, login , select, process,
    //            ]);

    //    this.Workflow =
    //        new WorkflowManager<WorkflowState, WorkflowTrigger>(
    //            this.Logger, this.Messenger, this.View!.WorkflowContent!, stateMachineDefinition);
    //}

    #endregion WORKFLOW ~ Maybe later 
}
