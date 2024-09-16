namespace Lyt.QuantumSimulator.Gates;

public class ZGate : UnaryOperation
{
    protected override Complex[,] GetMatrix()
    {
        return z_matrix;
    }

    private readonly Complex[,] z_matrix = new Complex[,]
    {
        { 1, 0 },
        { 0, -1 }
    };
}
