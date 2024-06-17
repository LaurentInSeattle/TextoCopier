namespace Lyt.TextoCopier.Workflow;

public sealed class NewEditGroupViewModel : Bindable<NewEditGroupView>
{
    private readonly IMessenger messenger;
    private readonly LocalizerModel localizer;
    private readonly TemplatesModel templatesModel; 

    public NewEditGroupViewModel(IMessenger messenger, LocalizerModel localizer, TemplatesModel templatesModel)
    {
        this.messenger = messenger;
        this.localizer = localizer;
        this.templatesModel = templatesModel;
        this.CloseCommand = new Command(this.OnClose);
        this.SaveCommand = new Command(this.OnSave);
    }

    public Group? EditedGroup { get; private set; }

    public bool IsEditing => this.EditedGroup != null;

    public override void Activate(object? activationParameter)
    {
        // TODO: Icon 
        if (activationParameter is Group group)
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
            this.Icon = "home";
            this.Title = this.localizer.Lookup("Shell.NewGroupLong"); 

        }

        this.OnEditing();
    }

    public override void Deactivate() => this.EditedGroup = null;

    private void OnSave(object? _)
    {
        if (this.Save(out string message))
        {
            this.OnClose(_);
        }
        else
        {
            this.ValidationMessage = message;
            this.SaveButtonIsDisabled = true;
        }
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));

    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : message;
        this.SaveButtonIsDisabled = !validated;
    }

    private bool Validate(out string message)
    {
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
            return this.templatesModel.ValidateGroupForEdit(this.Name, this.EditedGroup!.Name, this.Description, this.Icon, out message);
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

        string groupName = this.Name.Trim();
        string groupDescription = this.Description.Trim();
        string iconName = this.Icon.Trim();

        // Save to model 
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
            if (this.templatesModel.EditGroup(groupName, this.EditedGroup!.Name, groupDescription, iconName, out message))
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

    public string Title { get => this.Get<string>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }

    public string Icon { get => this.Get<string>()!; set => this.Set(value); }

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
