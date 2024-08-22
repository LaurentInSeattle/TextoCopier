namespace Lyt.Invasion.Model.Players;

/// <summary> Wealth of a player. </summary>
public sealed class Wealth
{
    public Dictionary<WealthKind, float> wealth { get; private set; }

    public Wealth() => this.wealth = [];

    public void AllocateInitialWealth(/* Player player*/ )
    {
        // MAYBE LATER 
        // Player could be needed, if AI players would need to cheat on initial wealth 
        this.wealth.Clear();
        this.wealth.Add(WealthKind.Wood, 100.0f);
        this.wealth.Add(WealthKind.Food, 500.0f);
        this.wealth.Add(WealthKind.Stone, 100.0f);
        this.wealth.Add(WealthKind.Gold, 0.0f);
        this.wealth.Add(WealthKind.Energy, 0.0f);
        this.wealth.Add(WealthKind.Knowledge, 0.0f);
        this.wealth.Add(WealthKind.Aura, 0.0f);
        this.wealth.Add(WealthKind.Health, 100.0f); 
        this.wealth.Add(WealthKind.Pollution, 0.0f);
    }
}