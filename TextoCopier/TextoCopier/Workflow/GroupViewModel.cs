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
        this.Bind("Italian on Zoom"); 
    }

    private void Bind(string groupName)
    {
        this.Logger.Info("Binding");
        if ( this.templatesModel.CheckGroup (groupName, out string message))
        {
            this.Logger.Info("Binding to " + groupName);
            Group group = this.templatesModel.GetGroup(groupName);
            var list = new List<TemplateViewModel>(group.Templates.Count);
            foreach (var template in group.Templates)
            {
                list.Add(new TemplateViewModel(groupName, template));
            }

            this.Templates = list;
            this.Logger.Info("Templates: " + group.Templates.Count);
        }

        this.Logger.Info(message);
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
    }

    public List<TemplateViewModel> Templates { get => this.Get<List<TemplateViewModel>>()!; set => this.Set(value); }
}
