namespace Lyt.TranslateRace.Messaging;

public enum Assignment
{
    Left, 
    Right, 
    Absent, 
    Participant,
}

public sealed class PlayerAssignmentMessage(PlayerViewModel playerViewModel, Assignment fromAssignment, Assignment toAssignment)
{
    public PlayerViewModel PlayerViewModel { get; private set; } = playerViewModel;

    public Assignment FromAssignment { get; private set; } = fromAssignment;

    public Assignment ToAssignment { get; private set; } = toAssignment;
}
