namespace Lyt.TranslateRace.Messaging;

public enum Assignment
{
    Left, 
    Right, 
    Absent, 
    Participant,
    Delete,
}

public sealed record class PlayerAssignmentMessage(
    PlayerViewModel PlayerViewModel, Assignment FromAssignment, Assignment ToAssignment); 