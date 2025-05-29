namespace Lyt.TextoCopier.Workflow.Templates;

public sealed partial class NewEditTemplateViewModel(
    TemplatesModel templatesModel) : ViewModel<NewEditTemplateView>
{
    private readonly TemplatesModel templatesModel = templatesModel;

    [ObservableProperty]
    private string? templateTitle;

    [ObservableProperty]
    private string? groupDescription;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? value;

    [ObservableProperty]
    private bool isWebLink;

    [ObservableProperty]
    private bool isHidden;

    [ObservableProperty]
    private string? validationMessage;

    [ObservableProperty]
    private bool saveButtonIsDisabled;

    public Group SelectedGroup { get; private set; } = new();

    public Template? EditedTemplate { get; private set; }

    public bool IsEditing => this.EditedTemplate != null;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        if (this.templatesModel.SelectedGroup is null)
        {
            string message = TemplatesModel.NoSuchGroup;
            this.Logger.Info("model.SelectedGroup is null: " + message);
            return;
        }
        else
        {
            this.SelectedGroup = this.templatesModel.SelectedGroup;
        }

        string groupName = this.SelectedGroup.Name;
        if (activationParameters is Template template)
        {
            this.EditedTemplate = template;
            this.Name = template.Name;
            this.Value = template.Value;
            this.IsHidden = template.ShouldHide;
            this.IsWebLink = template.IsLink;
            this.TemplateTitle = this.Name;
            string format = this.Localizer.Lookup("NewEditTemplateView.Changed.Format");
            this.GroupDescription = string.Format(format, groupName);
        }
        else
        {
            this.EditedTemplate = null;
            this.Name = string.Empty;
            this.Value = string.Empty;
            this.IsHidden = false;
            this.IsWebLink = false;
            this.TemplateTitle = this.Localizer.Lookup("Group.NewTemplateLong"); ;
            string format = this.Localizer.Lookup("NewEditTemplateView.Added.Format");
            this.GroupDescription = string.Format(format, groupName);
        }

        this.OnEditing();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.EditedTemplate = null;
    }

    [RelayCommand]
    public void OnSave()
    {
        if (this.Save(out string message))
        {
            this.OnClose();
        }
        else
        {
            this.ValidationMessage = this.Localizer.Lookup(message);
            this.SaveButtonIsDisabled = true;
        }
    }

    [RelayCommand]
    public void OnClose()
        => this.Messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));

    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : this.Localizer.Lookup(message);
        this.SaveButtonIsDisabled = !validated;
    }

    private bool Validate(out string message)
    {
        string groupName = this.SelectedGroup.Name;
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedTemplate is not null
            if (this.EditedTemplate is null || string.IsNullOrWhiteSpace(this.EditedTemplate.Name))
            {
                throw new InvalidOperationException("Should never happen");
            }

            return this.templatesModel.ValidateTemplateForEdit(groupName, this.Name, this.EditedTemplate!.Name, this.Value, out message);
        }
        else
        {
            return this.templatesModel.ValidateTemplateForAdd(groupName, this.Name, this.Value, out message);
        }
    }

    private bool Save(out string message)
    {
        if (!this.Validate(out message))
        {
            return false;
        }

        // Save to model 
        string groupName = this.SelectedGroup.Name;

        if (string.IsNullOrWhiteSpace(this.Name) ||
            string.IsNullOrWhiteSpace(this.Value))
        {
            throw new InvalidOperationException("Should never happen");
        }

        string newName = this.Name.Trim();
        string value = this.Value.Trim();
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedTemplate is not null
            if (this.EditedTemplate is null || string.IsNullOrWhiteSpace(this.EditedTemplate.Name))
            {
                throw new InvalidOperationException("Should never happen");
            }

            string oldName = this.EditedTemplate!.Name.Trim();
            if (this.templatesModel.EditTemplate(groupName, newName, oldName, value, this.IsWebLink, this.IsHidden, out message))
            {
                return true;
            }
        }
        else
        {
            if (this.templatesModel.AddTemplate(groupName, newName, value, this.IsWebLink, this.IsHidden, out message))
            {
                return true;
            }
        }

        return false;
    }

    // STill Needed ? 
    //
    //public bool SaveButtonIsDisabled;
    //{
    //    get => this.Get<bool>();
    //    set
    //    {
    //        this.Set(value);
    //        this.View.SaveButton.IsDisabled = value;
    //    }
    //}
}
