namespace Lyt.TextoCopier.Workflow;

public sealed class TemplateViewModel : Bindable<TemplateView>
{
    public TemplateViewModel()
    {
        this.CopyCommand = new Command(this.OnCopy);
        this.EditCommand = new Command(this.OnEdit);
        this.DeleteCommand = new Command(this.OnDelete);
    }

    private void OnCopy(object? _)
    {
        this.Logger.Info("Clicked on Copy!");
    }

    private void OnEdit(object? _)
    {
        this.Logger.Info("Clicked on Edit!");
    }

    private void OnDelete(object? _)
    {
        this.Logger.Info("Clicked on Delete!");
    }

    public ICommand CopyCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand EditCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
