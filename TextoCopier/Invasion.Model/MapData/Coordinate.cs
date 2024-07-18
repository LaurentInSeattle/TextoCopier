namespace Lyt.Invasion.Model.MapData;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Coordinate : IEquatable<Coordinate>
{
    /// <summary> Offset from left border of PixelMap </summary>
    public readonly int X = int.MinValue;

    /// <summary> Offset from top border of PixelMap </summary>
    public readonly int Y = int.MinValue;

    public Coordinate(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary> 
    /// Returns square distance between 2 coordinates. 
    /// It is faster to calculate the square distance than the distance, bit often the square is enough.
    /// </summary>
    public int GetSquareDistance(PixelMap pixelMap, Coordinate other)
    {
        int xDistance = (pixelMap.XCount + this.X - other.X) % pixelMap.XCount;
        if (xDistance > pixelMap.XCount / 2)
        {
            // in a wrap around map, the x-distance between 2 points cannot be more than half the map height
            xDistance = pixelMap.XCount - xDistance;
        }
        int yDistance = (pixelMap.YCount + this.Y - other.Y) % pixelMap.YCount;
        if (yDistance > pixelMap.YCount / 2)
        {
            // in a wrap around map, the y-distance between 2 points cannot be more than half the map width
            yDistance = pixelMap.YCount - yDistance;
        }

        return xDistance * xDistance + yDistance * yDistance;
    }

    /// <summary> Returns true if both coordinates have the same values </summary>
    public bool Equals(Coordinate? other) => other is not null && this.X == other.X && this.Y == other.Y;

    /// <summary> Returns true if both are coordinates and have the same values </summary>
    public override bool Equals(object? obj) => obj is Coordinate coordinate && this.Equals(coordinate);

    public static bool operator ==(Coordinate left, Coordinate right) => left.Equals(right);

    public static bool operator !=(Coordinate left, Coordinate right) => !left.Equals(right);

    /// <summary> Returns the hash code for this coordinate. </summary>
    public override int GetHashCode() => this.X << 16 + this.Y;

    /// <summary> Returns the X and Y value of the coordinate as string </summary>
    public override string ToString()
        => "X: " + (this.X == int.MinValue ? "undef" : this.X.ToString()) +
            ", Y: " + (this.Y == int.MinValue ? "undef" : this.Y.ToString());

    private string GetDebuggerDisplay() => this.ToString();
}
