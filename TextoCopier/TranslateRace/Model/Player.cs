namespace Lyt.TranslateRace.Model;

public sealed class Player (int index , Participant participant)
{
    public int Index  { get; set; } = index;

    public Participant Participant { get; set; } = participant;

    public int Traductions { get; set; } = 0;

    public int Points { get; set; } = 0;
}
