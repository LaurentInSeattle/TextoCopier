namespace Lyt.TranslateRace.Model;

public sealed class Team(string name, bool isLeft)
{
    public static string LeftName = "Squadra Azzurra";

    public static string RightName = "Scuderia Ferrari";

    public string Name { get; private set; } = name;

    public bool IsLeft { get; private set; } = isLeft;

    public List<Player> Players { get; set; } = new(16);

    public void Join(int index, Participant participant)
    {
        Player player = new(index, participant);
        this.Players.Add(player);
    }

    public bool Drop(Player player)
    {
        return this.Players.Remove(player);
    }
}
