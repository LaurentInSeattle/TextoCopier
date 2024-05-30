namespace Lyt.TextoCopier.Shell;

public class GroupIconViewModel : Bindable<GroupIconView>
{
    private readonly string groupName;

    public GroupIconViewModel(Group group, SelectionGroup selectionGroup, bool selected)
    {
        this.groupName = group.Name;
        this.IconGlyphSource = group.Icon;
        this.IconText = group.Name;
        this.SelectionGroup = selectionGroup;
        this.IsSelected = selected;
        this.GroupCommand = new Command(this.OnGroup);
        Schedule.OnUiThread(200, this.OnLoaded, DispatcherPriority.Normal);
    }

    protected void OnLoaded()
    {
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        var group = model.GetGroup(this.groupName);
        this.IconGlyphSource = group.Icon;
        //this.View!.Icon.GlyphSource = group.Icon;
    }

    private void OnGroup(object? _)
    {
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
    }

    public string IconGlyphSource { get => this.Get<string>()!; set => this.Set(value); }

    public string IconText { get => this.Get<string>()!; set => this.Set(value); }

    public SelectionGroup SelectionGroup { get => this.Get<SelectionGroup>()!; set => this.Set(value); }

    public bool IsSelected { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand GroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
