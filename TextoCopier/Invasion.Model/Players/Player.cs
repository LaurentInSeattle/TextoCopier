namespace Lyt.Invasion.Model.Players;

public abstract class Player
{
    public readonly Dictionary<WealthKind, int> Wealth;

    public readonly Dictionary<ActorKind, int> Population;

#pragma warning disable CS8618 
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // All players have a capital once the game is created, it is set later when building the map 
    protected Player(int index, PlayerInfo playerInfo)
#pragma warning restore CS8618 
    {
        this.Index = index;
        this.Avatar = playerInfo.Avatar;
        this.Name = playerInfo.Name;
        this.EmpireName = playerInfo.EmpireName;
        this.Color = playerInfo.Color;
        this.Territory = new List<Region>(128);
        this.Wealth = this.CreateInitialWealth();
        this.Population = this.CreateInitialPopulation();
    }

    public abstract bool IsHuman { get; }

    public int Index { get; private set; }

    public string Name { get; set; } = string.Empty;

    public string EmpireName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public abstract void Destroy();

    public Region Capital { get; internal set; }

    public List<Region> Territory { get; private set; }

    public Age Age { get; private set; }

    public void DoCollect() { }

    public void DoDeploy() { }
    
    public void DoDestroy() { }
    
    public void DoBuild() { }
    
    public void DoAttack() { }
    
    public void DoColonize() { }
    
    public void DoMove() { }

    private Dictionary<WealthKind, int> CreateInitialWealth()
    {
        Dictionary<WealthKind, int> wealth = new();
        return wealth;

    }

    private Dictionary<ActorKind, int> CreateInitialPopulation()
    {
        Dictionary<ActorKind, int> population = new();
        return population;
    }
}