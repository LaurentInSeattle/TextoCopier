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
                    Name = "Personal Items",
                    Description = "Personal Items",
                    Icon = "person_regular",
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
                    Icon = "video_person_regular",
                    Templates =
                    [
                        new Template { Name = "Meeting Id" , Value = "842 4041 3222" },
                        new Template { Name = "Meeting Code" , Value = "717140" },
                        new Template { Name = "Screen Name" , Value = "Enzo ~ Laurent" },
                        new Template { Name = "Kahoot" , Value = "https://kahoot.it/" },
                    ]
                }
            ]
        };

    /*
     copy_regular
     edit_regular
    clipboard_paste_regular
    delete_forever_regular
    delete_regular
    image_add_regular
    folder_add_regular
    text_add_regular
    mail_regular
    add_regular
    add_circle_regular
    settings_regular
    home_regular
    info_regular
    question_regular
    question_circle_regular
    warning_regular
    checkmark_regular
    checkmark_square_regular
    checkbox_checked_regular
    people_community_regular
    globe_regular
    heart_regular

emoji_regular
emoji_sad_regular
emoji_angry_regular
emoji_laugh_regular
emoji_meh_regular
emoji_surprise_regular


    * */


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
#pragma warning disable IDE0051 // Remove unused private members
    private static void TestJSonSaveLoad()
#pragma warning restore IDE0051 // Remove unused private members
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
