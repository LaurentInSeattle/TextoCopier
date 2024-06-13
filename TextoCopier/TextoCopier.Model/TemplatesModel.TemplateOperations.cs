namespace Lyt.TextoCopier.Model;

public sealed partial class TemplatesModel
{
    private bool CheckTemplate(string groupName, string templateName, out string message)
    {
        if ( !this.CheckGroup(groupName, out message))
        {
            return false; 
        }
    
        Group grp = this.GetGroup(groupName);
        Template? maybeTemplate =
            (from template in grp.Templates where template.Name == templateName select template).FirstOrDefault();
        bool fail = maybeTemplate is null;
        message = fail ? NoSuchTemplate : string.Empty;
        return !fail;
    }

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

    public bool AddTemplate(
        string groupName, string templateName, string templateValue,
        bool isLink, bool shouldHide, out string message)
    {
        bool status = this.CheckGroup(groupName, out message);
        if (status)
        {
            Group group = this.GetGroup(groupName);
            List<Template> templates = group.Templates;
            var template = 
                new Template 
                { 
                    Name = templateName, Value = templateValue, 
                    IsLink = isLink, ShouldHide = shouldHide,
                };
            templates.Add(template);
            this.IsDirty = true;
            this.NotifyUpdate(propertyName: string.Empty, methodName: nameof(this.AddTemplate));
        }

        return status;
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

    public bool ValidateTemplateForAdd(
        string groupName, string newTemplateName, string value, out string message)
    {
        if (!this.ValidateTemplateCommon(newTemplateName, value, out message))
        {
            return false;
        }

        bool fail = this.CheckTemplate(groupName, newTemplateName, out _);
        if (fail)
        {
            message = TemplateAlreadyExists;
        }

        return !fail;
    }

    public bool ValidateTemplateForEdit(
        string groupName, string newTemplateName, string oldTemplateName, string value, out string message)
    {
        if (!this.ValidateTemplateCommon(newTemplateName, value, out message))
        {
            return false;
        }

        if (newTemplateName != oldTemplateName)
        {
            if (this.CheckTemplate(groupName, newTemplateName, out _))
            {
                message = TemplateAlreadyExists;
                return false;
            }
        }

        return true;
    }

    private bool ValidateTemplateCommon(string templateName, string value, out string message)
    {
        message = string.Empty;
        templateName = templateName.Trim();
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(templateName))
        {
            message = TemplateNameIsBlank;
            return false;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            message = TemplateValueIsBlank;
            return false;
        }

        if (templateName.Length > StringMaxLength)
        {
            message = TemplateNameIsTooLong;
            return false;
        }

        if (value.Length > StringMaxLength)
        {
            message = TemplateValueIsTooLong;
            return false;
        }

        return true;
    }

}
