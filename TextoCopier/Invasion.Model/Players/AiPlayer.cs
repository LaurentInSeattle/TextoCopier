namespace Lyt.Invasion.Model.Players;

public sealed class AiPlayer : Player
{
    public AiPlayer(int index, PlayerInfo playerInfo, Game game) : base(index, playerInfo, game)
    {
    }

    public override bool IsHuman => false;

    public override void Turn()
    {

    }

    public override void Destroy()
    {
    }
}
