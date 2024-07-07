namespace Lyt.TextoCopier.Workflow;

public sealed class SettingsViewModel : Bindable<SettingsView>
{
    private readonly IMessenger messenger;
    private readonly IToaster toaster;
    private readonly IDialogService dialogService;
    private readonly LocalizerModel localizer;
    private readonly TemplatesModel templatesModel;

    // Must match the order of the ComboBox items listed in the view
    private readonly string[] Languages = ["en-US", "fr-FR", "it-IT"];

    public SettingsViewModel(
        IMessenger messenger, IDialogService dialogService, IToaster toaster, 
        LocalizerModel localizer, TemplatesModel templatesModel)
    {
        this.messenger = messenger;
        this.toaster = toaster; 
        this.dialogService = dialogService;
        this.localizer = localizer;
        this.templatesModel = templatesModel;

        this.CloseCommand = new Command(this.OnClose);
        this.SaveCommand = new Command(this.OnSave);
        this.SelectedLanguage = this.LanguageToIndex(this.templatesModel.Language);
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));

    private void OnSave(object? _)
    {
        string language = this.IndexToLanguage(this.SelectedLanguage);
        if (this.templatesModel.Language != language)
        {
            var confirmActionParameters = new ConfirmActionParameters
            {
                Title = this.localizer.Lookup("Settings.RestartRequired.Title"),
                Message = this.localizer.Lookup("Settings.RestartRequired.Hint"),
                ActionVerb = this.localizer.Lookup("Settings.RestartRequired.Verb"),
                OnConfirm = this.OnRestartConfirmed,
            };

            if (this.toaster.Host is Panel panel )
            {
                this.dialogService.Confirm(panel, confirmActionParameters);
            }
        }
        else
        {
            this.OnClose(null);
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
        this.localizer.SelectLanguage(language);
        this.templatesModel.Save();
        this.Logger.Debug("Selected language changed to: " + language);

        if (Application.Current is App application ) 
        {
            application.RestartRequired = true; 
            _ = application.Shutdown() ;
        } 
    }

    public int SelectedLanguage { get => this.Get<int>(); set => this.Set(value); }

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

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
