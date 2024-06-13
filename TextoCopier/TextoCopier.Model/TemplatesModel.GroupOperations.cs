namespace Lyt.TextoCopier.Model;

public sealed partial class TemplatesModel
{
    public const int StringMaxLength = 64;

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

    public bool ValidateGroupForAdd(string groupName, string groupDescription, string iconName, out string message)
    {
        if ( ! this.ValidateGroupCommon(groupName, groupDescription, iconName, out message))
        {
            return false; 
        }
    
        bool fail = this.CheckGroup(groupName, out _);
        if (fail)
        {
            message = GroupAlreadyExists;
        }

        return !fail;
    }

    public bool ValidateGroupForEdit(string newGroupName, string oldGroupName, string groupDescription, string iconName, out string message)
    {
        if (!this.ValidateGroupCommon(newGroupName, groupDescription, iconName, out message))
        {
            return false;
        }

        if (newGroupName != oldGroupName)
        {
            if (this.CheckGroup(newGroupName, out _))
            {
                message = GroupAlreadyExists;
                return false; 
            }
        }

        return true;
    }

    private bool ValidateGroupCommon(string groupName, string groupDescription, string iconName, out string message)
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
            message = DescriptionIsBlank;
            return false;
        }

        if (groupName.Length > StringMaxLength)
        {
            message = GroupNameIsTooLong;
            return false;
        }

        if (groupDescription.Length > StringMaxLength)
        {
            message = DescriptionIsTooLong;
            return false;
        }

        // TODO: Check icon 
        //if (string.IsNullOrWhiteSpace(iconName))
        //{
        //    message = IconNameIsBlank;
        //    return false;
        //}

        return true;
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

    public bool EditGroup(string newGroupName, string oldGroupName, string groupDescription, string iconName, out string message)
    {
        if (!this.ValidateGroupForEdit(newGroupName, oldGroupName, groupDescription, iconName, out message))
        {
            return false;
        } 

        var group = this.GetGroup(oldGroupName);
        this.Groups.Remove(group);
        var newGroup = new Group { Name = newGroupName, Description = groupDescription, Icon = iconName, Templates = group.Templates };
        this.Groups.Add(newGroup);

        this.SelectedGroup = newGroup;
        this.NotifyUpdate(propertyName: "", methodName: nameof(this.EditGroup));

        message = string.Empty;
        this.IsDirty = true;
        return true;
    }
}
