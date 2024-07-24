namespace Lyt.Invasion.Model.Utilities;

/// <summary> Shuffles lists of objects. </summary>
public static class Shuffler
{
    /// <summary>Shuffles the specified list using the Fisher Yates algorithm.</summary>
    public static void Shuffle<T>(this List<T> list, Random rnd)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = rnd.Next(0, 1 + i);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }
}
