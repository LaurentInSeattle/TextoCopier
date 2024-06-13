namespace Lyt.TextoCopier.Model;

public partial class TemplatesModel
{
    public const string GroupAlreadyExists = "TemplatesModel.GroupAlreadyExists";
    public const string NoSuchGroup = "TemplatesModel.NoSuchGroup";

    public const string GroupNameIsBlank = "TemplatesModel.GroupNameIsBlank"; // "Group name cannot be left empty or blank.";
    public const string DescriptionIsBlank = "TemplatesModel.DescriptionIsBlank"; // "Group description cannot be left empty or blank.";
    public const string IconNameIsBlank = "TemplatesModel.IconNameIsBlank";  // "An icon mane is required. ";
    public const string GroupNameIsTooLong = "TemplatesModel.GroupNameIsTooLong";
    public const string DescriptionIsTooLong = "TemplatesModel.DescriptionIsTooLong";
    public const string IconNotAvailable = "TemplatesModel.IconNotAvailable";

    public const string TemplateAlreadyExists = "TemplatesModel.TemplateAlreadyExists";
    public const string NoSuchTemplate = "TemplatesModel.NoSuchTemplate";

    public const string TemplateNameIsBlank = "TemplatesModel.TemplateNameIsBlank";
    public const string TemplateValueIsBlank = "TemplatesModel.TemplateValueIsBlank";
    public const string TemplateNameIsTooLong = "TemplatesModel.TemplateNameIsTooLong";
    public const string TemplateValueIsTooLong = "TemplatesModel.TemplateValueIsTooLong";
}
