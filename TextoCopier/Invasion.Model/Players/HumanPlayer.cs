namespace Lyt.Invasion.Model.Players;

public sealed class HumanPlayer : Player
{
    public HumanPlayer(int index, PlayerInfo playerInfo) : base(index, playerInfo)
    {
    }

    public override bool IsHuman => true;

    public override void Destroy()
    {
    }
}
