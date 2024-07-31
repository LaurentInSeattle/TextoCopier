namespace Lyt.Avalonia.PanZoom;

public static class DoubleHelpers
{
    public static double ToRealNumber(this double value, double defaultValue = 0.0)
        => (double.IsInfinity(value) || double.IsNaN(value)) ? defaultValue : value;

    public static bool IsWithinOnePercent(this double value, double testValue)
        => Math.Abs(value - testValue) < .01 * testValue;
}
