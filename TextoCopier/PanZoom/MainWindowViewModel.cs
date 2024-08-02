namespace PanZoom;

internal class MainWindowViewModel : Bindable<MainWindow>
{
    private void OnZoomIn ( object ? _)
    {
    }

    private void OnZoomOut(object? _)
    {

    }

    public ICommand ZoomInCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ZoomOutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

}
