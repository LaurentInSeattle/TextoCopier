namespace Lyt.Invasion.Model.Players;

public sealed class AiPlayer : Player
{
    public AiPlayer(int index, PlayerInfo playerInfo) : base(index, playerInfo)
    {
    }

    public override bool IsHuman => false;

    public override void Destroy()
    {
    }
}
