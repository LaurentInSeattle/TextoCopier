namespace Lyt.Invasion.Model.MapData;

public sealed class Region
{
    /// <summary> Unique ID number of region  </summary>
    public readonly short Id;

    /// <summary> X and y pixels where region started to grow from </summary>
    public readonly Coordinate Coordinate;

    public readonly Ecosystem Ecosystem;

    /// <summary> Center of country, i.e. middle of the country </summary>
    public readonly Coordinate Center;

    /// <summary> Count of pixels within the region </summary>
    public readonly int Size;

    /// <summary> Biggest army size the region can host </summary>
    public readonly double Capacity;

    /// <summary> Every pixel on the border of this region. This is used to draw the country as vector graph. </summary>
    public readonly List<Coordinate> BorderCoordinates;

    /// <summary> Ids of all other regions neighbouring this region. </summary>
    public readonly List<short> NeighbourIds;

    public Region(
        short id, Coordinate coordinate, Coordinate center, int size, List<Coordinate> borderCoordinates)
    {
        this.Id = id;
        this.Coordinate = coordinate;
        this.Center = center;
        this.Size = size;
        this.Capacity = 0; // For now
        this.BorderCoordinates = borderCoordinates;
        this.NeighbourIds = new (16);
        this.Path = new(256);
        this.SimplifiedPath = new(256);

        this.ClearDuplicateBorderPoints(); 
        this.CreateBorderPath();
        this.SimplifyBorderPath();
    }

    // LATER 
    // public Dictionary<ResourceKind, int> Resources = new(10);

    /// <summary> Player who currently owns the region, or null </summary>
    public Player? Owner { get; private set;  }

    /// <summary> Player who owned the region previously, or null </summary>
    public Player? PreviousOwner { get; private set; }

    public List<Coordinate> Path { get; private set; }

    public List<Vector2> SimplifiedPath { get; private set; }

    /// <summary> Size of the army within the region </summary>
    public int ArmySize { get; set; } = 0;

    public bool IsOwned => this.Owner is not null;

    /// <summary>
    /// Used by PixelMap to add neighbours, since it is not possible to add the neighbours in the constructor, because
    /// the neighbour might not be constructed yet.
    /// </summary>
    internal void AddNeighbour(Region neighbour) => this.NeighbourIds.Add(neighbour.Id);

    private void ClearDuplicateBorderPoints ()
    {
        var hash = new HashSet<Coordinate>(this.BorderCoordinates.Count);
        foreach (var coordinate in this.BorderCoordinates)
        {
            hash.Add(coordinate);
        }

        if( hash.Count < this.BorderCoordinates.Count)
        {
            Debug.WriteLine("List: " + this.BorderCoordinates.Count);
            Debug.WriteLine("Hash: " + hash.Count);
            this.BorderCoordinates.Clear();
            this.BorderCoordinates.AddRange(hash.ToArray());
        }
    }

    private void CreateBorderPath()
    {
        var pathPoints = new List<Coordinate>();
        var pointsToTest = this.BorderCoordinates.ToList(); 
        pointsToTest.RemoveAt(0);
        pathPoints.Add(this.BorderCoordinates[0]);
        foreach (var coordinate in this.BorderCoordinates)
        {
            int minSquareDistance = int.MaxValue;
            Coordinate? closest = null; 
            foreach (var point in pointsToTest)
            {
                int distance = coordinate.GetRawSquareDistance(point);
                if ( distance == 1)
                {
                    closest = point;
                    break;
                }
                else
                {
                    if ( distance < minSquareDistance)
                    {
                        closest = point;
                        minSquareDistance = distance;
                    }
                }
            }

            if (closest is not null)
            {
                pointsToTest.Remove(closest);
                pathPoints.Add(closest);
            } 
        }

        this.Path = pathPoints;
    }

    //private void CreateBorderPath()
    //{
    //    var points = new List<Vector2>();
    //    foreach (var coordinate in this.BorderCoordinates)
    //    {
    //        points.Add(coordinate.ToVector2());
    //    }

    //    var path = PathUtilities.CreatePath(points, this.Center.ToVector2());
    //    this.Path = path;
    //}

    private void SimplifyBorderPath ()
    {
        // Smooth the border 
        var points = new List<Vector2>();
        foreach (var coordinate in this.Path)
        {
            points.Add(coordinate.ToVector2());
        }

        var avgPoints = PathUtilities.MovingAverage(points);
        var simplified = PathUtilities.Simplify(avgPoints, 0.4f);
        this.SimplifiedPath = simplified;
    }

    /// <summary> Has this region a neighbouring region with a different owner ? </summary>
    public bool HasEnemies(Map map)
    {
        if (this.Owner is null)
        {
            return false;
        }

        foreach (short neighbourId in this.NeighbourIds)
        {
            Region neighbour = map[neighbourId];
            if (neighbour.Owner is null)
            {
                return false;
            }

            if (neighbour.Owner.Index != this.Owner.Index)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary> Borders neighbour to this region ? </summary>
    public bool IsNeighbour(Region region) => this.NeighbourIds.Contains(region.Id);

    /// <summary> Have all neighbours the same owner ? </summary>
    public bool IsInland(Map map)
    {
        if (this.Owner is null)
        {
            return false;
        }

        foreach (short neighbourId in this.NeighbourIds)
        {
            Region neighbour = map[neighbourId];
            if (neighbour.Owner is null)
            {
                return false;
            }

            if (neighbour.Owner.Index != this.Owner.Index)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Have all neighbours the same owner except the attacked region.
    /// This is used to decide how many armies should be moved to the attacked region.
    /// </summary>
    public bool IsInland(Map map, Region attacked)
    {
        if (this.Owner is null)
        {
            return false;
        }

        foreach (short neighbourId in this.NeighbourIds)
        {
            Region neighbour = map[neighbourId];
            if (neighbour.Owner is null)
            {
                return false;
            }

            if (neighbour == attacked)
            {
                continue;
            }

            if (neighbour.Owner.Index != this.Owner.Index)
            {
                return false;
            }
        }

        return true;
    }
}
