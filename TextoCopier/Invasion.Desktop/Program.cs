using System;
using Avalonia;
using Avalonia.Media.Fonts;
using Lyt.Invasion;

namespace Lyt.Invasion.Desktop;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().UsePlatformDetect().WithInterVariableFont().LogToTrace();
}

public static class AppBuilderExtension
{
    public static AppBuilder WithInterVariableFont(this AppBuilder appBuilder)
        => appBuilder.ConfigureFonts(fontManager =>
        {
            fontManager.AddFontCollection(new InterVariableFontCollection());
        });
}

public sealed class InterVariableFontCollection : EmbeddedFontCollection
{
    public InterVariableFontCollection() : base(
        new Uri("fonts:Inter-V", UriKind.Absolute),
        new Uri("avares://Invasion/Assets/Fonts/Inter-V.ttf", UriKind.Absolute)) { }
}