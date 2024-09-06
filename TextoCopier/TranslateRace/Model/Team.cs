namespace Lyt.TranslateRace.Model;

public sealed class Team
{
    public Team(string name) => this.Name = name;
    
    public string Name { get; private set; }

    public List<Player> Players { get; set; } = new(16);

    public void Join(Participant participant)
    {
        Player player = new(participant);
        this.Players.Add(player);
    }

    public bool Drop(Player player)
    {
        return this.Players.Remove(player);
    }
}
