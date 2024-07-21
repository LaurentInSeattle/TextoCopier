using System.Drawing;

namespace Lyt.Invasion.Model.MapData;

public sealed class Region
{
    public readonly Game game;

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

    private List<List<Coordinate>> CoordinateBuckets { get; set; }

    public Region(
        Game game, short id, Coordinate coordinate, Coordinate center, int size, List<Coordinate> borderCoordinates)
    {
        this.game = game;
        this.Id = id;
        this.Coordinate = coordinate;
        this.Center = center;
        this.Size = size;
        this.Capacity = 0; // For now
        this.BorderCoordinates = borderCoordinates;
        this.NeighbourIds = new(16);
        this.Paths = new(4);
        this.SimplifiedPaths = new(4);
        this.CoordinateBuckets = new(4);

        this.ClearDuplicateBorderPoints();
        this.CreateCoordinateBuckets();
        this.CreateBorderPaths();
        this.SimplifyBorderPaths();
    }

    public List<List<Coordinate>> Paths { get; private set; }

    public List<List<Vector2>> SimplifiedPaths { get; private set; }

    #region LATER 

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

    #endregion LATER 

    /// <summary>
    /// Used by PixelMap to add neighbours, since it is not possible to add the neighbours in the constructor, because
    /// the neighbour might not be constructed yet.
    /// </summary>
    internal void AddNeighbour(Region neighbour) => this.NeighbourIds.Add(neighbour.Id);

    private void CreateCoordinateBuckets()
    {
        for (int i = 0; i < 4; i++)
        {
            this.CoordinateBuckets.Add(new List<Coordinate>(256));
        }

        var gameOptions = game.GameOptions;
        int mapHalfWidth = gameOptions.PixelWidth / 2;
        int mapHalfHeight = gameOptions.PixelHeight / 2;

        Coordinate startPoint = this.BorderCoordinates[0];
        this.CoordinateBuckets[0].Add(startPoint);
        for (int i = 1; i < this.BorderCoordinates.Count; ++i)
        {
            var point = this.BorderCoordinates[i];
            bool farX = startPoint.GetRawXDistance(point) > mapHalfWidth;
            bool farY = startPoint.GetRawXDistance(point) > mapHalfHeight;
            if (farX && farY)
            {
                this.CoordinateBuckets[3].Add(point);
            }
            else if (farX && !farY)
            {
                this.CoordinateBuckets[1].Add(point);
            }
            else if (!farX && farY)
            {
                this.CoordinateBuckets[2].Add(point);
            }
            else
            {
                this.CoordinateBuckets[0].Add(point);
            }
        }

        //Debug.WriteLine(
        //    "Buckets: " + 
        //    this.CoordinateBuckets[0].Count + " " + this.CoordinateBuckets[1].Count + " " +
        //    this.CoordinateBuckets[2].Count + " " + this.CoordinateBuckets[3].Count); 
    }

    private void ClearDuplicateBorderPoints()
    {
        var hash = new HashSet<Coordinate>(this.BorderCoordinates.Count);
        foreach (var coordinate in this.BorderCoordinates)
        {
            hash.Add(coordinate);
        }

        if (hash.Count < this.BorderCoordinates.Count)
        {
            Debug.WriteLine("List: " + this.BorderCoordinates.Count);
            Debug.WriteLine("Hash: " + hash.Count);
            this.BorderCoordinates.Clear();
            this.BorderCoordinates.AddRange(hash.ToArray());
        }
    }

    private void CreateBorderPaths()
    {
        foreach (var bucket in this.CoordinateBuckets)
        {
            if (bucket.Count == 0)
            {
                continue;
            }
            
            // Create a copy is necessary
            var pointsToTest = bucket.ToList();
            pointsToTest.RemoveAt(0);

            var pathPoints = new List<Coordinate>();
            pathPoints.Add(bucket[0]);
            foreach (var coordinate in bucket)
            {
                int minSquareDistance = int.MaxValue;
                Coordinate? closest = null;
                foreach (var point in pointsToTest)
                {
                    int distance = coordinate.GetRawSquareDistance(point);
                    if (distance == 1)
                    {
                        closest = point;
                        break;
                    }
                    else
                    {
                        if (distance < minSquareDistance)
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

            this.Paths.Add(pathPoints);
        }
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

    private void SimplifyBorderPaths()
    {
        foreach (var path in this.Paths)
        {
            // Smooth the border 
            var points = new List<Vector2>();
            foreach (var coordinate in path)
            {
                points.Add(coordinate.ToVector2());
            }

            var avgPoints = PathUtilities.MovingAverage(points);
            var simplified = PathUtilities.Simplify(avgPoints, 0.4f);
            this.SimplifiedPaths.Add (simplified);
        }
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
