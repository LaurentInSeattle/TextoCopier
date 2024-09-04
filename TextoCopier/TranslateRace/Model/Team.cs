namespace Lyt.TranslateRace.Model;

public sealed class Team
{
    public string Name { get; set; } = string.Empty;

    public List<Player> Players { get; set; } = [];
}
