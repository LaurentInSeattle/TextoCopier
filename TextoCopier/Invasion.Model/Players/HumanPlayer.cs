namespace Lyt.Invasion.Model.Players;

public sealed class HumanPlayer : Player
{
    public override bool IsHuman => true;

    public override void Destroy()
    {
    }
}
