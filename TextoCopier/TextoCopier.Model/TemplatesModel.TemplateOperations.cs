using System.Runtime.CompilerServices;

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

    public bool EditTemplate(
        string groupName, string newName, string oldName, string value, bool isWebLink, bool isHidden, out string message)
    {
        bool status = this.CheckGroup(groupName, out message);
        if (status)
        {
            Group group = this.GetGroup(groupName);
            List<Template> templates = group.Templates;
            Template oldTemplate = TemplatesModel.GetTemplate(group, oldName);
            group.Templates.Remove(oldTemplate);
            var newTemplate =
                new Template
                {
                    Name = newName,
                    Value = value,
                    IsLink = isWebLink,
                    ShouldHide = isHidden,
                };
            templates.Add(newTemplate);
            this.IsDirty = true;
            this.NotifyUpdate(propertyName: string.Empty, methodName: nameof(this.EditTemplate));
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

    public bool ValidateTemplateForAdd(
        string groupName, string? newTemplateName, string? value, out string message)
    {
        if (!this.ValidateTemplateCommon(newTemplateName, value, out message))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(newTemplateName))
        {
            throw new InvalidOperationException("Should never happen");
        }

        bool fail = this.CheckTemplate(groupName, newTemplateName, out _);
        if (fail)
        {
            message = TemplateAlreadyExists;
        }

        return !fail;
    }

    public bool ValidateTemplateForEdit(
        string groupName, string? newTemplateName, string oldTemplateName, string? value, out string message)
    {
        if (!this.ValidateTemplateCommon(newTemplateName, value, out message))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(newTemplateName))
        {
            throw new InvalidOperationException("Should never happen");
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

    private bool ValidateTemplateCommon(string? templateName, string? value, out string message)
    {
        message = string.Empty;

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

        templateName = templateName.Trim();
        value = value.Trim();
        if (templateName.Length > StringMaxLength)
        {
            message = TemplateNameIsTooLong;
            return false;
        }

        if (value.Length > LongStringMaxLength)
        {
            message = TemplateValueIsTooLong;
            return false;
        }

        return true;
    }

}
