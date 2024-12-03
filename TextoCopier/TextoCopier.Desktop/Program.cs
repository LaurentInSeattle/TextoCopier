using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Threading;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Lyt.TextoCopier.Desktop;

class Program
{
    [STAThread]
    public static void Main(string[] args) 
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = 
            AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
        //Dispatcher.UIThread.ShutdownStarted += UIThread_ShutdownStarted;
        //Dispatcher.UIThread.UnhandledException += UIThread_UnhandledException;
        return builder;
    }

    private static void UIThread_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        if (Debugger.IsAttached) { Debugger.Break(); }
    }

    private static void UIThread_ShutdownStarted(object? sender, EventArgs e)
    {
        if (Debugger.IsAttached) { Debugger.Break(); }
    } 
}

#pragma warning restore IDE0130 