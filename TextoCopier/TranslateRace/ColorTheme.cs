namespace Lyt.TranslateRace;

public static class ColorTheme
{
    public enum Style
    {
        Default,
        Translucent,
    }

    public static Brush LeftBackground { get; private set; }
    public static Brush LeftForeground { get; private set; }
    public static Brush RightBackground { get; private set; }
    public static Brush RightForeground { get; private set; }
    public static Brush Background { get; private set; }
    public static Brush BeNice { get; private set; }
    public static Brush BeMean { get; private set; }
    public static Brush Text { get; private set; }
    public static Brush UiText { get; private set; }
    public static Brush WinText { get; private set; }

    #pragma warning disable CS8618 
    // Non-nullable field must contain a non-null value when exiting constructor.
    
    static ColorTheme() => ColorTheme.Set(Style.Translucent);
    
    #pragma warning restore CS8618 

    public static void Set(Style style)
    {
        // MUST use SolidColorBrush for all themes
        switch (style)
        {
            default:
            case Style.Default:
                throw new NotImplementedException(nameof(style));


            case Style.Translucent:
                BeNice = new SolidColorBrush(Color.FromArgb(0xC0, 0x2F, 0xA0, 0x5F));
                BeMean = new SolidColorBrush(Colors.LightCoral);
                Text = new SolidColorBrush(Colors.LavenderBlush);
                UiText = new SolidColorBrush(Colors.LightCoral);
                WinText = new SolidColorBrush(Color.FromArgb(0xFF, 0x2F, 0xB0, 0x5F));
                LeftBackground = new SolidColorBrush(Colors.DodgerBlue);
                LeftForeground = new SolidColorBrush(Colors.LightSkyBlue);
                RightBackground = new SolidColorBrush(Colors.LightSalmon);
                RightForeground = new SolidColorBrush(Colors.Firebrick);
                break;
        }
    }
}
