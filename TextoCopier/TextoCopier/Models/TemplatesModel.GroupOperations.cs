namespace Lyt.TextoCopier.Models;

public sealed partial class TemplatesModel
{
    public bool AddGroup(string groupName, string groupDescription, string iconName, out string message)
    {
        bool status = true;
        message = string.Empty;

        if (status)
        {
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return status;
    }

    public bool DeleteGroup(string groupName, out string message)
    {
        bool status = true;
        message = string.Empty;

        if (status)
        {
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return status;
    }

    public bool RenameGroup(string groupName, string newGroupName, out string message)
    {
        bool status = true;
        message = string.Empty;

        if (status)
        {
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return status;
    }

    public bool EditGroupDescription(string groupName, string newGroupDescription, out string message)
    {
        bool status = true;
        message = string.Empty;

        if (status)
        {
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return status;
    }

    public bool EditGroupIcon(string groupName, string newIconName, out string message)
    {
        bool status = true;
        message = string.Empty;

        if (status)
        {
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return status;
    }
}
