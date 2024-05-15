namespace Lyt.TextoCopier.Models;

public sealed partial class TemplatesModel
{
    public const string GroupAlreadyExists = "TemplatesModel.GroupAlreadyExists";
    public const string NoSuchGroup = "TemplatesModel.NoSuchGroup"; 

    private bool CheckGroup(string groupName, out string message)
    {
        message = string.Empty;
        var group = (from grp in this.Groups where grp.Name == groupName select grp).FirstOrDefault();
        bool fail = group is null;
        if (fail)
        {
            message = NoSuchGroup;
        }

        return ! fail;
    }

    private Group GetGroup(string groupName)
    {
        var group = (from grp in this.Groups where grp.Name == groupName select grp).FirstOrDefault();
        return group is null ? throw new InvalidOperationException(NoSuchGroup) : group;
    }


    public bool AddGroup(string groupName, string groupDescription, string iconName, out string message)
    {
        bool fail = this.CheckGroup(groupName, out _);
        if (fail)
        {
            message = GroupAlreadyExists;
        }
        else
        {
            var newGroup = new Group { Name = groupName, Description = groupDescription, Icon = iconName, Templates = [] };
            this.Groups.Add(newGroup);

            message = string.Empty;
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return ! fail;
    }

    public bool DeleteGroup(string groupName, out string message)
        => this.ModelOperation(this.DeleteGroupInternal, groupName, string.Empty, string.Empty, out message);

    private bool DeleteGroupInternal(Group group, string _1, string _2, out string message)
    {
        this.Groups.Remove(group);

        message = string.Empty;
        this.IsDirty = true;
        this.NotifyUpdate();

        return true;
    }

    public bool RenameGroup(string groupName, string newGroupName, out string message)
        => this.ModelOperation(this.RenameGroupInternal, groupName, newGroupName, string.Empty, out message);

    private bool RenameGroupInternal(Group group, string newGroupName, string _, out string message)
    {
        bool taken = this.CheckGroup( newGroupName, out string _ );
        if (taken)
        {
            message = GroupAlreadyExists;
        }
        else
        {
            group.Name = newGroupName;

            message = string.Empty;
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return ! taken;
    }

    public bool EditGroupDescription(string groupName, string newGroupDescription, out string message)
        => this.ModelOperation(this.EditGroupDescriptionInternal, groupName, newGroupDescription, string.Empty, out message);

    private bool EditGroupDescriptionInternal(Group group, string newGroupDescription, string _, out string message)
    {
        group.Description = newGroupDescription;

        message = string.Empty;
        this.IsDirty = true;
        this.NotifyUpdate();
        return true;
    }

    public bool EditGroupIcon(string groupName, string newIconName, out string message)
        => this.ModelOperation(this.EditGroupIconInternal, groupName, newIconName, string.Empty, out message);

    public bool EditGroupIconInternal(Group group, string newIconName, string _, out string message)
    {
        group.Icon = newIconName;

        message = string.Empty;
        this.IsDirty = true;
        this.NotifyUpdate();
        return true;
    }
}
