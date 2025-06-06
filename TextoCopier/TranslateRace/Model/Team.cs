namespace Lyt.TranslateRace.Model;

public sealed class Team(string name, bool isLeft)
{
#pragma warning disable CA2211 // Non-constant fields should not be visible

    public static string LeftName = "Squadra Azzurra";

    public static string RightName = "Scuderia Ferrari";

#pragma warning restore CA2211 // Non-constant fields should not be visible

    public string Name { get; private set; } = name;

    public bool IsLeft { get; private set; } = isLeft;

    public int CompletedTurns { get; set; } = 0;

    public int Score { get; set; } = 0;

    public List<Player> Players { get; set; } = new(16);

    public void Join(int index, Participant participant) => this.Players.Add(new(index, participant));

    public bool Drop(Player player) => this.Players.Remove(player);

    public int FirstPlayerIndex
    {
        get
        {
            if (this.Players.Count == 0 )
            {
                throw new Exception("No Players");
            }

            return (from p in this.Players select p.Index).Min();
        }
    }


    public Player At(int index)
    {
        var player = (from p in this.Players where p.Index == index select p).FirstOrDefault();
        return player is null ? throw new Exception("Null Player ??? ") : player;
    }
}
