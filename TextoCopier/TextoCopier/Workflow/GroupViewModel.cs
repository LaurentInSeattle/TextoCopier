namespace Lyt.TextoCopier.Workflow; 

public sealed class GroupViewModel : Bindable<GroupView>
{
    private readonly TemplatesModel templatesModel;

    public GroupViewModel(TemplatesModel templatesModel)
    {
        this.templatesModel = templatesModel;
        this.templatesModel.SubscribeToUpdates(this.OnModelUpdated, withUiDispatch: true);
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
    }
}
