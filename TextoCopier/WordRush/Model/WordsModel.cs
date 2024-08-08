namespace Lyt.WordRush.Model;

using static FileManagerModel;

public sealed partial class WordsModel : ModelBase
{
    private readonly FileManagerModel fileManager;

    public WordsModel(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
    }

    public override Task Initialize() 
    {  
        /* TODO */  
        return Task.CompletedTask;
    }
}
