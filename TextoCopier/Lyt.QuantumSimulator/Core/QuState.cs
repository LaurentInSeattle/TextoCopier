namespace Lyt.QuantumSimulator.Core;

public class QuState
{
    private Dictionary<int, int> posMap;
    private Complex[] tensor;

    QuState(Complex[] tensor)
    {
        this.posMap = [];
        this.tensor = tensor;
    }

    public QuState(ComplexPoint p, int key)
    {
        this.posMap = [];
        this.posMap.Add(key, 0);
        this.tensor = [p.X, p.Y];
    }

    public static QuState Combine(QuState s1, QuState s2)
    {
        var s = new QuState(AlgebraUtility.TensorProduct(s1.tensor, s2.tensor))
        {
            posMap = s2.posMap
        };

        int offset = s2.posMap.Count;
        foreach (var pair in s1.posMap)
        {
            s.posMap.Add(pair.Key, pair.Value + offset);
        }

        return s;
    }

    public void MultiplyBy(UnaryGate gate, int key)
    {
        int bitLen = AlgebraUtility.Log2(this.tensor.Length);
        int bitPosition = this.posMap[key];
        this.tensor = AlgebraUtility.Multiply( gate.GetMatrix(bitLen, bitPosition), this.tensor );
    }

    public void MultiplyBy(BinaryGate gate, int key1, int key2)
    {
        int bitLen = AlgebraUtility.Log2(this.tensor.Length);
        int bit1Pos = this.posMap[key1];
        int bit2Pos = this.posMap[key2];
        this.tensor = AlgebraUtility.Multiply( gate.GetMatrix(bitLen, bit1Pos, bit2Pos), this.tensor);
    }

    public ComplexPoint? Peek(int key)
    {
        var decoder = new VectorDecoder();
        var r = decoder.Solve(this.tensor);
        if (r != null)
        {
            return r[r.Length - this.posMap[key] - 1]; // big-endian
        }

        return null;
    }

    public bool Measure(int key)
    {
        int pos = this.posMap[key];
        int val = this.Sample();
        bool m = BinaryUtility.HasBit(val, pos);
        this.Collapse(pos, m);
        return m;
    }

    private int Sample()
    {
        double aux = 0.0;
        double x = RandomUtility.NextDouble();
        for ( int i = 0; i < this.tensor.Length; i++)
        {
            double magnitude = this.tensor[i].Magnitude; 
            aux += magnitude * magnitude;
            if (aux > x)
            {
                return i;
            }
        }

        return 0;
    }

    private void Collapse(int pos, bool b)
    {
        for (int i = 0; i < this.tensor.Length; i++)
        {
            if (BinaryUtility.HasBit(i, pos) != b)
            {
                this.tensor[i] = Complex.Zero;
            }
        }

        double sum = Math.Sqrt(this.tensor.Sum(x => x.Magnitude * x.Magnitude) );
        for (int i = 0; i < this.tensor.Length; i++)
        {
            this.tensor[i] /= sum;
        }
    }
}
