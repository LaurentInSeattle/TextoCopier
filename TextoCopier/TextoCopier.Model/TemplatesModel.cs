namespace Lyt.TextoCopier.Model;

using static FileManagerModel;

public sealed partial class TemplatesModel : ModelBase
{
    #region Default Template

    private static readonly TemplatesModel DefaultTemplate =
        new()
        {
            Groups =
            [
                new Group
                {
                    Name = "Personal Items",
                    Description = "Personal General Information",
                    Icon = "person",
                    Templates =
                    [
                        new Template { Name = "Email" , Value = "ly.testud@outlook.com" },
                        new Template { Name = "Git Password" , Value = "laurent.4.git", ShouldHide = true },
                        new Template { Name = "First" , Value = "Laurent" },
                        new Template { Name = "Middle" , Value = "Yves" },
                        new Template { Name = "Last" , Value = "Testud" },
                        new Template { Name = "Full Name" , Value = "Laurent Y. Testud" },
                        new Template { Name = "Phone" , Value = "+1 (206) 619-7238" },
                        new Template { Name = "Phone - Numbers only" , Value = "2066197238" },
                        // LATER 
                        // new Template { Name = "Full Name" , Value = "{First} {Last}" },
                    ]
                },
                new Group
                {
                    Name = "Jobs on LinkedIn",
                    Description = "Messaging on Linked In",
                    Icon = "link",
                    Templates =
                    [
                        new Template { Name = "Email" , Value = "ly.testud@outlook.com" },
                        new Template { Name = "Password" , Value = "Faster.Dude", ShouldHide = true },
                        new Template 
                        { 
                            Name = "Thanks + Yes" , 
                            Value = "Hi <name>, thank you for reaching out. Yes, this sounds like a very good opportunity and I would like to learn more about it."
                        },
                        new Template
                        {
                            Name = "Attachments" , Value = "I have attached my most recent resume and a visual presentation to this message."
                        },
                        new Template
                        {
                            Name = "Sig - Regards" , Value = "Best regards,    \nLaurent"
                        },
                    ]
                },
                new Group
                {
                    Name = "Italiano su Zoom",
                    Description = "Frasi, Codici e ID per Zoom",
                    Icon = "flag",
                    Templates =
                    [
                        new Template { Name = "Incontro ID" , Value = "84240413222" },
                        new Template { Name = "Incontro Codice" , Value = "717140" },
                        new Template { Name = "Nome di Schermo" , Value = "Enzo ~ Laurent" },
                        new Template { Name = "Kahoot" , Value = "https://kahoot.it/", IsLink = true},
                        new Template
                        {
                            Name = "Introduzione" ,
                            Value = "Ciao! Mi chiamo Lorenzo e vivo in Pleasanton, una piccola citta nella periferia di San Francisco, California."
                        },
                        new Template
                        {
                            Name = "Promemoria" , Value = "Non dimenticare di presentarti e dirci dove vivi o qualunque cosa vorresti..."
                        },
                    ]
                },
                new Group
                {
                    Name = "C# Code Snippets",
                    Description = "Code blocks for C#",
                    Icon = "code_block",
                    Templates =
                    [
                        new Template { Name = "if" , Value =
@"
            if (status)
            {
            }
" },
                        new Template { Name = "if-else" , Value =
@"
            if (status)
            {
            }
            else
            { 
            } 
" },
                    ]
                },
                new Group
                {
                    Name = "Test",
                    Description = "Testing Deletion",
                    Icon = "dismiss",
                    Templates =
                    [
                        new Template { Name = "Email" , Value = "ly.testud@outlook.com" },
                        new Template { Name = "Password" , Value = "Faster.Dude", ShouldHide = true },
                    ]
                },
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
    #endregion Default Template

    private const string TemplatesModelFilename = "Templates";

    private readonly FileManagerModel fileManager;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public TemplatesModel() : base ( null, null)
    {
        // Empty CTOR required for deserialization 
        this.ShouldAutoSave = false;
    }
#pragma warning restore CS8625 
#pragma warning restore CS8618

    public TemplatesModel(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        // ???? 
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        // ???? 
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
    }

    public override async Task Initialize() => await this.Load();

    public override async Task Shutdown()
    {
        if ( this.IsDirty)
        {
            await this.Save();  
        }
    }
    
    public List<Group> Groups { get; set; } = [];

    public Group? SelectedGroup { get => this.Get<Group?>(); set => this.Set(value); }

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
        // Null check is needed !
        // If the File Manager is null we are currently loading the model and activating properties on a second instance 
        // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
        if (this.fileManager is not null)
        {
            this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, this);
            base.Save();
        } 

        return Task.CompletedTask;
    }

    public bool SelectGroup(string groupName, out string message)
    {
        bool status = this.CheckGroup(groupName, out message);
        if (status)
        {
            Group group = this.GetGroup(groupName);
            if (status)
            {
                this.SelectedGroup = group;
            }
        }

        return status;
    }

    private delegate bool ModelOperationDelegate(Group group, string parameter1, string parameter2, out string message);

    private bool ModelOperation(
        ModelOperationDelegate modelOperation, string groupName, 
        string parameter1, string parameter2, 
        out string message,
        [CallerMemberName] string callerMemberName = "")
    {
        bool status = this.CheckGroup(groupName, out message);
        if (status)
        {
            Group group = this.GetGroup(groupName);
            status = modelOperation(group, parameter1, parameter2, out message);
            if (status)
            {
                this.IsDirty = true;
                this.NotifyUpdate(propertyName:string.Empty, methodName: callerMemberName);
            }
        }

        return status;
    }
}
