namespace Lyt.Invasion.Model.MapData;

public sealed class Map
{
    public Map(Game game, IMessenger messenger, ILogger logger)
    {
        this.Messenger = messenger;
        this.Logger = logger;
        this.Regions = new Region[game.GameOptions.RegionCount];
        this.PixelMap = new PixelMap(game, this, messenger, logger);
    }

    public ILogger Logger { get; private set; }

    public IMessenger Messenger { get; private set; }

    public Region[] Regions { get; private set; }

    public PixelMap PixelMap { get; private set; }

    /// <summary> Indexer to regions by region.Id /// </summary>
    public Region this[short index] => this.Regions[index];

    public void AddRegionAt(short index, Region region) => this.Regions[index] = region;

    public void Destroy()
    {
    }
}
