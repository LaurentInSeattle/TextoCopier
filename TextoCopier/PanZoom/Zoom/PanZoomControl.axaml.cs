namespace Lyt.Avalonia.Controls.PanZoom;

public partial class PanZoomControl : UserControl
{
    private Size contentSize; 

    public PanZoomControl()
    {
        this.InitializeComponent();

        this.Loaded += this.OnLoaded;
        this.ZoomContentPresenter.SizeChanged += this.OnZoomContentPresenterSizeChanged;
        this.ScrollViewer.SizeChanged += this.OnScrollViewerSizeChanged;
        //this.PointerPressed += this.OnPointerPressed;
        //this.PointerReleased += this.OnPointerReleased;
        //this.PointerMoved += this.OnPointerMoved;
        //this.PointerWheelChanged += this.OnPointerWheelChanged;
        //this.DoubleTapped += this.OnDoubleTapped;
    }

    ~PanZoomControl()
    {
        this.Loaded -= this.OnLoaded;
        this.ZoomContentPresenter.SizeChanged -= this.OnZoomContentPresenterSizeChanged;
        this.ScrollViewer.SizeChanged -= this.OnScrollViewerSizeChanged;
        //this.PointerPressed -= this.OnPointerPressed;
        //this.PointerReleased -= this.OnPointerReleased;
        //this.PointerMoved -= this.OnPointerMoved;
        //this.PointerWheelChanged -= this.OnPointerWheelChanged;
        //this.DoubleTapped -= this.OnDoubleTapped;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // get the presenter, and initialize
        if (this.ZoomContentPresenter is null)
        {
            throw new Exception("no presenter");
        }
        
        if (this.ZoomableContent is null)
        {
            Debug.WriteLine("Loaded: no content");
            return; 
        }

        this.UpdateContent(this.ZoomableContent);
        this.UpdateZoom(this.Zoom);
    }

    private void OnScrollViewerSizeChanged(object? sender, SizeChangedEventArgs e)
        => this.ZoomToFit();

    private void OnZoomContentPresenterSizeChanged(object? sender, SizeChangedEventArgs e) 
        => this.AdjustContentSize(this.ZoomContentPresenter.Content);

    private void AdjustContentSize(object? controlObject  )
    {
        if (controlObject is not Control control)
        {
            return;
        }

        this.contentSize = control.Bounds.Size;
        if ((contentSize.Width <= 0.000_001) || (contentSize.Height <= 0.000_001))
        {
            Debug.WriteLine("Invalid content size, forcing a layout pass");
            control.InvalidateVisual();
            this.contentSize = control.Bounds.Size;
            if ((contentSize.Width <= 0.000_001) || (contentSize.Height <= 0.000_001))
            {
                Debug.WriteLine("Still... Invalid content size");
            }
        }
    }

    private void UpdateContent(Control control)
    {
        this.ZoomContentPresenter.Content = control;
        this.AdjustContentSize(control);
    }

    private void UpdateZoom(double zoom)
    {
        if ((contentSize.Width <= 0.000_001) || (contentSize.Height<= 0.000_001))
        {
            Debug.WriteLine("Invalid content");
            this.AdjustContentSize(this.ZoomContentPresenter.Content);
        }

        this.Grid.Width = this.contentSize.Width * zoom;
        this.Grid.Height = this.contentSize.Height * zoom;
    }

    private void ProcessRequest(ActionRequest newActionRequest)
    {
        Debug.WriteLine( "Request: " + newActionRequest.ToString() );
        switch (newActionRequest)
        {
            default:
            case ActionRequest.None: return; 

            case ActionRequest.Fit:
                this.ZoomToFit(); 
                break;
            case ActionRequest.One:
                break;
        }
    }

    private void ZoomToFit()
    {
        Debug.WriteLine("Executing Request: ZoomToFit" );
        Size size = this.ScrollViewer.Bounds.Size;
        double zoomX = size.Width / this.contentSize.Width;
        double zoomY = size.Height / this.contentSize.Height;
        double zoom = Math.Min(zoomX, zoomY);
        this.Zoom = zoom;
        this.Grid.Width = this.contentSize.Width * zoom;
        this.Grid.Height = this.contentSize.Height * zoom;
    }

    private void ZoomToOne()
    {
        Debug.WriteLine("Executing Request: Zoom To One");
        this.Zoom = 1.0; 
        this.Grid.Width = this.contentSize.Width;
        this.Grid.Height = this.contentSize.Height;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs args)
    {
    }

    private void OnPointerMoved(object? sender, PointerEventArgs args)
    {
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs args)
    {
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs args)
    {
    }
}