using static Lyt.Avalonia.Persistence.FileManagerModel;

namespace Lyt.Invasion.Model;

public sealed class InvasionModel : ModelBase
{
    private const string InvasionModelFilename = "Invasion";

    private readonly FileManagerModel fileManager;

    private Game? game;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE0021 // Use expression body for constructor 
    public InvasionModel() : base(null, null)
    {
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        // Empty CTOR required for deserialization 
        this.ShouldAutoSave = false;
    }
#pragma warning restore IDE0021
#pragma warning restore CS8625 
#pragma warning restore CS8618

    public InvasionModel(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
    }

    public override async Task Initialize() => await this.Load();

    public override async Task Shutdown()
    {
        if (this.IsDirty)
        {
            await this.Save();
        }
    }

    public Task Load()
    {
        try
        {
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            string msg = "Failed to load InvasionModel from " + InvasionModel.InvasionModelFilename;
            this.Logger.Fatal(msg);
            throw new Exception(msg, ex);
        }
    }

    public override Task Save()
    {
        // Null check is needed !
        // If the File Manager is null we are currently loading the model and activating properties on a second instance 
        // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
        if (this.fileManager is not null)
        {
            this.fileManager.Save(Area.User, Kind.Json, InvasionModel.InvasionModelFilename, this);
            base.Save();
        }

        return Task.CompletedTask;
    }

    public void NewGame(GameOptions gameOptions)
    {
        this.game = new Game(gameOptions);
    }
}
