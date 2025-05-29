namespace Lyt.TextoCopier.Workflow;

public sealed partial class HelpViewModel : ViewModel<HelpView>
{
    [ObservableProperty]
    private string about;

    public HelpViewModel()
        => this.About = this.Localizer.LookupResource("About");

    [RelayCommand]
    public void OnClose()
        => this.Messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));
}
