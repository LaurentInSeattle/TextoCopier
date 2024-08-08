using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PanZoom;

public class ShellViewModel : Bindable<ShellView>
{
    PanZoomControl panZoom;

    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;

    public ShellViewModel(
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.panZoom = this.View.PanAndZoom;
        var bitmap = new Bitmap(AssetLoader.Open(new Uri("avares://PanZoom/Assets/Images/dark.jpg")));
        var image = new Image
        {
            Stretch = Stretch.UniformToFill,
            Source = bitmap,
        };
        this.ZoomableContent = image;
    }

    private void OnZoomIn(object? _)
    {
        this.panZoom.ViewportZoom = 0.5;
    }

    private void OnZoomOut(object? _)
    {
    }

    public Control ZoomableContent { get => this.Get<Control>()!; set => this.Set(value); }

    public ICommand ZoomInCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ZoomOutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
