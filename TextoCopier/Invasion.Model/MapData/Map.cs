namespace Lyt.Invasion.Model.MapData;

public sealed class Map
{
    private readonly Region[] regions = [];

    /// <summary> Indexer to regions by region.Id /// </summary>
    public Region this[short index] => this.regions[index];

    public void Destroy()
    {
    }
}
