namespace Lyt.Invasion.Model.MapData;

public sealed class PixelMap
{
    /// <summary> Count of map-pixels in x direction, largest V coordinate is XCount-1 </summary>
    public readonly int XCount;

    /// <summary>Largest possible x coordinate </summary>
    public readonly int XMax;

    /// <summary> Count of map-pixels in y direction, largest y coordinate is YCount-1</summary>
    public readonly int YCount;

    /// <summary> Largest possible y coordinate </summary>
    public readonly int YMax;

    /// <summary> The Id for each pixel </summary>
    /// <remarks> there are hundred thousands of pixels. It's better to store the Id as short</remarks>
    private readonly short[,] RegionIdsPerPixel;

    /// <summary> Indexer, returns the Id of the region owning that pixel </summary>
    public short this[Coordinate coordinate] => this.RegionIdsPerPixel[coordinate.X, coordinate.Y];

    /// <summary> Count of pixels the largest country occupies </summary>
    public double BiggestCountrySize { get; private set; }
}
