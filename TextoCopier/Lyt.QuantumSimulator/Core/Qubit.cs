namespace Lyt.QuantumSimulator.Core;

public class Qubit
{
    private static readonly Dictionary<QuState, List<Qubit>> cache = [];

    private static int counter = 0;

    public Qubit(bool b)
    {
        this.Id = ++counter;
        this.SetState(new QuState(new ComplexPoint(b), this.Id));
    }

    public int Id { get; private set; }

    public QuState State { get; private set; }

    public static void Combine(Qubit q1, Qubit q2)
    {
        var s1 = q1.State;
        var s2 = q2.State;
        if (s1 != s2)
        {
            var s = QuState.Combine(s1, s2);
            UpdateCache(s1, s);
            UpdateCache(s2, s);
        }
    }

    public ComplexPoint? Peek() => this.State.Peek(this.Id);

    public bool Measure() => this.State.Measure(this.Id);
    
    private void SetState(QuState state)
    {
        this.State = state;
        this.AddToCache();
    }

    private void AddToCache()
    {
        if (!cache.TryGetValue(this.State, out List<Qubit>? value))
        {
            value = [];
            cache.Add(this.State, value);
        }

        value.Add(this);
        DumpCache("Add");
    }

    private static void UpdateCache(QuState oldState, QuState newState)
    {
        foreach (var qubit in cache[oldState])
        {
            qubit.SetState(newState);
        }

        cache.Remove(oldState);

        DumpCache("Update");
    }

    public static void ClearCache()
    {
        counter = 0;
        cache.Clear();
    }

    [Conditional("DumpCache")]
    private static void DumpCache (string method)
    {
        Debug.WriteLine("Cache: " + method + "  " + cache.Count.ToString()); 
        Debug.Indent();
        foreach (var state in cache.Keys)
        {
            Debug.WriteLine("State: " + state);
            Debug.Indent();
            foreach (var qubit in cache[state])
            {
                Debug.WriteLine("Id: " +qubit.Id);
            }
            Debug.Unindent();
        }

        Debug.Unindent();
        Debug.WriteLine("-");
    }
}
