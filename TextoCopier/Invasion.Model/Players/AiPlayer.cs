namespace Lyt.Invasion.Model.Players;

public sealed class AiPlayer : Player
{
    public override bool IsHuman => false;

    public override void Destroy()
    {
    }
}
