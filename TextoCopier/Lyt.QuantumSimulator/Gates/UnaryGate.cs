namespace Lyt.QuantumSimulator.Gates;

public abstract class UnaryGate
{
    public void Apply(Qubit q) => q.State.MultiplyBy(this, q.Id);

    public Complex[,] GetMatrix(int bitLen, int bitPos)
    {
        var matrix = this.GetMatrix();

        if (bitLen > 1)
        {
            var table = AlgebraUtility.LookupTable(matrix);
            int mLen = 1 << bitLen;
            matrix = new Complex[mLen, mLen];

            for (int i = 0; i < mLen; i++)
            {
                int x = BinaryUtility.HasBit(i, bitPos) ? 1 : 0;
                foreach (var y in table[x])
                {
                    int j = BinaryUtility.SetBit(i, bitPos, BinaryUtility.HasBit(y.Key, 0));

                    matrix[i, j] = y.Value;
                    matrix[j, i] = y.Value;
                }
            }
        }

        return matrix;
    }

    protected abstract Complex[,] GetMatrix();
}
