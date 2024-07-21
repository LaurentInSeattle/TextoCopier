namespace Lyt.Invasion.Model.Utilities;

public static class PathUtilities
{
    private const float BigEpsilon = 1_000_000 * float.Epsilon; 

    public static List<Vector2> CreatePath(List<Vector2> points, Vector2 center)
    {
        double angle;
        float cx = center.X;
        float cy = center.Y;
        var list = new List<Tuple<float, float, Vector2>>(points.Count);
        foreach (Vector2 point in points)
        {
            float dx = point.X - cx;
            float dy = point.Y - cy;
            if ((Math.Abs(dx) < BigEpsilon) && (Math.Abs(dy) < BigEpsilon))
            {
                continue;
            }
            if (Math.Abs(dy) < BigEpsilon) 
            {
                angle = Math.Sign(dx) * Math.PI ;
            }
            else if (Math.Abs(dx) < BigEpsilon)
            {
                angle = Math.Sign(dy) * Math.PI / 2.0;
            }
            else
            {
                angle = Math.Atan2(dy, dx);
            }

            angle += 0.5+Math.Tau;
            list.Add(new Tuple<float, float, Vector2>((float)angle, dx * dx + dy * dy, point));
        }

        var sortedList =
            (from item in list orderby item.Item1 ascending , item.Item2 select item.Item3);
        return sortedList.ToList();
    }

    private static void SquaredPolar(Vector2 point, Vector2 center, out float angle, out float squaredRadius)
    {
        float dx = point.X - center.X;
        float dy = point.Y - center.Y;
        angle = (float)Math.Atan2(dy, dx);
        squaredRadius = dx * dx + dy * dy;
    }

    // Return minimum distance between line segment v-w and point p
    private static float SquaredSegmentDistance(Vector2 p, Vector2 v, Vector2 w)
    {
        float lengthSquared = Vector2.DistanceSquared(v, w);
        if (lengthSquared < 0.000_001f)
        {
            // Segment degenerated to single point
            return Vector2.DistanceSquared(p, v);
        }

        // Consider the line extending the segment, parameterized as v + t (w - v).
        // We find projection of point p onto the line. 
        // It falls where t = [(p-v) . (w-v)] / |w-v|^2
        // We clamp t from [0,1] to handle points outside the segment vw.
        float t = Math.Max(0.0f, Math.Min(1.0f, Vector2.Dot(p - v, w - v) / lengthSquared));
        Vector2 projection = v + t * (w - v);
        return Vector2.DistanceSquared(p, projection);
    }

    // simplification using optimized the Ramer-Douglas-Peucker algorithm with recursion elimination
    private static List<Vector2> RamerDouglasPeucker(IList<Vector2> points, double sqTolerance)
    {
        int len = points.Count;
        int?[] markers = new int?[len];
        int? first = 0;
        int? last = len - 1;
        int? index = 0;
        var stack = new List<int?>();
        var newPoints = new List<Vector2>();
        markers[first.Value] = markers[last.Value] = 1;

        while ((last is not null) && (first is not null))
        {
            double maxSqDist = 0.0;
            for (int? i = first + 1; i < last; i++)
            {
                float sqDist = SquaredSegmentDistance(points[i.Value], points[first.Value], points[last.Value]);
                if (sqDist > maxSqDist)
                {
                    index = i;
                    maxSqDist = sqDist;
                }
            }

            if (maxSqDist > sqTolerance)
            {
                markers[index.Value] = 1;
                stack.AddRange([first, index, index, last]);
            }


            if (stack.Count > 0)
            {
                last = stack[^1];
                stack.RemoveAt(stack.Count - 1);
            }
            else
            {
                last = null;
            }

            if (stack.Count > 0)
            {
                first = stack[^1];
                stack.RemoveAt(stack.Count - 1);
            }
            else
            {
                first = null;
            }
        }

        for (int i = 0; i < len; i++)
        {
            if (markers[i] != null)
            {
                newPoints.Add(points[i]);
            }
        }

        return newPoints;
    }

    /// <summary> Simplifies a list of points to a shorter list of points. </summary>
    /// <remarks> See: https://en.wikipedia.org/wiki/Ramer%E2%80%93Douglas%E2%80%93Peucker_algorithm </remarks>
    /// <param name="points">Points original list of points</param>
    /// <param name="tolerance">Tolerance tolerance in the same measurement as the point coordinates</param>
    /// <returns>Simplified list of points</returns>
    public static List<Vector2> Simplify(IList<Vector2> points, float tolerance = 0.3f)
    {
        if (points.Count == 0)
        {
            return [];
        }

        if (points.Count <= 3)
        {
            return new List<Vector2>(points);
        }

        return PathUtilities.RamerDouglasPeucker(points, tolerance * tolerance);
    }

    public static List<Vector2> MovingAverage(IList<Vector2> values)
    {
        int count = values.Count;
        if (count <= 5)
        {
            return [.. values];
        }

        List<Vector2> averagedValues = new(count)
        {
            values[0],
            Vector2.Add(values[0], values[1]) / 2.0f
        };
        for (int i = 2; i < count - 2; ++i)
        {
            var v1 = Vector2.Add(values[i - 2], values[i - 1]);
            var v2 = Vector2.Add(values[i], values[i + 1]);
            var v3 = Vector2.Add(v1, v2);
            var v = Vector2.Add(v3, values[i + 2]);
            averagedValues.Add(v / 5.0f);
        }

        averagedValues.Add(Vector2.Add(values[count - 2], values[count - 1]) / 2.0f);
        averagedValues.Add(values[count - 1]);

        return averagedValues;
    }
}

