namespace Lyt.TextoCopier.Models; 

public sealed partial class TemplatesModel
{
    public bool AddTemplate(string groupName, string templateName, string templateValue, out string message)
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

    public bool DeleteTemplate(string groupName, string templateName, out string message)
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

    public bool EditTemplateValue(string groupName, string templateName, string templateValue, out string message)
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

    public bool RenameTemplate(string groupName, string templateName, string newTemplateName, out string message)
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
