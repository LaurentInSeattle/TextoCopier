﻿namespace Lyt.WordRush;

public partial class App : ApplicationBase
{
    public const string Organization = "Lyt";
    public const string Application = "WordRush";
    public const string RootNamespace = "Lyt.WordRush";
    public const string AssemblyName = "WordRush";
    public const string AssetsFolder = "Assets";

    public App() : base(
        App.Organization,
        App.Application,
        App.RootNamespace,
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [
            // Models 
            typeof(FileManagerModel),
            typeof(WordsModel),
            typeof(GameHistory),
        ],
        [
           // Singletons
           typeof(ShellViewModel),
           typeof(SetupViewModel),
           typeof(CountdownViewModel),
           typeof(GameViewModel),
           typeof(GameOverViewModel),
        ],
        [
            // Services 
#if DEBUG
            new Tuple<Type, Type>(typeof(ILogger), typeof(LogViewerWindow)),
#else
            new Tuple<Type, Type>(typeof(ILogger), typeof(Logger)),
#endif
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

    public bool RestartRequired { get; set; }

    protected override async Task OnStartupBegin()
    {
        ViewModel.TypeInitialize(ApplicationBase.AppHost); 

        var logger = App.GetRequiredService<ILogger>();
        logger.Debug("OnStartupBegin begins" ); 

        // This needs to complete before all models are initialized.
        var fileManager = App.GetRequiredService<FileManagerModel>();
        await fileManager.Configure(
            new FileManagerConfiguration(
                App.Organization, App.Application, App.RootNamespace, App.AssemblyName, App.AssetsFolder));

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
            if ( (process is not null) && (process.MainModule is not null ) )
            {
                Process.Start(process.MainModule.FileName);
            } 
        }

        return Task.CompletedTask; 
    }

    // Why does it need to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
