namespace Lyt.TextoCopier.Models;

using static FileManagerModel;

public sealed partial class TemplatesModel : ModelBase
{
    private static readonly TemplatesModel DefaultTemplate =
        new()
        {
            Groups =
            [
                new Group
                {
                    Name = "Me",
                    Description = "Personal Items",
                    Icon = "Person",
                    Templates =
                    [
                        new Template { Name = "Email" , Value = "JaneDoe@domain.com" },
                        new Template { Name = "First" , Value = "Jane" },
                        new Template { Name = "Last" , Value = "Doe" },
                        new Template { Name = "Full Name" , Value = "Jane Doe" },
                        // LATER 
                        // new Template { Name = "Full Name" , Value = "{First} {Last}" },
                    ]
                },
                new Group
                {
                    Name = "Italian on Zoom",
                    Description = "Codes and Ids for Zoom",
                    Icon = "ItalianFlag",
                    Templates =
                    [
                        new Template { Name = "Meeting Id" , Value = "842 4041 3222" },
                        new Template { Name = "Meeting Code" , Value = "717140" },
                        new Template { Name = "Screen Name" , Value = "Enzo" },
                        new Template { Name = "Kahoot" , Value = "https://kahoot.it/" },
                    ]
                }
            ]
        };

    private const string TemplatesModelFilename = "Templates";

    private readonly FileManagerModel fileManager;

    public TemplatesModel() : base() 
    {
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        FileManagerModel fileManager = App.GetRequiredService<FileManagerModel>();
        this.fileManager = fileManager;
    }

    public override async Task Initialize() => await this.Load();

    public override Task Shutdown() => Task.CompletedTask;

    public List<Group> Groups { get; set; } = [];

    public Task Load()
    {
        if (!this.fileManager.Exists(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename))
        {
            this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, TemplatesModel.DefaultTemplate);
        }

        TemplatesModel model =
            this.fileManager.Load<TemplatesModel>(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename);
        this.Groups = model.Groups;
        return Task.CompletedTask;
    }

    public override Task Save()
    {
        this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, this);
        base.Save();
        return Task.CompletedTask;
    }

    private delegate bool ModelOperationDelegate (Group group, string parameter1, string parameter2, out string message);

    private bool ModelOperation (ModelOperationDelegate modelOperation, string groupName , string parameter1 , string parameter2, out string message )
    {
        bool status = this.CheckGroup(groupName, out message);
        if (status)
        {
            Group group = this.GetGroup(groupName);
            status = modelOperation(group, parameter1, parameter2, out message);
            if (status)
            {
                this.IsDirty = true;
                this.NotifyUpdate();
            }
        } 

        return status;
    }

    [Conditional("DEBUG")]
    private static void TestJSonSaveLoad()
    {
        FileManagerModel fileManager = App.GetRequiredService<FileManagerModel>();
        fileManager.Save(Area.User, Kind.Json, nameof(DefaultTemplate), TemplatesModel.DefaultTemplate);
        var model = fileManager.Load<TemplatesModel>(Area.User, Kind.Json, nameof(DefaultTemplate));
        if (model is TemplatesModel templates)
        {
            foreach (var group in templates.Groups)
            {
                Debug.WriteLine(group.Name);
                foreach (var template in group.Templates)
                {
                    Debug.WriteLine("      " + template.Name + "  " + template.Value);
                }
            }
        }
    }
}
