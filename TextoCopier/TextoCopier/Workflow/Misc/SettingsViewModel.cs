namespace Lyt.TextoCopier.Workflow;

public sealed class SettingsViewModel : Bindable<SettingsView>
{
    private readonly IMessenger messenger;

    public SettingsViewModel()
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.CloseCommand = new Command(this.OnClose);
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.GoBack));

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
