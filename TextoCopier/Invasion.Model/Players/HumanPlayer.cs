namespace Lyt.Invasion.Model.Players;

public sealed class HumanPlayer : Player
{
    public HumanPlayer(int index, PlayerInfo playerInfo, Game game) : base(index, playerInfo, game)
    {
    }

    public override bool IsHuman => true;

    public override void Destroy()
    {
    }
}
