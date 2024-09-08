namespace Lyt.TranslateRace.Messaging;

public sealed class PlayerDropMessage(Player player)
{
    public Player Player { get; private set; } = player;
}
