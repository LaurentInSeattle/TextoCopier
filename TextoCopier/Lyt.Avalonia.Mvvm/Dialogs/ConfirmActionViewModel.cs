namespace Lyt.Avalonia.Mvvm.Dialogs; 

public sealed class ConfirmActionViewModel : Bindable<ConfirmActionView>
{
    public ConfirmActionViewModel()
    {
        this.ActionCommand = new Command(this.OnAction);
        this.DismissCommand = new Command(this.OnDismiss);
    }

    private void OnAction(object? _)
    {
    }

    private void OnDismiss(object? _)
    {
    }

    public string Title { get => this.Get<string>()!; set => this.Set(value); }

    public string Text { get => this.Get<string>()!; set => this.Set(value); }

    public string ActionVerb { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand DismissCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ActionCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

}
