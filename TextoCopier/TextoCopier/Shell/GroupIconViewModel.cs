namespace Lyt.TextoCopier.Shell;

public class GroupIconViewModel : Bindable<GroupIconView>
{
    private readonly IMessenger messenger;
    private readonly string groupName;

    public GroupIconViewModel(Group group, SelectionGroup selectionGroup, bool selected)
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.groupName = group.Name;

        base.DisablePropertyChangedLogging = true;

        this.IconGlyphSource = group.Icon;
        this.IconText = group.Name;
        this.SelectionGroup = selectionGroup;
        this.IsSelected = selected;
        this.GroupCommand = new Command(this.OnGroup);
    }

    private void OnGroup(object? _)
    {
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        if (! model.SelectGroup(this.groupName, out string message))
        {
            this.Logger.Warning("Failed to select group: " + message);
        }

        this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.StaticView.Group)); 
    }

    public string IconGlyphSource { get => this.Get<string>()!; set => this.Set(value); }

    public string IconText { get => this.Get<string>()!; set => this.Set(value); }

    public SelectionGroup SelectionGroup { get => this.Get<SelectionGroup>()!; set => this.Set(value); }

    public bool IsSelected { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand GroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
