namespace Lyt.TranslateRace.Workflow.Setup;

public sealed partial class NewParticipantViewModel(TranslateRaceModel translateRaceModel) 
    : ViewModel<NewParticipantView>
{
    private readonly TranslateRaceModel translateRaceModel = translateRaceModel;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        this.Name = string.Empty;
        this.Title = "Aggiungi un nuovo partecipante";
        this.OnEditing();
    }

    [RelayCommand]
    public void OnSave()
    {
        if (this.Save(out string _))
        {
            this.OnClose();
        }
        else
        {
            this.ValidationMessage = "Impossibile salvare il nuovo partecipante.";
            this.SaveButtonIsDisabled = true;
        }
    }

    [RelayCommand]
    public void OnClose()
        => this.Messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.Setup));

    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : message;
        this.SaveButtonIsDisabled = !validated;
    }

    private bool Validate(out string message) 
        => this.translateRaceModel.ValidateNewParticipantForAdd(this.Name!, out message);

    private bool Save(out string message)
    {
        if (!this.Validate(out message))
        {
            return false;
        }


        // Save to model 
        string name = this.Name!.Trim();
        if (this.translateRaceModel.AddParticipant(name, out message))
        {
            return true;
        }

        return false;
    }

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? validationMessage;

    [ObservableProperty]
    private bool saveButtonIsDisabled; 
}
