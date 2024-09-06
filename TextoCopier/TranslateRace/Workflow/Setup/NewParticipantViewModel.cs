namespace Lyt.TranslateRace.Workflow.Setup;

public sealed class NewParticipantViewModel : Bindable<NewParticipantView>
{
    private readonly IMessenger messenger;
    private readonly TranslateRaceModel translateRaceModel; 

    public NewParticipantViewModel(IMessenger messenger, TranslateRaceModel translateRaceModel)
    {
        this.messenger = messenger;
        this.translateRaceModel = translateRaceModel;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

            this.Name = string.Empty;
            this.Title = "Aggiungi un nuovo partecipante"; 
        this.OnEditing();
    }

    private void OnSave(object? _)
    {
        if (this.Save(out string message))
        {
            this.OnClose(_);
        }
        else
        {
            this.ValidationMessage = "Impossibile salvare il nuovo partecipante.";
            this.SaveButtonIsDisabled = true;
        }
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.Setup));

    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : message;
        this.SaveButtonIsDisabled = !validated;
    }

    private bool Validate(out string message)
    {
        return this.translateRaceModel.ValidateNewParticipantForAdd(this.Name, out message);
    }

    private bool Save(out string message)
    {
        if (!this.Validate(out message))
        {
            return false;
        }


        // Save to model 
        string name = this.Name.Trim();
        if (this.translateRaceModel.AddParticipant(name, out message))
        {
            return true;
        }

        return false;
    }

    public string Title { get => this.Get<string>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

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
