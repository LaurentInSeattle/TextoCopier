namespace Lyt.TranslateRace.Model;

public class Participant
{
    public static readonly string ParticipantsFilename = "translaterace_participants";

    public string Name { get; set; } = string.Empty;

    public int Participations { get; set; } = 0;
}
