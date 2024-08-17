namespace Lyt.Avalonia.Zoom;

public enum ZoomViewModifierMode
{
    None,

    /// <summary> You can pan the view with the mouse in this mode. </summary>
    Pan,

    /// <summary> You can zoom in with the mouse in this mode.</summary>
    ZoomIn,

    /// <summary> You can zoom out with the mouse in this mode. </summary>
    ZoomOut,

    /// <summary> Zooming after the user has been selected the zooming box. </summary>
    ZoomBox
}

public enum ZoomControlModes
{
    /// <summary> The content should fill the given space. </summary>
    Fill,

    /// <summary> The content will be represented in its original size. </summary>
    Original,

    /// <summary> The content will be zoomed according to region of interest.
    /// </summary>
    Roi,

    /// <summary> The content will be zoomed with a custom percent. </summary>
    Custom
}


public partial class ZoomControl : UserControl
{
    /// <summary> Transform applied to the presenter. </summary>
    private ScaleTransform scaleTransform;
    private Vector startTranslate;
    private TransformGroup transformGroup;

    /// <summary> Transform applied to the scrollviewer. </summary>
    private TranslateTransform translateTransform;

    private int zoomAnimCount;
    private bool isAnimatingZooming;

    public ZoomControl()
    {
        this.InitializeComponent();

        //this.PointerPressed += this.OnPointerPressed;
        //this.PointerReleased += this.OnPointerReleased;
        //this.PointerMoved += this.OnPointerMoved;
        //this.PointerWheelChanged += this.OnPointerWheelChanged;
        //this.DoubleTapped += this.OnDoubleTapped;

        this.Loaded += this.OnLoaded;

        //PreviewMouseWheel += ZoomControl_MouseWheel;
        //PreviewMouseDown += ZoomControl_PreviewMouseDown;
        //MouseDown += ZoomControl_MouseDown;
        //MouseUp += ZoomControl_MouseUp;
    }

    ~ZoomControl()
    {
        //this.PointerPressed -= this.OnPointerPressed;
        //this.PointerReleased -= this.OnPointerReleased;
        //this.PointerMoved -= this.OnPointerMoved;
        //this.PointerWheelChanged -= this.OnPointerWheelChanged;
        //this.DoubleTapped += this.OnDoubleTapped;
        this.Loaded -= this.OnLoaded;

        //this.PropertyChanged -= this.OnPropertyChanged;
        //this.SizeChanged -= this.OnSizeChanged;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // get the presenter, and initialize
        if (this.ZoomContentPresenter is null)
        {
            throw new Exception("no prenseter");
        }

        this.TranslateX = 0.0;
        this.TranslateY = 0.0;
        this.Zoom = 1.0;

        // add the Scale + translate transform to the presenter
        this.transformGroup = new TransformGroup();
        this.scaleTransform = new ScaleTransform();
        this.translateTransform = new TranslateTransform();
        this.transformGroup.Children.Add(this.scaleTransform);
        this.transformGroup.Children.Add(this.translateTransform);
        this.ZoomContentPresenter.RenderTransform = this.transformGroup;
        this.ZoomContentPresenter.RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        this.ZoomContentPresenter.Content = this.ZoomableContent;
        this.Mode = ZoomControlModes.Fill; // was ROI 

        this.ZoomContentPresenter.SizeChanged += this.OnZoomContentPresenterSizeChanged;
        this.ZoomContentPresenter.ContentSizeChanged += this.OnZoomContentPresenterContentSizeChanged;
    }

    private void OnZoomContentPresenterContentSizeChanged(object sender, ContentSizeChangedEventArgs e) => this.Adjust();

    private void OnZoomContentPresenterSizeChanged(object? sender, SizeChangedEventArgs e) => this.Adjust();

    private void Adjust()
    {
        if (this.Mode == ZoomControlModes.Fill)
        {
            this.ZoomToFill();
        }
        else if (this.Mode == ZoomControlModes.Roi)
        {
            // this.DoZoomToRoi();
        }
    }

    public ZoomViewModifierMode ModifierMode { get; set; }

    public ZoomControlModes Mode { get; set; }

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

    private void ZoomToFill()
    {
        if (this.ZoomContentPresenter is null)
        {
            return;
        }

        Rect bounds = this.Bounds;
        double deltaZoom = Math.Min(
            bounds.Width / this.ZoomContentPresenter.ContentSize.Width,
            bounds.Height / this.ZoomContentPresenter.ContentSize.Height);
        Vector initialTranslate = this.GetInitialTranslate();
        this.ZoomAnimation(deltaZoom, initialTranslate.X * deltaZoom, initialTranslate.Y * deltaZoom);
    }

    private Vector GetInitialTranslate()
    {
        if (this.ZoomContentPresenter is null)
        {
            return new Vector(0.0, 0.0);
        }

        double w = this.ZoomContentPresenter.ContentSize.Width - this.ZoomContentPresenter.DesiredSize.Width;
        double h = this.ZoomContentPresenter.ContentSize.Height - this.ZoomContentPresenter.DesiredSize.Height;
        double transformX = -w / 2.0;
        double transformY = -h / 2.0;
        return new Vector(transformX, transformY);
    }

    private void ZoomAnimation(double targetZoom, double transformX, double transformY)
    {
        this.isAnimatingZooming = false;

        // TODO: Animate 
        //
        //var duration = new Duration(AnimationLength);
        //StartAnimation(TranslateXProperty, transformX, duration);
        //StartAnimation(TranslateYProperty, transformY, duration);
        //StartAnimation(ZoomProperty, targetZoom, duration);

        this.TranslateX = transformX;
        this.TranslateY = transformY;
        double currentZoom = this.Zoom;
        this.Zoom = targetZoom;
        this.UpdateTransforms(currentZoom);

        this.isAnimatingZooming = false;
    }

    private void UpdateTransforms(double currentZoom)
    {
        if ((this.translateTransform is null) || (this.scaleTransform is null))
        {
            return ;
            // throw new InvalidOperationException("No transforms");
        }

        this.translateTransform.X = this.TranslateX;
        this.translateTransform.Y = this.TranslateY;
        double zoom = this.Zoom;
        this.scaleTransform.ScaleX = zoom;
        this.scaleTransform.ScaleY = zoom;
        if (!this.isAnimatingZooming)
        {
            double delta = this.Zoom / currentZoom;
            this.TranslateX *= delta;
            this.TranslateY *= delta;
            this.Mode = ZoomControlModes.Custom;
        }

        this.InvalidateVisual();
    }
}