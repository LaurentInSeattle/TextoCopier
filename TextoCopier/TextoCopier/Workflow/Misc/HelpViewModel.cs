namespace Lyt.TextoCopier.Workflow;

public sealed partial class HelpViewModel : ViewModel<HelpView>
{
    [ObservableProperty]
    private string about;

    public HelpViewModel()
        => this.About = this.Localizer.LookupResource("About");

    [RelayCommand]
    public void OnClose()
        => new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack).Publish();
}
