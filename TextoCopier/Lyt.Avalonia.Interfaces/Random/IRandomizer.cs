namespace Lyt.Avalonia.Interfaces.Random; 

public interface IRandomizer
{
    int Next(int min, int max);

    int Next(int max);

    float NextSingle ();

    void Shuffle<T> (IList<T> list);
}
