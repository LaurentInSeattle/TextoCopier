namespace Lyt.Invasion.Model.MapData;

public sealed class Map
{
    private readonly Region[] regions;

    private readonly PixelMap pixelMap; 

     /// <summary> Indexer to regions by region.Id /// </summary>
    public Region this[short index] => this.regions[index];

    public Map(GameOptions gameOptions)
    {
        this.regions = new Region[gameOptions.RegionCount];
        this.pixelMap = new PixelMap(gameOptions);
    }

    public void Destroy()
    {
    }
}
