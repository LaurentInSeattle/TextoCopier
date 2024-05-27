
namespace Lyt.TextoCopier.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel()
    {
        this.GlyphCommand = new Command(this.OnGlyph);
    }

    private void OnGlyph(object? parameter)
    {
        if (parameter is null) 
        { 
            parameter = string.Empty; 
        }

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
    }
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
