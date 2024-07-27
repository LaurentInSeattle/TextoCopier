namespace Lyt.TextoCopier.Workflow;

public sealed class HelpViewModel : Bindable<HelpView>
{
    private readonly IMessenger messenger;
    private readonly LocalizerModel localizerModel;

    public HelpViewModel(IMessenger messenger, LocalizerModel localizerModel)
    {
        this.DisablePropertyChangedLogging = true; 
        this.messenger = messenger;
        this.localizerModel = localizerModel;
    }

    protected override void OnViewLoaded()  => this.About = this.localizerModel.LookupResource("About");
    
    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string About { get => this.Get<string>()!; set => this.Set(value); }
}
