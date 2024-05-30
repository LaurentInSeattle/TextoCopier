namespace Lyt.TextoCopier.Workflow;

public sealed class GroupViewModel : Bindable<GroupView>
{
    public GroupViewModel() : this(ApplicationBase.GetRequiredService<TemplatesModel>()) { }

    private readonly TemplatesModel templatesModel;

    public GroupViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.templatesModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
        this.Templates = [];
    }

    private void Bind(string groupName)
    {
        if ( this.templatesModel.CheckGroup (groupName, out string message))
        {
            this.GroupName = groupName;
            Group group = this.templatesModel.GetGroup(groupName);
            var list = new List<TemplateViewModel>(group.Templates.Count);
            foreach (var template in group.Templates)
            {
                list.Add(new TemplateViewModel(groupName, template));
            }

            this.Templates = list;
        }

        this.Logger.Info(message);
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        var group = this.templatesModel.SelectedGroup; 
        if ( group is not null)
        {
            this.Bind(group.Name);
        }
        else
        {
            this.GroupName = "No group selected..."; 
        }
    }

    public List<TemplateViewModel> Templates { get => this.Get<List<TemplateViewModel>>()!; set => this.Set(value); }

    public string GroupName { get => this.Get<string>()!; set => this.Set(value); }
}
