namespace Lyt.TextoCopier.Workflow;

public sealed class NewEditGroupViewModel : Bindable<NewEditGroupView>
{
    private readonly IMessenger messenger;

    public NewEditGroupViewModel()
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.CloseCommand = new Command(this.OnClose);
        this.SaveCommand = new Command(this.OnSave);
    }

    public Group? EditedGroup { get; private set; }

    public bool IsEditing => this.EditedGroup != null;

    public override void Activate(object? activationParameter)
    {
        if (activationParameter is Group group)
        {
            this.EditedGroup = group;
            this.Name = group.Name;
            this.Description = group.Description;
            this.Icon = group.Icon;
        }
        else
        {
            this.EditedGroup = null;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Icon = "home";
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
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.StaticView.GoBack));

    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : message;
        this.SaveButtonIsDisabled = !validated;
    }

    private bool Validate(out string message)
    {
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
            return model.ValidateGroupForEdit(this.Name, this.EditedGroup!.Name, this.Description, this.Icon, out message);
        }
        else
        {
            return model.ValidateGroupForAdd(this.Name, this.Description, this.Icon, out message);
        }
    }

    private bool Save(out string message)
    {
        if (!this.Validate(out message))
        {
            return false;
        }

        // Save to model 
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
            if (model.EditGroup(this.Name, this.EditedGroup!.Name, this.Description, this.Icon, out message))
            {
                return true;
            }
        }
        else
        {
            if (model.AddGroup(this.Name, this.Description, this.Icon, out message))
            {
                return true;
            }
        }

        return false;
    }

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
