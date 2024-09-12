namespace Lyt.TranslateRace.Model;

public sealed class Team(string name, bool isLeft)
{
    public static string LeftName = "Squadra Azzurra";

    public static string RightName = "Scuderia Ferrari";

    public string Name { get; private set; } = name;

    public bool IsLeft { get; private set; } = isLeft;

    public int Score { get; set; } = 0;

    public List<Player> Players { get; set; } = new(16);

    public void Join(int index, Participant participant) => this.Players.Add(new(index, participant));

    public bool Drop(Player player) => this.Players.Remove(player);
}
