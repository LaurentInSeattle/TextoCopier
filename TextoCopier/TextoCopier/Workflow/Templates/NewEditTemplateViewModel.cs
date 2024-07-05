namespace Lyt.TextoCopier.Workflow;

public sealed class NewEditTemplateViewModel : Bindable<NewEditTemplateView>
{
    private readonly IMessenger messenger;
    private readonly LocalizerModel localizer;
    private readonly TemplatesModel templatesModel;

    public NewEditTemplateViewModel(IMessenger messenger, LocalizerModel localizer, TemplatesModel templatesModel)
    {
        this.messenger = messenger;
        this.localizer = localizer;
        this.templatesModel = templatesModel;

        this.CloseCommand = new Command(this.OnClose);
        this.SaveCommand = new Command(this.OnSave);
        this.SelectedGroup = new();
    }

    public Group SelectedGroup { get; private set; }

    public Template? EditedTemplate { get; private set; }

    public bool IsEditing => this.EditedTemplate != null;

    public override void Activate(object? activationParameter)
    {
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
        if (activationParameter is Template template)
        {
            this.EditedTemplate = template;
            this.Name = template.Name;
            this.Value = template.Value;
            this.IsHidden = template.ShouldHide;
            this.IsWebLink = template.IsLink;
            this.TemplateTitle = this.Name;
            string format = this.localizer.Lookup("NewEditTemplateView.Changed.Format");
            this.GroupDescription = string.Format(format, groupName);
        }
        else
        {
            this.EditedTemplate = null;
            this.Name = string.Empty;
            this.Value = string.Empty;
            this.IsHidden = false;
            this.IsWebLink = false;
            this.TemplateTitle = this.localizer.Lookup("Group.NewTemplateLong"); ;
            string format = this.localizer.Lookup("NewEditTemplateView.Added.Format");
            this.GroupDescription = string.Format(format, groupName);
        }

        this.OnEditing();
    }

    public override void Deactivate() => this.EditedTemplate = null;

    private void OnSave(object? _)
    {
        if (this.Save(out string message))
        {
            this.OnClose(_);
        }
        else
        {
            this.ValidationMessage = this.localizer.Lookup(message);
            this.SaveButtonIsDisabled = true;
        }
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));

    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : this.localizer.Lookup(message);
        this.SaveButtonIsDisabled = !validated;
    }

    private bool Validate(out string message)
    {
        string groupName = this.SelectedGroup.Name;
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
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
        string newName = this.Name.Trim();
        string value = this.Value.Trim();
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedTemplate is not null
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

    public string TemplateTitle { get => this.Get<string>()!; set => this.Set(value); }

    public string GroupDescription { get => this.Get<string>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Value { get => this.Get<string>()!; set => this.Set(value); }

    public bool IsWebLink { get => this.Get<bool>()!; set => this.Set(value); }

    public bool IsHidden { get => this.Get<bool>()!; set => this.Set(value); }

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }

    public bool SaveButtonIsDisabled
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.View.SaveButton.IsDisabled = value;
        }
    }

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
