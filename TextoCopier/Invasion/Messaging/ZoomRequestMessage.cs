namespace Lyt.Invasion.Messaging;

public sealed class ZoomRequestMessage(double value)
{
    public double ZoomFactor { get; private set; } = value;
}
