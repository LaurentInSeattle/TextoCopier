namespace Lyt.TranslateRace.Workflow.Setup;

public sealed partial class PlayerViewModel : ViewModel<PlayerView>
{
    private readonly Assignment assignment;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? leftGlyphSource;

    [ObservableProperty]
    private string? centerGlyphSource;

    [ObservableProperty]
    private string? rightGlyphSource;

    public PlayerViewModel(Participant participant, Assignment assignment)
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

    [RelayCommand]
    public void OnLeft()
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

    [RelayCommand]
    public void OnRight()
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

    [RelayCommand]
    public void OnCenter(object? _)
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
}
