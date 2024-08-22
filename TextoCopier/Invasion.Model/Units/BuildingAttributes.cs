namespace Lyt.Invasion.Model.Units;

public class Capacity
{
    public Trait Trait { get; set; }

    public int Quantity { get; set; }
}

public class Cost
{
    public WealthKind Kind { get; set; } 

    public int Amount { get; set; }
}

public class Production
{
    public WealthKind Kind { get; set; }

    public int Amount { get; set; }
}

public class Boost
{
    public WealthKind Kind { get; set; }

    public int PercentAmount { get; set; }
}

public class Evolution
{
    public BuildingKind Kind { get; set; }

    public Age Age { get; set; }
}

public class BuildingAttributes
{
    public BuildingKind Kind { get; set; }

    public string Name { get; set; } =string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsLandmark { get; set; } 

    /// <summary> When the building can be built. </summary>
    public Age MinimumAge { get; set; } = Age.Stone;

    /// <summary> When the building stops being productive, and becomes "historical". </summary>
    public Age MaximumAge { get; set; } = Age.Stone;

    public int LimitPerRegion { get; set; } = 3;

    public int LimitPerEmpire { get; set; } = 10;

    public List<Trait> Traits { get; set; } = [];

    public List<Capacity> Capacities { get; set; } = [];

    public Evolution Evolution { get; set; } = new ();

    public List<Cost> ConstructionCosts { get; set; } = [];

    public List<Cost> UpkeepCosts { get; set; } = [];

    /// <summary> What is returned to the player if the building is destroyed. </summary>
    public List<Cost> ResidualValue { get; set; } = [];

    public List<Boost> Boosts { get; set; } = [];

    public List<Production> ProductionPerWorker { get; set; } = [];
}
