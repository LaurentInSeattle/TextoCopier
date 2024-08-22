namespace Lyt.Invasion.Model.Units;

public sealed class Building (BuildingKind kind, Player owner, Region region)
{
    public BuildingKind Kind { get; set; } = kind;

    public Player Owner { get; set; } = owner;

    public Region Location { get; set; } = region;
}
