namespace PanZoom;

public class ShellViewModel : Bindable<ShellView>
{
    // PanZoomControl panZoom;
    ZoomControl zoom;

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
        //base.OnViewLoaded();
        ////this.zoom = this.View.PanAndZoom;
        //var bitmap = new Bitmap(AssetLoader.Open(new Uri("avares://PanZoom/Assets/Images/dark.jpg")));
        //var image = new Image
        //{
        //    Stretch = Stretch.UniformToFill,
        //    Source = bitmap,
        //};

        //image.Width = bitmap.PixelSize.Width;
        //image.Height = bitmap.PixelSize.Height;
        //this.ZoomableContent = image;

        this.View.scroller.Offset = new Vector(100, 100.0); 
    }

    private void OnZoomIn(object? _)
    {
        this.ZoomFactor = 2.0; 
        // this.panZoom.ViewportZoom = 0.5;
    }

    private void OnZoomOut(object? _)
    {
        this.ZoomFactor = 0.5;
    }

    private void OnZoomOne(object? _)
    {
        this.ZoomFactor = 1.0;
    }

    public double ZoomFactor { get => this.Get<double>(); set => this.Set(value); }

    public Control ZoomableContent { get => this.Get<Control>()!; set => this.Set(value); }

    public ICommand ZoomOneCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ZoomInCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ZoomOutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
