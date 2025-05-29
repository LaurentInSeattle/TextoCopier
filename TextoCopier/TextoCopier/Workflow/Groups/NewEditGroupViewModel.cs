namespace Lyt.TextoCopier.Workflow;

public sealed partial class NewEditGroupViewModel(TemplatesModel templatesModel) 
    : ViewModel<NewEditGroupView>
{
    private readonly TemplatesModel templatesModel = templatesModel;

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private string? icon;

    [ObservableProperty]
    private List<string>? iconNames;

    [ObservableProperty]
    private string? validationMessage;

    [ObservableProperty]
    private bool saveButtonIsDisabled;

    public Group? EditedGroup { get; private set; }

    public bool IsEditing => this.EditedGroup != null;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        if (activationParameters is Group group)
        {
            this.EditedGroup = group;
            this.Name = group.Name;
            this.Description = group.Description;
            this.Icon = group.Icon;
            this.Title = this.Name;  
        }
        else
        {
            this.EditedGroup = null;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Icon = string.Empty;
            this.Title = this.Localizer.Lookup("Shell.NewGroupLong"); 

        }

        this.IconNames = this.templatesModel.AvailableIcons; 
        this.OnEditing();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.EditedGroup = null;
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
        if ( string.IsNullOrWhiteSpace(this.Name))
        {
            message = string.Empty;
            return false;
        }

        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
            if (this.EditedGroup is null || string.IsNullOrWhiteSpace(this.EditedGroup.Name))
            {
                throw new InvalidOperationException("Should never happen");
            }

            return this.templatesModel.ValidateGroupForEdit(this.Name, this.EditedGroup.Name, this.Description, this.Icon, out message);
        }
        else
        {
            return this.templatesModel.ValidateGroupForAdd(this.Name, this.Description, this.Icon, out message);
        }
    }

    private bool Save(out string message)
    {
        if (!this.Validate(out message))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.Name) ||
            string.IsNullOrWhiteSpace(this.Description) ||
            string.IsNullOrWhiteSpace(this.Icon) )
        {
            throw new InvalidOperationException("Should never happen");
        }

        string groupName = this.Name.Trim();
        string groupDescription = this.Description.Trim();
        string iconName = this.Icon.Trim();

        // Save to model 
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
            if (this.EditedGroup is null || string.IsNullOrWhiteSpace(this.EditedGroup.Name))
            {
                throw new InvalidOperationException("Should never happen");
            }

            if (this.templatesModel.EditGroup(groupName, this.EditedGroup.Name, groupDescription, iconName, out message))
            {
                return true;
            }
        }
        else
        {
            if (this.templatesModel.AddGroup(groupName, groupDescription, iconName, out message))
            {
                return true;
            }
        }

        return false;
    }

    // Still needed ? 
    // private bool saveButtonIsDisabled;
    //{
    //    get => this.Get<bool>();
    //    set
    //    {
    //        this.Set(value);
    //        this.View.SaveButton.IsDisabled = value;
    //    }
    //}
}
