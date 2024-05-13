namespace Lyt.TextoCopier.Models;

public sealed class TemplatesModel : ModelBase
{
    public override Task Initialize() { return Task.CompletedTask; }

    public override Task Shutdown() { return Task.CompletedTask; }

    public List<Group> Groups { get; set; } = [];

    public static TemplatesModel DefaultTemplate =
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
                        new Template { Name = "Email" , Value = "Someone@domain.com" },
                        new Template { Name = "First" , Value = "Jane" },
                        new Template { Name = "Last" , Value = "Doe" },
                        new Template { Name = "Full Name" , Value = "{First} {Last}" },
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

    [Conditional("DEBUG")]
    private void TestJSonSaveLoad()
    {
        FileManagerModel fileManager = App.GetRequiredService<FileManagerModel>();
        fileManager.Save(
            FileManagerModel.Area.User, FileManagerModel.Kind.Json, "DefaultTemplate", TemplatesModel.DefaultTemplate);
        var model = fileManager.Load<TemplatesModel>(FileManagerModel.Area.User, FileManagerModel.Kind.Json, "DefaultTemplate");
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
