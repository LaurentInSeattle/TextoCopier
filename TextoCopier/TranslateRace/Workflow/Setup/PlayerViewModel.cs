namespace Lyt.TranslateRace.Workflow.Setup;

public sealed class PlayerViewModel : Bindable<PlayerView>
{
    private readonly Assignment assignment;

    public PlayerViewModel(Participant participant, Assignment assignment)
        : base(disablePropertyChangedLogging: true, disableAutomaticBindingsLogging: true)
    {
        this.Participant = participant;
        this.assignment = assignment;
        this.Name = this.Participant.Name;
        this.ViewSetup();
    }

    private void ViewSetup()
    {
        switch (this.assignment)
        {
            default:
            case Assignment.Participant:
                this.LeftGlyphSource = "arrow_left";
                this.CenterGlyphSource = "arrow_down";
                this.RightGlyphSource = "arrow_right";
                this.LeftIsVisible = true;
                this.CenterIsVisible = true;
                break;

            case Assignment.Left:
                this.LeftGlyphSource = "arrow_down";
                this.CenterGlyphSource = "arrow_down";
                this.RightGlyphSource = "arrow_right";
                this.LeftIsVisible = true;
                this.CenterIsVisible = false;
                break;

            case Assignment.Right:
                this.LeftGlyphSource = "arrow_left";
                this.CenterGlyphSource = "arrow_down";
                this.RightGlyphSource = "arrow_down";
                this.LeftIsVisible = true;
                this.CenterIsVisible = false;
                break;

            case Assignment.Absent:
                this.LeftGlyphSource = "arrow_left";
                this.CenterGlyphSource = "arrow_up";
                this.RightGlyphSource = "dismiss";
                this.LeftIsVisible = false;
                this.CenterIsVisible = true;
                break;
        }
    }

    public bool LeftIsVisible { get; private set; }

    public bool CenterIsVisible { get; private set; }

    public Participant Participant { get; private set; }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnLeft(object? _)
    {
        Assignment newAssignment;
        switch (this.assignment)
        {
            default:
            case Assignment.Participant:
                newAssignment = Assignment.Left;
                break;

            case Assignment.Left:
                newAssignment = Assignment.Absent;
                break;

            case Assignment.Right:
                newAssignment = Assignment.Participant;
                break;

            case Assignment.Absent:
                return;
        }

        IMessenger messenger = ApplicationBase.GetRequiredService<IMessenger>();
        messenger.Publish(new PlayerAssignmentMessage(this, this.assignment, newAssignment));
    }

    private void OnRight(object? _)
    {
        Assignment newAssignment;
        switch (this.assignment)
        {
            default:
            case Assignment.Participant:
                newAssignment = Assignment.Right;
                break;

            case Assignment.Left:
                newAssignment = Assignment.Participant;
                break;

            case Assignment.Right:
                newAssignment = Assignment.Absent;
                break;

            case Assignment.Absent:
                newAssignment = Assignment.Delete;
                break;
        }

        IMessenger messenger = ApplicationBase.GetRequiredService<IMessenger>();
        messenger.Publish(new PlayerAssignmentMessage(this, this.assignment, newAssignment));
    }

    private void OnCenter(object? _)
    {
        Assignment newAssignment;
        switch (this.assignment)
        {
            default:
            case Assignment.Participant:
                newAssignment = Assignment.Absent;
                break;

            case Assignment.Right:
            case Assignment.Left:
                return;

            case Assignment.Absent:
                newAssignment = Assignment.Participant;
                break;
        }

        IMessenger messenger = ApplicationBase.GetRequiredService<IMessenger>();
        messenger.Publish(new PlayerAssignmentMessage(this, this.assignment, newAssignment));
    }

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    #region Bound properties 

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string LeftGlyphSource { get => this.Get<string>()!; set => this.Set(value); }

    public string CenterGlyphSource { get => this.Get<string>()!; set => this.Set(value); }

    public string RightGlyphSource { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand LeftCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand RightCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CenterCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
    
    #endregion  Bound properties 
}
