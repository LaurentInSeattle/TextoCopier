namespace Lyt.TextoCopier.Model;

public sealed partial class TemplatesModel
{
    public const string TemplateAlreadyExists = "TemplatesModel.TemplateAlreadyExists";
    public const string NoSuchTemplate = "TemplatesModel.NoSuchTemplate";

    private static bool CheckTemplate(Group grp, string templateName, out string message)
    {
        Template? maybeTemplate =
            (from template in grp.Templates where template.Name == templateName select template).FirstOrDefault();
        bool fail = maybeTemplate is null;
        message = fail ? NoSuchTemplate : string.Empty;
        return !fail;
    }

    private static Template GetTemplate(Group grp, string templateName)
    {
        Template? maybeTemplate =
            (from template in grp.Templates where template.Name == templateName select template).FirstOrDefault();
        return maybeTemplate is null ? throw new InvalidOperationException(NoSuchTemplate) : maybeTemplate;
    }

    public bool AddTemplate(string groupName, string templateName, string templateValue, out string message)
        => this.ModelOperation(this.AddTemplateInternal, groupName, templateName, templateValue, out message);

    private bool AddTemplateInternal(Group group, string templateName, string templateValue, out string message)
    {
        List<Template> templates = group.Templates;
        var template = new Template { Name = templateName, Value = templateValue };
        templates.Add(template);

        message = string.Empty;
        this.IsDirty = true;
        this.NotifyUpdate();
        return true;
    }

    public bool DeleteTemplate(string groupName, string templateName, out string message)
        => this.ModelOperation(this.DeleteTemplateInternal, groupName, templateName, string.Empty, out message);

    private bool DeleteTemplateInternal(Group group, string templateName, string _, out string message)
    {
        bool status = TemplatesModel.CheckTemplate(group, templateName, out message);
        if (status)
        {
            Template template = TemplatesModel.GetTemplate(group, templateName);
            group.Templates.Remove(template);

            message = string.Empty;
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return status;
    }

    public bool EditTemplateValue(string groupName, string templateName, string templateValue, out string message)
        => this.ModelOperation(this.EditTemplateValueInternal, groupName, templateName, templateValue, out message);

    private bool EditTemplateValueInternal(Group group, string templateName, string templateValue, out string message)
    {
        bool status = TemplatesModel.CheckTemplate(group, templateName, out message);
        if (status)
        {
            Template template = TemplatesModel.GetTemplate(group, templateName);
            template.Value = templateValue;

            message = string.Empty;
            this.IsDirty = true;
            this.NotifyUpdate();
        }

        return status;
    }

    public bool RenameTemplate(string groupName, string templateName, string newTemplateName, out string message)
        => this.ModelOperation(this.RenameTemplateInternal, groupName, templateName, newTemplateName, out message);

    private bool RenameTemplateInternal(Group group, string templateName, string newTemplateName, out string message)
    {
        bool status = TemplatesModel.CheckTemplate(group, templateName, out message);
        if (status)
        {
            Template template = TemplatesModel.GetTemplate(group, templateName);
            bool taken = TemplatesModel.CheckTemplate(group, newTemplateName, out _);
            if (taken)
            {
                message = TemplateAlreadyExists;
                status = false;
            }
            else
            {
                template.Name = newTemplateName;
                message = string.Empty;
                this.IsDirty = true;
                this.NotifyUpdate();
            }
        }

        return status;
    }
}
