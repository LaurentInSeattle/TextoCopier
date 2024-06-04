namespace Lyt.TextoCopier.Workflow;

public sealed class NewTemplateViewModel : Bindable<NewTemplateView>
{
    private readonly IMessenger messenger;

    public NewTemplateViewModel()
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.CloseCommand = new Command(this.OnClose);
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.StaticView.GoBack));

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
