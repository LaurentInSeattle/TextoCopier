using System;
using Avalonia;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Lyt.TextoCopier.Desktop;

class Program
{
    [STAThread]
    public static void Main(string[] args) 
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}

#pragma warning restore IDE0130 