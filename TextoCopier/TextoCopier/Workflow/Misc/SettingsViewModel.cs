namespace Lyt.TextoCopier.Workflow;

public sealed partial class SettingsViewModel : ViewModel<SettingsView>
{
    private readonly IToaster toaster;
    private readonly IDialogService dialogService;
    private readonly TemplatesModel templatesModel;

    // Must match the order of the ComboBox items listed in the view
    private readonly string[] Languages = ["en-US", "fr-FR", "it-IT"];

    [ObservableProperty]
    private int selectedLanguage;

    public SettingsViewModel(
        IDialogService dialogService, IToaster toaster, 
        TemplatesModel templatesModel)
    {
        this.toaster = toaster; 
        this.dialogService = dialogService;
        this.templatesModel = templatesModel;

        this.SelectedLanguage = this.LanguageToIndex(this.templatesModel.Language);
    }

    [RelayCommand]
    public void OnClose()
        => this.Messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));

    [RelayCommand]
    public void OnSave()
    {
        string language = this.IndexToLanguage(this.SelectedLanguage);
        if (this.templatesModel.Language != language)
        {
            var confirmActionParameters = new ConfirmActionParameters
            {
                Title = this.Localizer.Lookup("Settings.RestartRequired.Title"),
                Message = this.Localizer.Lookup("Settings.RestartRequired.Hint"),
                ActionVerb = this.Localizer.Lookup("Settings.RestartRequired.Verb"),
                OnConfirm = this.OnRestartConfirmed,
            };

            if (this.toaster.Host is Panel panel )
            {
                this.dialogService.Confirm(panel, confirmActionParameters);
            }
        }
        else
        {
            this.OnClose();
        }
    }

    private void OnRestartConfirmed(bool confirmed)
    {
        if (!confirmed)
        {
            return;
        }

        string language = this.IndexToLanguage(this.SelectedLanguage);
        this.templatesModel.Language = language;
        this.Localizer.SelectLanguage(language);
        this.templatesModel.Save();
        this.Logger.Debug("Selected language changed to: " + language);

        if (Application.Current is App application ) 
        {
            application.RestartRequired = true; 
            _ = application.Shutdown() ;
        } 
    }

    private int LanguageToIndex(string language)
    {
        int index = 0;
        foreach (string item in this.Languages)
        {
            if (item == language)
            {
                return index;
            }

            ++index;
        }

        throw new Exception("Incorrect language definitions.");
    }

    private string IndexToLanguage(int index) => this.Languages[index];
}
