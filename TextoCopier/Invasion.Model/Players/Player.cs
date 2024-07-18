namespace Lyt.Invasion.Model.Players;

public abstract class Player
{
    public int Id { get; private set; }

    public string Name { get; set; } = string.Empty;

    public string EmpireName { get; set; } = string.Empty;

    public abstract bool IsHuman { get; }

    public abstract void Destroy();

    public Age Age { get; private set; }

    public Dictionary<WealthCriteria, int> Wealth = new(10);

    public Dictionary<Actor, int> Population = new(10);
}