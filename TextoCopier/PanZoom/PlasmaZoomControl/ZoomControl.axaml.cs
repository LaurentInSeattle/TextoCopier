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
    private ZoomContentPresenter presenter;

    /// <summary> Transform applied to the presenter. </summary>
    private ScaleTransform scaleTransform;
    private Vector startTranslate;
    private TransformGroup transformGroup;

    /// <summary> Transform applied to the scrollviewer. </summary>
    private TranslateTransform translateTransform;

    private int zoomAnimCount;
    private bool isZooming ;

    public ZoomControl()
    {
        this.InitializeComponent();

        this.PointerPressed += this.OnPointerPressed;
        this.PointerReleased += this.OnPointerReleased;
        this.PointerMoved += this.OnPointerMoved;
        this.PointerWheelChanged += this.OnPointerWheelChanged;
        this.DoubleTapped += this.OnDoubleTapped;

        //PreviewMouseWheel += ZoomControl_MouseWheel;
        //PreviewMouseDown += ZoomControl_PreviewMouseDown;
        //MouseDown += ZoomControl_MouseDown;
        //MouseUp += ZoomControl_MouseUp;
    }

    ~ZoomControl()
    {
        this.PointerPressed -= this.OnPointerPressed;
        this.PointerReleased -= this.OnPointerReleased;
        this.PointerMoved -= this.OnPointerMoved;
        this.PointerWheelChanged -= this.OnPointerWheelChanged;
        this.DoubleTapped += this.OnDoubleTapped;
        //this.SizeChanged -= this.OnSizeChanged;
        //this.Loaded -= this.OnLoaded;
        //this.PropertyChanged -= this.OnPropertyChanged;
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