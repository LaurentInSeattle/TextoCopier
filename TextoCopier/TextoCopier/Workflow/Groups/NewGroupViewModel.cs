
using System.ComponentModel.DataAnnotations;

namespace Lyt.TextoCopier.Workflow;

public sealed class NewGroupViewModel : Bindable<NewGroupView>
{
    private readonly IMessenger messenger;

    public NewGroupViewModel()
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.CloseCommand = new Command(this.OnClose);
        this.SaveCommand = new Command(this.OnSave);
    }

    public override void Activate() 
    {
        this.Name = string.Empty;
        this.Description = string.Empty;
        this.Icon = string.Empty;
        this.OnEditing();
    }

    private void OnSave(object? _)
    {
        if(this.Save(out string message))
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
        this.SaveButtonIsDisabled = ! validated;
    }

    private bool Validate(out string message)
    {
        message = string.Empty;

        string name = this.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            message = "Group name cannot be left empty or blank.";
            return false;
        }

        string description = this.Description.Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            message = "Group description cannot be left empty or blank.";
            return false;
        }

        string iconName = this.Icon.Trim();
        if (string.IsNullOrWhiteSpace(iconName))
        {
            message = "An icon mane is required. ";
            return false;
        }

        return true;
    }

    private bool Save ( out string message )
    {
        if ( ! this.Validate(out message) )
        {
            return false; 
        }

        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        // TODO: Save to model 
        // if ( model.AddGroup(........); ) .....

        return true; 
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
