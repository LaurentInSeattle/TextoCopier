﻿namespace Lyt.QuantumSimulator.Gates;

public class XGate : UnaryOperation
{
    protected override Complex[,] GetMatrix() => this.x_matrix;

    private readonly Complex[,] x_matrix = new Complex[,]
    {
        { 0, 1 },
        { 1, 0 }
    };
}