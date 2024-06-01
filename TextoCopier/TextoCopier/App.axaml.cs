namespace Lyt.TextoCopier;

public partial class App : ApplicationBase
{
    public const string Organization = "Lyt";
    public const string Application = "TextoCopier";
    public const string RootNamespace = "Lyt.TextoCopier";

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
            typeof(TemplatesModel),
        ],
        [
           // Singletons
           typeof(Profiler),
           typeof(ShellViewModel),
           typeof(GroupViewModel),
        ],
        [
            // Services 
#if DEBUG
            new Tuple<Type, Type>(typeof(ILogger), typeof(LogViewerWindow)),
#else
            new Tuple<Type, Type>(typeof(ILogger), typeof(Logger)),
#endif
            new Tuple<Type, Type>(typeof(IDialogService), typeof(DialogService)),
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
            new Tuple<Type, Type>(typeof(IToaster), typeof(Toaster)),
        ],
        singleInstanceRequested: true)
    {
        // This should be empty, use the OnStartup override
    }

    protected override async Task OnStartupBegin()
    {
        // This needs to complete before all models are initialized.
        var fileManager = App.GetRequiredService<FileManagerModel>();
        await fileManager.Configure(new FileManagerConfiguration(App.Organization, App.Application, App.RootNamespace));
    }

    // Why does it need to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
