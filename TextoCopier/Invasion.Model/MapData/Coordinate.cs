namespace Lyt.Invasion.Model.MapData;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Coordinate(int x, int y) : IEquatable<Coordinate>
{
    /// <summary> Offset from left border of PixelMap </summary>
    public readonly int X = x;

    /// <summary> Offset from top border of PixelMap </summary>
    public readonly int Y = y;

    /// <summary> Converts to System.Numerics.Vector2 </summary>
    public Vector2 ToVector2() => new(this.X, this.Y);

    /// <summary>
    /// Returns the coordinate of the pixel left of this coordinate pixel. 
    /// If the coordinate is at the left border, the right most pixel gets returned.
    /// </summary>
    public Coordinate Left(PixelMap map) => new(this.X < 1 ? map.XMax : this.X - 1, this.Y);

    /// <summary>
    /// Returns the coordinate of the pixel right of this coordinate pixel. 
    /// If the coordinate is at the right border, the left mostpixel gets returned.
    /// </summary>
    public Coordinate Right(PixelMap map)
    {
        int newX = this.X + 1;
        if (newX >= map.XCount)
        {
            newX = 0;
        }

        return new Coordinate(newX, this.Y);
    }

    /// <summary>
    /// Returns the coordinate of the pixel above of this coordinate pixel. 
    /// If the coordinate is at the top border, the bottom most pixel gets returned.
    /// </summary>
    public Coordinate Up(PixelMap map) => new(this.X, this.Y < 1 ? map.YCount - 1 : this.Y - 1);

    /// <summary>
    /// Returns the coordinate of the pixel below of this coordinate pixel. 
    /// If the coordinate is at the bottom border, the top most pixel gets returned.
    /// </summary>
    public Coordinate Down(PixelMap map)
    {
        int newY = this.Y + 1;
        if (newY >= map.YCount)
        {
            newY = 0;
        }

        return new(this.X, newY);
    }

    /// <summary> 
    /// Returns square distance between 2 coordinates. 
    /// It is faster to calculate the square distance than the distance, bit often the square is enough.
    /// </summary>
    public int GetRawSquareDistance(Coordinate other)
    {
        int xDistance = this.X - other.X;
        int yDistance = this.Y - other.Y;
        return xDistance * xDistance + yDistance * yDistance;
    }

    /// <summary> Returns distance between 2 coordinates along the X axis.  </summary>
    public int GetRawXDistance(Coordinate other) => Math.Abs (this.X - other.X);

    /// <summary> Returns distance between 2 coordinates along the Y axis.  </summary>
    public int GetRawYDistance(Coordinate other) => Math.Abs(this.Y - other.Y);

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
