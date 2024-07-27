namespace Lyt.Invasion.Utilities;

public static class ColorUtilities
{
    public static Color ColorNameToColor(this string knownColor)
    {
        var type = typeof(Colors);
        var colorProperty = type.GetProperty(knownColor, BindingFlags.Static | BindingFlags.Public);
        if (colorProperty is not null)
        {
            var getMethod = colorProperty.GetGetMethod();
            object? colorObject = getMethod?.Invoke(null, null);
            if (colorObject is Color color)
            {
                return color;
            }
        }

        throw new Exception("Invalid or unknown color");
    }
}
