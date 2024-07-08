namespace Lyt.TextoCopier.Model;

using static FileManagerModel;

public sealed partial class TemplatesModel : ModelBase
{
    public const string DefaultLanguage = "it-IT";

    #region Default Template

    private static readonly TemplatesModel DefaultTemplate =
        new()
        {
            Language = DefaultLanguage ,
            Groups =
            [
                new Group
                {
                    Name = "Personal Items",
                    Description = "Personal General Information",
                    Icon = "person",
                    Templates =
                    [
                        new Template { Name = "Email" , Value = "john.doe@email.com" },
                        new Template { Name = "Git Password" , Value = "john.4.git", ShouldHide = true },
                        new Template { Name = "Phone" , Value = "+1 (206) 420-0666" },
                    ]
                },
                new Group
                {
                    Name = "Jobs on LinkedIn",
                    Description = "Messaging on Linked In",
                    Icon = "link",
                    Templates =
                    [
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
                            Name = "Sig - Regards" , Value = "Best regards,    \nJohn"
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
                        new Template { Name = "Nome di Schermo" , Value = "John" },
                        new Template { Name = "Kahoot" , Value = "https://kahoot.it/", IsLink = true},
                        new Template
                        {
                            Name = "Introduzione" ,
                            Value = "Ciao! Mi chiamo John e vivo in Pleasanton, una piccola citta nella periferia di San Francisco, California."
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
            ]
        };

    #endregion Default Template

    private const string TemplatesModelFilename = "Templates";

    private readonly FileManagerModel fileManager;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE0021 // Use expression body for constructor 
    public TemplatesModel() : base ( null, null)
    {
        // Empty CTOR required for deserialization 
        this.ShouldAutoSave = false;
    }
#pragma warning restore IDE0021
#pragma warning restore CS8625 
#pragma warning restore CS8618

    public TemplatesModel(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
    }

    // Serialized -  No model changed event
    [JsonRequired]
    public string Language { get; set; } = TemplatesModel.DefaultLanguage;

    // Serialized -  No model changed event
    [JsonRequired]
    public List<Group> Groups { get; set; } = [];

    [JsonIgnore]
    // Not serialized -  With model changed event
    public Group? SelectedGroup { get => this.Get<Group?>(); set => this.Set(value); }

    [JsonIgnore]
    // Not serialized - No model changed event
    public List<string> AvailableIcons { get; set; } = [];


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
            if (!this.fileManager.Exists(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename))
            {
                this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, TemplatesModel.DefaultTemplate);
            }

            TemplatesModel model =
                this.fileManager.Load<TemplatesModel>(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename);

            // Copy all properties with attribute [JsonRequired]
            base.CopyJSonRequiredProperties<TemplatesModel>(model);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            string msg = "Failed to load TemplatesModel from " + TemplatesModel.TemplatesModelFilename;
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
