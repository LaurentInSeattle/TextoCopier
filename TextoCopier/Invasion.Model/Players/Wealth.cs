namespace Lyt.Invasion.Model.Players;

/// <summary> Wealth of a player. </summary>
public sealed class Wealth
{
    public Dictionary<WealthKind, int> wealth { get; private set; }

    public Wealth()
    {
        this.wealth = new Dictionary<WealthKind, int>();
    }

    public void AllocateInitialWealth(Player player)
    {
        this.wealth.Clear();
    }
}
