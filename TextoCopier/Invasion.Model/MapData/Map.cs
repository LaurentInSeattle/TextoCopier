namespace Lyt.Invasion.Model.MapData;

public sealed class Map
{
    public readonly ILogger Logger;

    public readonly IMessenger Messenger;

    public readonly IRandomizer Randomizer;

    public Map(Game game, IMessenger messenger, ILogger logger, IRandomizer randomizer)
    {
        this.Messenger = messenger;
        this.Logger = logger;
        this.Randomizer = randomizer;

        this.Regions = new Region[game.GameOptions.RegionCount];
        this.PixelMap = new PixelMap(game, this, messenger, logger, randomizer);
        this.GenerateLandscape();
        foreach (Region region in this.Regions)
        {
            region.Resources = new Resources(region);
        }
    }

    public Region[] Regions { get; private set; }

    public PixelMap PixelMap { get; private set; }

    /// <summary> Indexer to regions by region.Id /// </summary>
    public Region this[short index] => this.Regions[index];

    public void AddRegionAt(short index, Region region) => this.Regions[index] = region;

    private void GenerateLandscape()
    {
        var sortedBySizeRegions = (from region in this.Regions orderby region.Size descending select region).ToList();
        int fivePc = this.Regions.Length / 20;
        int tenPc = this.Regions.Length / 10;

        // Largest 5% of regions are deep sea 
        var oceanRegions = new List<Region>(2 * fivePc);
        for (int i = 0; i < fivePc; ++i)
        {
            var region = sortedBySizeRegions[i];
            region.Ecosystem = Ecosystem.Ocean;
            oceanRegions.Add(region);

            // First neighbour is also Ocean, if not already assigned 
            int neighbourId = region.NeighbourIds[0];
            var neighbour = this.Regions[neighbourId];
            if (neighbour.Ecosystem == Ecosystem.Unknown)
            {
                neighbour.Ecosystem = Ecosystem.Ocean;
                oceanRegions.Add(neighbour);
            }
        }

        var coastalRegions = new HashSet<Region>(2 * fivePc);
        foreach (var region in oceanRegions)
        {
            foreach (int neighbourId in region.NeighbourIds)
            {
                var neighbour = this.Regions[neighbourId];
                if (neighbour.Ecosystem == Ecosystem.Unknown)
                {
                    coastalRegions.Add(neighbour);
                }
            }
        }

        foreach (var region in coastalRegions)
        {
            int count = 0; 
            foreach (int neighbourId in region.NeighbourIds)
            {
                var neighbour = this.Regions[neighbourId];
                if (neighbour.Ecosystem == Ecosystem.Ocean)
                {
                    ++ count ;
                }
            }

            if( count == 0)
            {
                // WTF ? 
                if ( Debugger.IsAttached ) {  Debugger.Break(); }
                continue;
            }
            else if (count == 1)
            {
                region.Ecosystem = Ecosystem.Coast;
            }
            else // count > 1 
            {
                region.Ecosystem = Ecosystem.Wetland;
            }
        }

        // Smallest 10% of regions are mountains 
        var mountainRegions = new List<Region>(2 * tenPc);
        for (int i = 0; i < tenPc; ++i)
        {
            int k = sortedBySizeRegions.Count - 1 - i;
            var region = sortedBySizeRegions[k];
            region.Ecosystem = Ecosystem.Mountain;
            mountainRegions.Add(region);

            // First neighbour is also Mountain, if not already assigned 
            int neighbourId = region.NeighbourIds[0];
            var neighbour = this.Regions[neighbourId];
            if (neighbour.Ecosystem == Ecosystem.Unknown)
            {
                neighbour.Ecosystem = Ecosystem.Mountain;
                mountainRegions.Add(neighbour);
            }
        }

        // Neighbours of Mountains, if not already assigned, are Hills 
        foreach (var region in mountainRegions)
        {
            foreach (int neighbourId in region.NeighbourIds)
            {
                var neighbour = this.Regions[neighbourId];
                if (neighbour.Ecosystem == Ecosystem.Unknown)
                {
                    neighbour.Ecosystem = Ecosystem.Hills;
                }
            }
        }

        // Everything else is Plains: Forest, Grassland or Desert 
        foreach (var region in this.Regions)
        {
            if (region.Ecosystem != Ecosystem.Unknown)
            {
                continue;
            }

            // Choose from Forest, Grassland, Desert based on latitude 
            int latitude = 100 * region.AltCenter.Y / this.PixelMap.YCount;
            bool polar = (latitude < 15) || (latitude > 85);
            bool equatorial = (latitude < 57) && (latitude > 42);
            if (polar)
            {
                region.Ecosystem = Ecosystem.Grassland;
            }
            else if (equatorial)
            {
                region.Ecosystem = Ecosystem.Desert;
            }
            else
            {
                region.Ecosystem = Ecosystem.Forest;
            }

            // Debug.WriteLine( "Latitude: " + latitude + "  " + region.Ecosystem );
        }
    }

    public void Destroy()
    {
    }
}
