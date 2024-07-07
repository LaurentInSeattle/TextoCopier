namespace Lyt.TextoCopier.Workflow;

public sealed class SettingsViewModel : Bindable<SettingsView>
{
    private readonly IMessenger messenger;
    private readonly LocalizerModel localizerModel;
    private readonly TemplatesModel templatesModel;

    // Must match the order of the ComboBox items listed in the view
    private readonly string [] Languages = [ "en-US", "fr-FR", "it-IT" ] ;

    public SettingsViewModel(IMessenger messenger, LocalizerModel localizerModel, TemplatesModel templatesModel)
    {
        this.messenger = messenger;
        this.localizerModel = localizerModel;
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
            this.templatesModel.Language = language;
            this.localizerModel.SelectLanguage(language);
            this.templatesModel.Save();
            this.Logger.Debug("Selected language changed to: " + language);
        } 
        else
        {
            this.OnClose(null); 
        }
    }

    public int SelectedLanguage { get => this.Get<int>(); set => this.Set(value); }

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    private int LanguageToIndex (string language)
    {
        int index = 0;
        foreach (string item in this.Languages) 
        { 
            if( item == language)
            {
                return index; 
            }

            ++index; 
        }

        throw new Exception("Incorrect language definitions."); 
    }

    private string IndexToLanguage(int index) => this.Languages[index];
}
