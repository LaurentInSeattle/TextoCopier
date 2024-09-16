using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lyt.QuantumSimulator.UnitTests.Algorithms;

[TestClass()]
public class DeutschAlgorithmTests
{
    [TestMethod()]
    public void BalancedGateTest()
    {
        Qubit.ClearCache();
        Assert.IsTrue(DeutschAlgorithm.IsBalanced(new BalancedGate()));
    }

    [TestMethod()]
    public void ConstantGateTest()
    {
        Qubit.ClearCache();
        Assert.IsFalse(DeutschAlgorithm.IsBalanced(new ConstantGate()));
    }

    public class BalancedGate : BinaryOperation
    {
        protected override Complex[,] GetMatrix()
        {
            return new Complex[,]
            {
                { 0, 0, 1, 0 },
                { 0, 1, 0, 0 },
                { 1, 0, 0, 0 },
                { 0, 0, 0, 1 }
            };
        }
    }

    public class ConstantGate : BinaryOperation
    {
        protected override Complex[,] GetMatrix()
        {
            return new Complex[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
        }
    }
}