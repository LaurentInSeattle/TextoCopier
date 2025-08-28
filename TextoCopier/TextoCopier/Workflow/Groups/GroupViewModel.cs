namespace Lyt.TextoCopier.Workflow;

public sealed partial class GroupViewModel : ViewModel<GroupView>, IRecipient<ModelUpdateMessage>
{
    private readonly TemplatesModel templatesModel;

    [ObservableProperty]
    private List<TemplateViewModel> templates;

    [ObservableProperty]
    private string? groupName;

    [ObservableProperty]
    private string? groupDescription;

    public GroupViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.Templates = [];
        this.Subscribe<ModelUpdateMessage>();
    }

    [RelayCommand]
    public void OnNewTemplate(object? _)
        => new ViewActivationMessage(ViewActivationMessage.ActivatedView.NewTemplate).Publish();

    public void Receive(ModelUpdateMessage message)
    {
        var group = this.templatesModel.SelectedGroup;
        if (group is not null)
        {
            this.Bind(group.Name);
        }
        else
        {
            this.GroupName = this.Localizer.Lookup("Group.NoSelection");
            this.GroupDescription = string.Empty;
        }
    }

    private void Bind(string groupName)
    {
        if ( this.templatesModel.CheckGroup (groupName, out string message))
        {
            Group group = this.templatesModel.GetGroup(groupName);
            var list = new List<TemplateViewModel>(group.Templates.Count);
            var grid = this.View.InnerGrid; 
            foreach (var template in group.Templates)
            {
                list.Add(new TemplateViewModel(groupName, template, grid));
            }

            this.Templates = list;
            this.GroupName = groupName;
            this.GroupDescription = group.Description; 
        }

        this.Logger.Info(message);
    }
}
