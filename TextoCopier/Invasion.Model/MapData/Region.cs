namespace Lyt.Invasion.Model.MapData;

public sealed class Region
{
    /// <summary> Factor used to set how much of simplification is needed for the border paths.</summary>
    private const float simplificationTolerance = 0.4f;

    /// <summary> Distance used to determine if a border path is "broken" by the wrapping of the map  </summary>
    private const int maxSquareDistance = 500;

    public readonly Game game;

    /// <summary> Unique ID number of this region  </summary>
    public readonly short Id;

    /// <summary> X and y pixels where region started to grow from </summary>
    public readonly Coordinate Coordinate;

    /// <summary> Center of country, i.e. middle of the country </summary>
    public readonly Coordinate Center;

    /// <summary> Center of country, i.e. middle of the country </summary>
    public readonly Coordinate AltCenter;

    /// <summary> Count of pixels within the region </summary>
    public readonly int Size;

    /// <summary> Ids of all other regions neighbouring this region. </summary>
    public readonly List<short> NeighbourIds;

    /// <summary> Simpliefied border paths. </summary>
    public readonly List<List<Vector2>> SimplifiedPaths;

    public Ecosystem Ecosystem { get; internal set; }

    public Region(
        Game game, short id, Coordinate coordinate, Coordinate center, int size, List<Coordinate> borderCoordinates)
    {
        this.game = game;
        this.Id = id;
        this.Coordinate = coordinate;
        this.Center = center;
        this.Size = size;
        this.Ecosystem = Ecosystem.Unknown;
        this.NeighbourIds = new(16);

        // this.Capacity = 0; // For now

        Region.ClearDuplicateBorderPoints(borderCoordinates);
        var paths = Region.CreateBorderPaths(borderCoordinates);
        this.SimplifiedPaths = Region.SimplifyBorderPaths(paths);
        this.AltCenter = Region.CalculateCenter(paths);
    }

    /// <summary>
    /// Used by PixelMap to add neighbours, since it is not possible to add the neighbours in the constructor, because
    /// the neighbour might not be constructed yet.
    /// </summary>
    internal void AddNeighbour(Region neighbour) => this.NeighbourIds.Add(neighbour.Id);

    private static void ClearDuplicateBorderPoints(List<Coordinate> borderCoordinates)
    {
        var hash = new HashSet<Coordinate>(borderCoordinates.Count);
        foreach (var coordinate in borderCoordinates)
        {
            hash.Add(coordinate);
        }

        if (hash.Count < borderCoordinates.Count)
        {
            Debug.WriteLine("List: " + borderCoordinates.Count);
            Debug.WriteLine("Hash: " + hash.Count);
            borderCoordinates.Clear();
            borderCoordinates.AddRange(hash.ToArray());
        }
    }

    private static List<List<Coordinate>> CreateBorderPaths(List<Coordinate> borderCoordinates)
    {
        var paths = new List<List<Coordinate>>(8);
        var pointsToTest = borderCoordinates;
        Coordinate point;
        Coordinate? lastPoint = null;
        while (pointsToTest.Count > 2)
        {
            var pathPoints = new List<Coordinate>(256);
            if (lastPoint is not null)
            {
                pathPoints.Add(lastPoint);
                pointsToTest.Remove(lastPoint);
                point = lastPoint;
            }
            else
            {
                point = pointsToTest[0];
                pathPoints.Add(point);
                pointsToTest.RemoveAt(0);
            }

            bool found = true;
            while (found)
            {
                int minSquareDistance = int.MaxValue;
                Coordinate? closest = null;
                found = false;
                foreach (var coordinate in pointsToTest)
                {
                    int distance = coordinate.GetRawSquareDistance(point);
                    if (distance == 1)
                    {
                        closest = coordinate;
                        break;
                    }
                    else
                    {
                        if (distance > maxSquareDistance)
                        {
                            lastPoint = coordinate;
                            continue;
                        }

                        if (distance < minSquareDistance)
                        {
                            closest = coordinate;
                            minSquareDistance = distance;
                        }
                    }
                }

                if (closest is not null)
                {
                    found = true;
                    pointsToTest.Remove(closest);
                    pathPoints.Add(closest);
                    point = closest;
                }

            } // Inner loop 

            paths.Add(pathPoints);
        } // Outer loop 

        return paths;
    }

    private static List<List<Vector2>> SimplifyBorderPaths(List<List<Coordinate>> paths)
    {
        var simplifiedPaths = new List<List<Vector2>>(paths.Count);
        foreach (var path in paths)
        {
            // Smooth the border 
            var points = new List<Vector2>();
            foreach (var coordinate in path)
            {
                points.Add(coordinate.ToVector2());
            }

            var avgPoints = PathUtilities.MovingAverage(points);
            var simplified = PathUtilities.Simplify(avgPoints, simplificationTolerance);
            simplifiedPaths.Add(simplified);
        }

        return simplifiedPaths;
    }

    private static Coordinate CalculateCenter(List<List<Coordinate>> paths)
    {
        int x = 0;
        int y = 0;
        int count = 0;
        // Debug.WriteLine("Path Count: " + this.Paths.Count);
        Coordinate center = new(0, 0);
        var path = (from p in paths orderby p.Count descending select p).FirstOrDefault();
        if (path is not null)
        {
            //foreach (var path in this.Paths)
            {
                foreach (var coordinate in path)
                {
                    x += coordinate.X;
                    y += coordinate.Y;
                    count++;
                }
            }

            if (count > 0)
            {
                center = new(x / count, y / count);
            }
        }

        return center;
    }

    #region LATER 

    /// <summary> Biggest army size the region can host </summary>
    public readonly double Capacity;

    /// <summary> Player who currently owns the region, or null </summary>
    public Player? Owner { get; private set; }

    /// <summary> Player who owned the region previously, or null </summary>
    public Player? PreviousOwner { get; private set; }

    // LATER 
    // public Dictionary<ResourceKind, int> Resources = new(10);

    // LATER 
    /// <summary> Size of the army within the region </summary>
    // public int ArmySize { get; set; } = 0;

    public bool IsOwned => this.Owner is not null;

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

    #endregion LATER 
}
