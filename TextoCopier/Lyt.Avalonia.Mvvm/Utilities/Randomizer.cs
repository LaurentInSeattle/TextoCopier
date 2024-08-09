namespace Lyt.Avalonia.Mvvm.Utilities; 

public sealed class Randomizer : IRandomizer
{
    private readonly Random random;

    public Randomizer() => this.random = new Random(Environment.TickCount);       
    
    public float NextSingle() => this.random.NextSingle();

    public int Next(int min, int max) => this.random.Next(min, max);

    public int Next(int max) => this.random.Next(max);

    /// <summary>Shuffles the specified list using the Fisher Yates algorithm.</summary>
    public void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = this.random.Next(0, 1 + i);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }
}
