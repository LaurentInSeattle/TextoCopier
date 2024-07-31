namespace Lyt.Avalonia.PanZoom;

public class ZoomAdjustConverter : MarkupExtension, IValueConverter
{

    public object? Convert(object? value, Type targetType, object? _, CultureInfo culture)
    {
        if ((value is double doubleValue) && doubleValue > 0)
        {
            return Math.Log(doubleValue);
        }

        return 1.0;

    }

    public object? ConvertBack(object? value, Type targetType, object? _, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            return Math.Exp(doubleValue);
        }

        return 1.0;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this; 
}
