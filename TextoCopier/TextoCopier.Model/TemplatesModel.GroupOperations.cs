namespace Lyt.TextoCopier.Model;

public sealed partial class TemplatesModel
{
    public const int StringMaxLength = 64;

    public const string GroupAlreadyExists = "TemplatesModel.GroupAlreadyExists";
    public const string NoSuchGroup = "TemplatesModel.NoSuchGroup";
    public const string GroupNameIsBlank = "TemplatesModel.GroupNameIsBlank"; // "Group name cannot be left empty or blank.";
    public const string DescrptionIsBlank = "TemplatesModel.DescriptionIsBlank"; // "Group description cannot be left empty or blank.";
    public const string IconNameIsBlank = "TemplatesModel.IconNameIsBlank";  // "An icon mane is required. ";
    public const string GroupNameIsTooLong = "TemplatesModel.GroupNameIsTooLong"; 
    public const string DescrptionIsTooLong = "TemplatesModel.DescriptionIsTooLong"; 
    public const string IconNotAvailable = "TemplatesModel.IconNotAvailable";  

    public bool CheckGroup(string groupName, out string message)
    {
        message = string.Empty;
        var group = (from grp in this.Groups where grp.Name == groupName select grp).FirstOrDefault();
        bool fail = group is null;
        if (fail)
        {
            message = NoSuchGroup;
        }

        return !fail;
    }

    public Group GetGroup(string groupName)
    {
        var group = (from grp in this.Groups where grp.Name == groupName select grp).FirstOrDefault();
        return group is null ? throw new InvalidOperationException(NoSuchGroup) : group;
    }

    public bool ValidateGroup(bool isEditing, string groupName, string groupDescription, string iconName, out string message)
    {
        message = string.Empty;
        groupName = groupName.Trim();
        groupDescription = groupDescription.Trim(); 
        iconName = iconName.Trim();

        if (string.IsNullOrWhiteSpace(groupName))
        {
            message = GroupNameIsBlank;
            return false;
        }

        if (string.IsNullOrWhiteSpace(groupDescription))
        {
            message = DescrptionIsBlank;
            return false;
        }

        if (groupName.Length > StringMaxLength)
        {
            message = GroupNameIsTooLong;
            return false;
        }

        if (groupDescription.Length > StringMaxLength)
        {
            message = DescrptionIsBlank;
            return false;
        }

        // TODO: Check icon 
        //if (string.IsNullOrWhiteSpace(iconName))
        //{
        //    message = IconNameIsBlank;
        //    return false;
        //}

        bool fail = false;
        if (isEditing)
        {
            fail = this.CheckGroup(groupName, out _);
            if (fail)
            {
                message = GroupAlreadyExists;
            }
        }
        else
        {
            fail = this.CheckGroup(groupName, out _);
            if (fail)
            {
                message = GroupAlreadyExists;
            }
        }

        return !fail;
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
            this.SelectedGroup = newGroup;
            this.NotifyUpdate(propertyName: "", methodName: nameof(this.AddGroup));
        }

        return !fail;
    }

    public bool DeleteGroup(string groupName, out string message)
        => this.ModelOperation(this.DeleteGroupInternal, groupName, string.Empty, string.Empty, out message);

    private bool DeleteGroupInternal(Group group, string _1, string _2, out string message)
    {
        bool wasSelected = group == this.SelectedGroup;
        this.Groups.Remove(group);
        message = string.Empty;
        this.IsDirty = true;

        if (wasSelected)
        {
            this.SelectedGroup = null;
        }

        return true;
    }

    public bool RenameGroup(string groupName, string newGroupName, out string message)
        => this.ModelOperation(this.RenameGroupInternal, groupName, newGroupName, string.Empty, out message);

    private bool RenameGroupInternal(Group group, string newGroupName, string _, out string message)
    {
        bool taken = this.CheckGroup(newGroupName, out string _);
        if (taken)
        {
            message = GroupAlreadyExists;
        }
        else
        {
            group.Name = newGroupName;
            message = string.Empty;
            this.IsDirty = true;
        }

        return !taken;
    }

    public bool EditGroupDescription(string groupName, string newGroupDescription, out string message)
        => this.ModelOperation(this.EditGroupDescriptionInternal, groupName, newGroupDescription, string.Empty, out message);

    private bool EditGroupDescriptionInternal(Group group, string newGroupDescription, string _, out string message)
    {
        group.Description = newGroupDescription;

        message = string.Empty;
        this.IsDirty = true;
        return true;
    }

    public bool EditGroupIcon(string groupName, string newIconName, out string message)
        => this.ModelOperation(this.EditGroupIconInternal, groupName, newIconName, string.Empty, out message);

    public bool EditGroupIconInternal(Group group, string newIconName, string _, out string message)
    {
        group.Icon = newIconName;

        message = string.Empty;
        this.IsDirty = true;
        return true;
    }
}
