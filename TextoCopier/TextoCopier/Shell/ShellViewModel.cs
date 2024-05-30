namespace Lyt.TextoCopier.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel() : this(ApplicationBase.GetRequiredService<TemplatesModel>()) { }

    private readonly TemplatesModel templatesModel;

    public ShellViewModel(TemplatesModel templatesModel)
    {
        this.Groups = [];
        this.GlyphCommand = new Command(this.OnGlyph);
        this.templatesModel = templatesModel;
    }

    private void OnGlyph(object? parameter)
    {
        parameter ??= string.Empty;
        this.Logger.Info("Clicked on glyph!  " + parameter.ToString());
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Select default language 
        var localizer = App.GetRequiredService<LocalizerModel>();
        localizer.DetectAvailableLanguages();
        localizer.SelectLanguage("it-IT");

        //string hello = localizer.Lookup("My.Strings.HelloWorld"); 
        //this.Logger.Info(hello);
        //string _ = localizer.Lookup("Whatever");

        var vm = App.GetRequiredService<GroupViewModel>();
        vm.CreateViewAndBind();
        this.View!.ShellViewContent.Content = vm.View; 
        this.Bind();
    }

    private void Bind()
    {
        this.Logger.Info("Binding groups");

        SelectionGroup selectionGroup = this.View!.SelectionGroup;
        bool selected = true;
        var list = new List<GroupIconViewModel>(this.templatesModel.Groups.Count);
        foreach (var group in this.templatesModel.Groups)
        {
            list.Add(new GroupIconViewModel(group, selectionGroup, selected));
            selected = false;
        }

        this.Groups = list;
        this.Logger.Info("Groups: " + this.templatesModel.Groups.Count);

        foreach (var group in this.View!.GroupsItemsControl.Items)
        {
            if (group is GroupIconView view)
            {
                var icon = view.Icon;
                var dc = view.DataContext;
            }
        }
    }

    public List<GroupIconViewModel> Groups { get => this.Get<List<GroupIconViewModel>>()!; set => this.Set(value); }

    public ICommand GlyphCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

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
