namespace Lyt.TextoCopier.Workflow;

public sealed class GroupViewModel : Bindable<GroupView>
{
    public GroupViewModel() : this(ApplicationBase.GetRequiredService<TemplatesModel>()) { }

    private readonly TemplatesModel templatesModel;
    private readonly IMessenger messenger;

    public GroupViewModel(TemplatesModel templatesModel)
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.templatesModel = templatesModel;
        this.templatesModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
        this.Templates = [];
        this.NewTemplateCommand = new Command(this.OnNewTemplate);
    }

    private void OnNewTemplate(object? _)
    {
        this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.StaticView.NewTemplate));
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
            this.GroupDescription = string.Empty;
        }
    }

    public List<TemplateViewModel> Templates { get => this.Get<List<TemplateViewModel>>()!; set => this.Set(value); }

    public string GroupName { get => this.Get<string>()!; set => this.Set(value); }

    public string GroupDescription { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand NewTemplateCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

}
