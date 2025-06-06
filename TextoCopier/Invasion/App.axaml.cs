﻿namespace Lyt.Invasion;

public partial class App : ApplicationBase
{
    public const string Organization = "Lyt";
    public const string Application = "Invasion";
    public const string RootNamespace = "Lyt.Invasion";
    public const string AssemblyName = "Invasion";
    public const string AssetsFolder = "Assets";

    public App() : base(
        App.Organization,
        App.Application,
        App.RootNamespace,
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [
            // Models 
            typeof(LocalizerModel),
            typeof(FileManagerModel),
            typeof(InvasionModel),
        ],
        [
           // Singletons
           typeof(ShellViewModel),
           typeof(WelcomeViewModel),
           typeof(GameOverViewModel),
           typeof(GameViewModel),
           typeof(SetupViewModel),
           typeof(PlayerSetupViewModel),
           typeof(PlayerViewModel),
           typeof(RegionViewModel),
        ],
        [
            // Services 
            App.LoggerService,
            new Tuple<Type, Type>(typeof(IDialogService), typeof(DialogService)),
            new Tuple<Type, Type>(typeof(IDispatch), typeof(Dispatch)),
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
            new Tuple<Type, Type>(typeof(IProfiler), typeof(Profiler)),
            new Tuple<Type, Type>(typeof(IToaster), typeof(Toaster)),
            new Tuple<Type, Type>(typeof(IRandomizer), typeof(Randomizer)),
            new Tuple<Type, Type>(typeof(IAnimationService), typeof(AnimationService)),
        ],
        singleInstanceRequested: true)
    {
        // This should be empty, use the OnStartup override
    }

    private static Tuple<Type, Type> LoggerService =>
            Debugger.IsAttached ? 
                new Tuple<Type, Type>(typeof(ILogger), typeof(LogViewerWindow)) : 
                new Tuple<Type, Type>(typeof(ILogger), typeof(Logger));

    public bool RestartRequired { get; set; }

    protected override async Task OnStartupBegin()
    {
        ViewModel.TypeInitialize(AppHost); 
        var logger = App.GetRequiredService<ILogger>();
        logger.Debug("OnStartupBegin begins");

        // This needs to complete before all models are initialized.
        var fileManager = App.GetRequiredService<FileManagerModel>();
        await fileManager.Configure(
            new FileManagerConfiguration(
                App.Organization, App.Application, App.RootNamespace, App.AssemblyName, App.AssetsFolder));

        // The localizer needs the File Manager, do not change the order.
        var localizer = App.GetRequiredService<LocalizerModel>();
        await localizer.Configure(
            new LocalizerConfiguration
            {
                AssemblyName = App.AssemblyName,
                Languages = ["en-US", "fr-FR", "it-IT"],
                // Use default for all other config parameters 
            });

        logger.Debug("OnStartupBegin complete");
    }

    protected override Task OnShutdownComplete()
    {
        var logger = App.GetRequiredService<ILogger>();
        logger.Debug("On Shutdown Complete");

        if (this.RestartRequired)
        {
            logger.Debug("On Shutdown Complete: Restart Required");
            var process = Process.GetCurrentProcess();
            if ((process is not null) && (process.MainModule is not null))
            {
                Process.Start(process.MainModule.FileName);
            }
        }

        return Task.CompletedTask;
    }

    // Why does it need to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
