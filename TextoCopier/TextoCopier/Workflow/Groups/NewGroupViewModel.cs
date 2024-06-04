namespace Lyt.TextoCopier.Workflow;

public sealed class NewGroupViewModel : Bindable<NewGroupView>
{
    private readonly IMessenger messenger;

    public NewGroupViewModel()
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.CloseCommand = new Command(this.OnClose);
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.StaticView.GoBack));

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
