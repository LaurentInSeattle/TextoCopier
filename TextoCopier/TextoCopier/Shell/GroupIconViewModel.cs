namespace Lyt.TextoCopier.Shell;

public sealed partial class GroupIconViewModel : ViewModel<GroupIconView>
{
    private readonly string groupName;

    public GroupIconViewModel(Group group, SelectionGroup selectionGroup, bool selected)
    {
        this.groupName = group.Name;

        this.IconGlyphSource = group.Icon;
        this.IconText = group.Name;
        this.SelectionGroup = selectionGroup;
        this.IsSelected = selected;
    }

    [RelayCommand]
    public void OnGroup()
    {
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        if (! model.SelectGroup(this.groupName, out string message))
        {
            this.Logger.Warning("Failed to select group: " + message);
        }

        this.Messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.Group)); 
    }

    [ObservableProperty]
    private string iconGlyphSource;

    [ObservableProperty]
    private string iconText;

    [ObservableProperty]
    private SelectionGroup selectionGroup;

    [ObservableProperty]
    private bool isSelected;
}
