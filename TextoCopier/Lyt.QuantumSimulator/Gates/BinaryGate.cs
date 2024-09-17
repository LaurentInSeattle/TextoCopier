namespace Lyt.QuantumSimulator.Gates;

public abstract class BinaryGate
{
    public void Apply(Qubit q1, Qubit q2)
    {
        Qubit.Combine(q1, q2);
        q1.State.MultiplyBy(this, q1.Id, q2.Id);
    }

    public Complex[,] GetMatrix(int bitLen, int bit1Pos, int bit2Pos)
    {
        var matrix = this.GetMatrix();
        var table = AlgebraUtility.LookupTable(matrix);

        int mLen = 1 << bitLen;
        matrix = new Complex[mLen, mLen];

        for (int i = 0; i < mLen; i++)
        {
            int x =
                (BinaryUtility.HasBit(i, bit1Pos) ? 2 : 0) +
                (BinaryUtility.HasBit(i, bit2Pos) ? 1 : 0);

            foreach (var y in table[x])
            {
                int  j = BinaryUtility.SetBit( i, bit1Pos, BinaryUtility.HasBit(y.Key, 1) );
                j = BinaryUtility.SetBit( j, bit2Pos, BinaryUtility.HasBit(y.Key, 0));

                matrix[i, j] = y.Value;
                matrix[j, i] = y.Value;
            }
        }

        return matrix;
    }

    protected abstract Complex[,] GetMatrix();
}
