namespace Lyt.Invasion.Converters;

public class EnumColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || value.ToString() is not string colorString)
        {
            return AvaloniaProperty.UnsetValue;
        }

        try
        {
            return colorString.ColorNameToColor();
        }
        catch (Exception)
        {
            return AvaloniaProperty.UnsetValue;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => throw new NotImplementedException("Cant convert color back to string");
}
