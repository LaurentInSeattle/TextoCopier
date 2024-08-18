namespace PanZoom;

public class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel() {    }

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

        // this.View.scroller.Offset = new Vector(100, 100.0); 
    }

    private void OnZoomIn(object? _)
    {
        this.ZoomFactor = 2.0; 
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
