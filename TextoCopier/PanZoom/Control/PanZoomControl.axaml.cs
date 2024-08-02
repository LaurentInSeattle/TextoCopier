using Avalonia.Controls;

namespace Lyt.Avalonia.PanZoom;

public partial class PanZoomControl : UserControl
{
    /// <summary> Reference to the underlying content, which is named PART_Content in the template. </summary>
    private Control _content ;

    /// <summary> Reference to the underlying content, which is named PART_Content in the template. </summary>
    private Control _oldContent;

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
        this._oldContent = this.PART_Content;
        this._content = this.PART_Content;
        this._oldContent = this._content;
        this._partDragZoomBorder = this.PART_DragZoomBorder;
        this._partDragZoomCanvas = this.PART_DragZoomCanvas;
        this.SizeChanged += this.OnSizeChanged;
        this.Loaded += this.OnLoaded;
        this.PropertyChanged += this.OnPropertyChanged;

        this.PointerPressed += this.OnPointerPressed;
        this.PointerReleased += this.OnPointerReleased;
        this.PointerMoved += this.OnPointerMoved;
        this.PointerWheelChanged += this.OnPointerWheelChanged;
        this.DoubleTapped += this.OnDoubleTapped;
    }

    ~PanZoomControl()
    {
        this.PointerPressed -= this.OnPointerPressed;
        this.PointerReleased -= this.OnPointerReleased;
        this.PointerMoved -= this.OnPointerMoved;
        this.PointerWheelChanged -= this.OnPointerWheelChanged;
        this.DoubleTapped += this.OnDoubleTapped;
        this.SizeChanged -= this.OnSizeChanged;
        this.Loaded -= this.OnLoaded;
        this.PropertyChanged -= this.OnPropertyChanged;
    }

    private void OnLoaded(object? _, RoutedEventArgs e)
    {
        // Setup the transform on the content so that we can scale it by 'ViewportZoom'.
        this._contentZoomTransform = new ScaleTransform(this.InternalViewportZoom, this.InternalViewportZoom);

        // Setup the transform on the content so that we can translate it by 'ContentOffsetX' and 'ContentOffsetY'.
        this._contentOffsetTransform = new TranslateTransform();
        UpdateTranslationX();
        UpdateTranslationY();

        // Setup a transform group to contain the translation and scale transforms, and then
        // assign this to the content's 'RenderTransform'.
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(this._contentOffsetTransform);
        transformGroup.Children.Add(this._contentZoomTransform);
        _content.RenderTransform = transformGroup;
    }

    /// <summary> Need to update zoom values if size changes, and update ViewportZoom if too low </summary>
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        e.Handled = true;
        var newSize = e.NewSize;
        if (newSize.Width <= 1 || newSize.Height <= 1)
        {
            return;
        } 

        Rect bounds = this._content.Bounds;
        switch (_currentZoomTypeEnum)
        {
            case CurrentZoomTypeEnum.Fit:
                InternalViewportZoom = 
                    ViewportHelpers.FitZoom(newSize.Width, newSize.Height, bounds.Width, bounds.Height);
                break;
            case CurrentZoomTypeEnum.Fill:
                InternalViewportZoom = 
                    ViewportHelpers.FillZoom(newSize.Width, newSize.Height, bounds.Width, bounds.Height);
                break;
        }
        if (InternalViewportZoom < MinimumZoomClamped)
        {
            InternalViewportZoom = MinimumZoomClamped;
        } 

        //// INotifyPropertyChanged property update
        ////
        //OnPropertyChanged(nameof(MinimumZoomClamped));
        //OnPropertyChanged(nameof(FillZoomValue));
        //OnPropertyChanged(nameof(FitZoomValue));
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ContentControl.ContentProperty)
        {
            this.OnContentChanged();
            return;
        }
    }

    /// <summary> When content is renewed, set event to set the initial position as specified. </summary>
    protected void OnContentChanged()
    {
        if (this._oldContent is not null)
        {
            this._oldContent.SizeChanged -= SetZoomAndPanInitialPosition;
        }

        this._oldContent = this._content;
        this._content.SizeChanged += SetZoomAndPanInitialPosition;
    }

    /// <summary> When content is renewed, set the initial position as specified </summary>
    private void SetZoomAndPanInitialPosition(object? sender, SizeChangedEventArgs e)
    {
        Rect bounds = this._content.Bounds;
        switch (ZoomAndPanInitialPosition)
        {
            default:
            case InitialPositionEnum.Default:
                break;

            case InitialPositionEnum.FitScreen:
                InternalViewportZoom = FitZoomValue;
                break;

            case InitialPositionEnum.FillScreen:
                InternalViewportZoom = FillZoomValue;
                ContentOffsetX = (bounds.Width - ViewportWidth / InternalViewportZoom) / 2;
                ContentOffsetY = (bounds.Height - ViewportHeight / InternalViewportZoom) / 2;
                break;

            case InitialPositionEnum.OneHundredPercentCentered:
                InternalViewportZoom = 1.0;
                ContentOffsetX = (bounds.Width - ViewportWidth) / 2;
                ContentOffsetY = (bounds.Height - ViewportHeight) / 2;
                break;
        }
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

            // TODO ???
            // ScrollOwner?.InvalidateScrollInfo();
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

            // TODO ???
            // ScrollOwner?.InvalidateScrollInfo();
        }

        // Update the size of the viewport onto the content based on the passed in 'arrangeBounds'.
        UpdateViewportSize(arrangeBounds);

        return size;
    }

    #region events
    /// <summary>
    /// Event raised when the ContentOffsetX property has changed.
    /// </summary>
    public event EventHandler ContentOffsetXChanged;

    /// <summary>
    /// Event raised when the ContentOffsetY property has changed.
    /// </summary>
    public event EventHandler ContentOffsetYChanged;

    /// <summary>
    /// Event raised when the ViewportZoom property has changed.
    /// </summary>
    public event EventHandler ContentZoomChanged;
    #endregion

    /// <summary> Reset the viewport zoom focus to the center of the viewport. </summary>
    private void ResetViewportZoomFocus()
    {
        ViewportZoomFocusX = ViewportWidth / 2;
        ViewportZoomFocusY = ViewportHeight / 2;
    }

    /// <summary> Update the viewport size from the specified size. </summary>
    private void UpdateViewportSize(Size newSize)
    {
        if (_viewport == newSize)
            return;

        _viewport = newSize;

        // Update the viewport size in content coordiates.
        UpdateContentViewportSize();

        // Initialise the content zoom focus point.
        UpdateContentZoomFocusX();
        UpdateContentZoomFocusY();

        // Reset the viewport zoom focus to the center of the viewport.
        ResetViewportZoomFocus();

        // Update content offset from itself when the size of the viewport changes.
        // This ensures that the content offset remains properly clamped to its valid range.
        this.ContentOffsetX = this.ContentOffsetX;
        this.ContentOffsetY = this.ContentOffsetY;

        // Tell that owning ScrollViewer that scrollbar data has changed.
        // TODO ???
        // ScrollOwner?.InvalidateScrollInfo();
    }

    /// <summary> Update the size of the viewport in content coordinates after the viewport size or 'ViewportZoom' has changed.  </summary>
    private void UpdateContentViewportSize()
    {
        ContentViewportWidth = ViewportWidth / InternalViewportZoom;
        ContentViewportHeight = ViewportHeight / InternalViewportZoom;

        _constrainedContentViewportWidth = Math.Min(ContentViewportWidth, _unScaledExtent.Width);
        _constrainedContentViewportHeight = Math.Min(ContentViewportHeight, _unScaledExtent.Height);

        UpdateTranslationX();
        UpdateTranslationY();
    }

    /// <summary> Update the X coordinate of the translation transformation. </summary>
    private void UpdateTranslationX()
    {
        if (this._contentOffsetTransform != null)
        {
            var scaledContentWidth = this._unScaledExtent.Width * this.InternalViewportZoom;
            if (scaledContentWidth < this.ViewportWidth)
            {
                // When the content can fit entirely within the viewport, center it.
                this._contentOffsetTransform.X = (this.ContentViewportWidth - this._unScaledExtent.Width) / 2;
            }
            else
            {
                this._contentOffsetTransform.X = -this.ContentOffsetX;
            }
        }
    }

    /// <summary> Update the Y coordinate of the translation transformation. </summary>
    private void UpdateTranslationY()
    {
        if (this._contentOffsetTransform != null)
        {
            var scaledContentHeight = this._unScaledExtent.Height * this.InternalViewportZoom;
            if (scaledContentHeight < this.ViewportHeight)
            {
                // When the content can fit entirely within the viewport, center it.
                this._contentOffsetTransform.Y = (this.ContentViewportHeight - this._unScaledExtent.Height) / 2;
            }
            else
            {
                this._contentOffsetTransform.Y = -this.ContentOffsetY;
            }
        }
    }

    /// <summary> Update the X coordinate of the zoom focus point in content coordinates. </summary>
    private void UpdateContentZoomFocusX()
    {
        ContentZoomFocusX = ContentOffsetX + (_constrainedContentViewportWidth / 2);
    }

    /// <summary> Update the Y coordinate of the zoom focus point in content coordinates. </summary>
    private void UpdateContentZoomFocusY()
    {
        ContentZoomFocusY = ContentOffsetY + (_constrainedContentViewportHeight / 2);
    }

    public double FitZoomValue
        => ViewportHelpers.FitZoom( 
            this.Bounds.Width, this.Bounds.Height, this._content.Bounds.Width, this._content.Bounds.Height);

    public double FillZoomValue 
        => ViewportHelpers.FillZoom(
            this.Bounds.Width, this.Bounds.Height, this._content.Bounds.Width, this._content.Bounds.Height);

    public double MinimumZoomClamped 
        // TODO
        // use switch expression 
        => ((MinimumZoomType == MinimumZoomTypeEnum.FillScreen) ? 
                FillZoomValue : 
               (MinimumZoomType == MinimumZoomTypeEnum.FitScreen) ? 
                    FitZoomValue : 
                    MinimumZoom)
            .ToRealNumber();

    private enum CurrentZoomTypeEnum 
    { 
        Fill, 
        Fit, 
        Other, 
    }

    private CurrentZoomTypeEnum _currentZoomTypeEnum;

    private void SetCurrentZoomTypeEnum()
    {
        if (ViewportZoom.IsWithinOnePercent(FitZoomValue))
        {
            _currentZoomTypeEnum = CurrentZoomTypeEnum.Fit;
        }
        else if (ViewportZoom.IsWithinOnePercent(FillZoomValue))
        {
            _currentZoomTypeEnum = CurrentZoomTypeEnum.Fill;
        }
        else
        {
            _currentZoomTypeEnum = CurrentZoomTypeEnum.Other;
        }
    }
}