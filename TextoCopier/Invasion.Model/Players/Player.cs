namespace Lyt.Invasion.Model.Players;

public abstract class Player
{
    protected Player(int index, PlayerInfo playerInfo)
    {
        this.Index = index;
        this.Avatar = playerInfo.Avatar;
        this.Name = playerInfo.Name;
        this.EmpireName = playerInfo.EmpireName;
        this.Color = playerInfo.Color;
        this.Territory = new List<Region>(128);
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

    public Dictionary<WealthCriteria, int> Wealth = new(10);

    public Dictionary<Actor, int> Population = new(10);

    public void DoCollect() { }

    public void DoDeploy() { }
    
    public void DoDestroy() { }
    
    public void DoBuild() { }
    
    public void DoAttack() { }
    
    public void DoColonize() { }
    
    public void DoMove() { }
}