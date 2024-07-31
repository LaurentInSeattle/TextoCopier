namespace Lyt.Avalonia.PanZoom;

public partial class PanZoomControl : UserControl
{
    /// <summary> Reference to the underlying content, which is named PART_Content in the template. </summary>
    private Control? _content = null;

    /// <summary>The transform that is applied to the content to scale it by 'ViewportZoom'. </summary>
    private ScaleTransform? _contentZoomTransform = null;

    /// <summary> The transform that is applied to the content to offset it by 'ContentOffsetX' and 'ContentOffsetY'. </summary>
    private TranslateTransform? _contentOffsetTransform = null;

    /// <summary> The height of the viewport in content coordinates, clamped to the height of the content. </summary>
    private double _constrainedContentViewportHeight = 0.0;

    /// <summary> The width of the viewport in content coordinates, clamped to the width of the content. </summary>
    private double _constrainedContentViewportWidth = 0.0;

    /// <summary>
    /// Normally when content offsets changes the content focus is automatically updated.
    /// This syncronization is disabled when 'disableContentFocusSync' is set to 'true'.
    /// When we are zooming in or out we 'disableContentFocusSync' is set to 'true' because 
    /// we are zooming in or out relative to the content focus we don't want to update the focus.
    /// </summary>
    private bool _disableContentFocusSync = false;

    /// <summary>
    /// Enable the update of the content offset as the content scale changes.
    /// This enabled for zooming about a point (google-maps style zooming) and zooming to a rect.
    /// </summary>
    private bool _enableContentOffsetUpdateFromScale = false;

    /// <summary> Used to disable syncronization between IScrollInfo interface and ContentOffsetX/ContentOffsetY.</summary>
    private bool _disableScrollOffsetSync = false;

    // These data members are for the implementation of the IScrollInfo interface.
    // This interface works with the ScrollViewer such that when ZoomAndPanControl is 
    // wrapped (in XAML) with a ScrollViewer the IScrollInfo interface allows the ZoomAndPanControl to
    // handle the the scrollbar offsets.
    //
    // The IScrollInfo properties and member functions are implemented in ZoomAndPanControl_IScrollInfo.cs.
    //
    // There is a good series of articles showing how to implement IScrollInfo starting here:
    //     http://blogs.msdn.com/bencon/archive/2006/01/05/509991.aspx
    //

    /// <summary> Records the unscaled extent of the content. This is calculated during the measure and arrange. </summary>
    private Size _unScaledExtent = new Size(0, 0);

    /// <summary>
    /// Records the size of the viewport (in viewport coordinates) onto the content.
    /// This is calculated during the measure and arrange.
    /// </summary>
    private Size _viewport = new Size(0, 0);

    /// <summary>
    /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
    /// </summary>
    static PanZoomControl()
    {
        // DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(typeof(ZoomAndPanControl)));
    }

    public PanZoomControl() 
    {    
        this.InitializeComponent();
        this.SizeChanged += this.OnSizeChanged;
    }

    /// <summary> Need to update zoom values if size changes, and update ViewportZoom if too low </summary>
    // protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        //base.OnRenderSizeChanged(sizeInfo);
        //if (sizeInfo.NewSize.Width <= 1 || sizeInfo.NewSize.Height <= 1) return;
        //switch (_currentZoomTypeEnum)
        //{
        //    case CurrentZoomTypeEnum.Fit:
        //        InternalViewportZoom = ViewportHelpers.FitZoom(sizeInfo.NewSize.Width, sizeInfo.NewSize.Height,
        //            _content?.ActualWidth, _content?.ActualHeight);
        //        break;
        //    case CurrentZoomTypeEnum.Fill:
        //        InternalViewportZoom = ViewportHelpers.FillZoom(sizeInfo.NewSize.Width, sizeInfo.NewSize.Height,
        //            _content?.ActualWidth, _content?.ActualHeight);
        //        break;
        //}
        //if (InternalViewportZoom < MinimumZoomClamped) InternalViewportZoom = MinimumZoomClamped;

        //// INotifyPropertyChanged property update
        ////
        //OnPropertyChanged(nameof(MinimumZoomClamped));
        //OnPropertyChanged(nameof(FillZoomValue));
        //OnPropertyChanged(nameof(FitZoomValue));
    }

    /// <summary> Called when a template has been applied to the control. </summary>
    public override void OnApplyTemplate()
    {
        //base.OnApplyTemplate();

        //_content = this.Template.FindName("PART_Content", this) as FrameworkElement;
        //if (_content != null)
        //{
        //    //
        //    // Setup the transform on the content so that we can scale it by 'ViewportZoom'.
        //    //
        //    this._contentZoomTransform = new ScaleTransform(this.InternalViewportZoom, this.InternalViewportZoom);

        //    //
        //    // Setup the transform on the content so that we can translate it by 'ContentOffsetX' and 'ContentOffsetY'.
        //    //
        //    this._contentOffsetTransform = new TranslateTransform();
        //    UpdateTranslationX();
        //    UpdateTranslationY();

        //    //
        //    // Setup a transform group to contain the translation and scale transforms, and then
        //    // assign this to the content's 'RenderTransform'.
        //    //
        //    var transformGroup = new TransformGroup();
        //    transformGroup.Children.Add(this._contentOffsetTransform);
        //    transformGroup.Children.Add(this._contentZoomTransform);
        //    _content.RenderTransform = transformGroup;
        //    ZoomAndPanControl_EventHandlers_OnApplyTemplate();
        //}
    }

    /// <summary> Measure the control and its children. </summary>
    protected override Size MeasureOverride(Size constraint)
    {
        var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        var childSize = base.MeasureOverride(infiniteSize);

        if (childSize != _unScaledExtent)
        {
            // Use the size of the child as the un-scaled extent content.
            _unScaledExtent = childSize;
            ScrollOwner?.InvalidateScrollInfo();
        }

        // Update the size of the viewport onto the content based on the passed in 'constraint'.
        UpdateViewportSize(constraint);
        var width = constraint.Width;
        var height = constraint.Height;
        if (double.IsInfinity(width)) width = childSize.Width;
        if (double.IsInfinity(height)) height = childSize.Height;
        UpdateTranslationX();
        UpdateTranslationY();
        return new Size(width, height);
    }

    /// <summary> Arrange the control and its children. </summary>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var size = base.ArrangeOverride(this.DesiredSize);
        if (_content.DesiredSize != _unScaledExtent)
        {
            // Use the size of the child as the un-scaled extent content.
            _unScaledExtent = _content.DesiredSize;
            ScrollOwner?.InvalidateScrollInfo();
        }

        // Update the size of the viewport onto the content based on the passed in 'arrangeBounds'.
        UpdateViewportSize(arrangeBounds);

        return size;
    }

}