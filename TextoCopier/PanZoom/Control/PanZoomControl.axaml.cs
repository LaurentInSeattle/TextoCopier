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

    ///// <summary> Called when a template has been applied to the control. </summary>
    public void OnApplyTemplate()
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

    #region Event Handlers

    /// <summary>
    /// This is required for the animations, but has issues if set by something like a slider.
    /// </summary>
    private double InternalViewportZoom
    {
        get { return (double)GetValue(InternalViewportZoomProperty); }
        set { SetValue(InternalViewportZoomProperty, value); }
    }
    private static readonly DependencyProperty InternalViewportZoomProperty = DependencyProperty.Register("InternalViewportZoom",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(1.0, InternalViewportZoom_PropertyChanged, InternalViewportZoom_Coerce));

    /// <summary>
    /// Event raised when the 'ViewportZoom' property has changed value.
    /// </summary>
    private static void InternalViewportZoom_PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomAndPanControl)dependencyObject;

        if (c._contentZoomTransform != null)
        {
            //
            // Update the content scale transform whenever 'ViewportZoom' changes.
            //
            c._contentZoomTransform.ScaleX = c.InternalViewportZoom;
            c._contentZoomTransform.ScaleY = c.InternalViewportZoom;
        }

        //
        // Update the size of the viewport in content coordinates.
        //
        c.UpdateContentViewportSize();

        if (c._enableContentOffsetUpdateFromScale)
        {
            try
            {
                // 
                // Disable content focus syncronization.  We are about to update content offset whilst zooming
                // to ensure that the viewport is focused on our desired content focus point.  Setting this
                // to 'true' stops the automatic update of the content focus when content offset changes.
                //
                c._disableContentFocusSync = true;

                //
                // Whilst zooming in or out keep the content offset up-to-date so that the viewport is always
                // focused on the content focus point (and also so that the content focus is locked to the 
                // viewport focus point - this is how the google maps style zooming works).
                //
                var viewportOffsetX = c.ViewportZoomFocusX - (c.ViewportWidth / 2);
                var viewportOffsetY = c.ViewportZoomFocusY - (c.ViewportHeight / 2);
                var contentOffsetX = viewportOffsetX / c.InternalViewportZoom;
                var contentOffsetY = viewportOffsetY / c.InternalViewportZoom;
                c.ContentOffsetX = (c.ContentZoomFocusX - (c.ContentViewportWidth / 2)) - contentOffsetX;
                c.ContentOffsetY = (c.ContentZoomFocusY - (c.ContentViewportHeight / 2)) - contentOffsetY;
            }
            finally
            {
                c._disableContentFocusSync = false;
            }
        }
        c.ContentZoomChanged?.Invoke(c, EventArgs.Empty);
        c.ViewportZoom = c.InternalViewportZoom;
        c.OnPropertyChanged(new DependencyPropertyChangedEventArgs(ViewportZoomProperty, c.ViewportZoom, c.InternalViewportZoom));
        c.ScrollOwner?.InvalidateScrollInfo();
        c.SetCurrentZoomTypeEnum();
        c.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Method called to clamp the 'ViewportZoom' value to its valid range.
    /// </summary>
    private static object InternalViewportZoom_Coerce(DependencyObject dependencyObject, object baseValue)
    {
        var c = (ZoomAndPanControl)dependencyObject;
        var value = Math.Max((double)baseValue, c.MinimumZoomClamped);
        switch (c.MinimumZoomType)
        {
            case MinimumZoomTypeEnum.FitScreen:
                value = Math.Min(Math.Max(value, c.FitZoomValue), c.MaximumZoom);
                break;
            case MinimumZoomTypeEnum.FillScreen:
                value = Math.Min(Math.Max(value, c.FillZoomValue), c.MaximumZoom);
                break;
            case MinimumZoomTypeEnum.MinimumZoom:
                value = Math.Min(Math.Max(value, c.MinimumZoom), c.MaximumZoom);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return value;
    }
    #endregion

    /// <summary>
    /// Reset the viewport zoom focus to the center of the viewport.
    /// </summary>
    private void ResetViewportZoomFocus()
    {
        ViewportZoomFocusX = ViewportWidth / 2;
        ViewportZoomFocusY = ViewportHeight / 2;
    }

    /// <summary>
    /// Update the viewport size from the specified size.
    /// </summary>
    private void UpdateViewportSize(Size newSize)
    {
        if (_viewport == newSize)
            return;

        _viewport = newSize;

        //
        // Update the viewport size in content coordiates.
        //
        UpdateContentViewportSize();

        //
        // Initialise the content zoom focus point.
        //
        UpdateContentZoomFocusX();
        UpdateContentZoomFocusY();

        //
        // Reset the viewport zoom focus to the center of the viewport.
        //
        ResetViewportZoomFocus();

        //
        // Update content offset from itself when the size of the viewport changes.
        // This ensures that the content offset remains properly clamped to its valid range.
        //
        this.ContentOffsetX = this.ContentOffsetX;
        this.ContentOffsetY = this.ContentOffsetY;

        //
        // Tell that owning ScrollViewer that scrollbar data has changed.
        //
        ScrollOwner?.InvalidateScrollInfo();
    }

    /// <summary>
    /// Update the size of the viewport in content coordinates after the viewport size or 'ViewportZoom' has changed.
    /// </summary>
    private void UpdateContentViewportSize()
    {
        ContentViewportWidth = ViewportWidth / InternalViewportZoom;
        ContentViewportHeight = ViewportHeight / InternalViewportZoom;

        _constrainedContentViewportWidth = Math.Min(ContentViewportWidth, _unScaledExtent.Width);
        _constrainedContentViewportHeight = Math.Min(ContentViewportHeight, _unScaledExtent.Height);

        UpdateTranslationX();
        UpdateTranslationY();
    }

    /// <summary>
    /// Update the X coordinate of the translation transformation.
    /// </summary>
    private void UpdateTranslationX()
    {
        if (this._contentOffsetTransform != null)
        {
            var scaledContentWidth = this._unScaledExtent.Width * this.InternalViewportZoom;
            if (scaledContentWidth < this.ViewportWidth)
                //
                // When the content can fit entirely within the viewport, center it.
                //
                this._contentOffsetTransform.X = (this.ContentViewportWidth - this._unScaledExtent.Width) / 2;
            else
                this._contentOffsetTransform.X = -this.ContentOffsetX;
        }
    }

    /// <summary>
    /// Update the Y coordinate of the translation transformation.
    /// </summary>
    private void UpdateTranslationY()
    {
        if (this._contentOffsetTransform != null)
        {
            var scaledContentHeight = this._unScaledExtent.Height * this.InternalViewportZoom;
            if (scaledContentHeight < this.ViewportHeight)
                //
                // When the content can fit entirely within the viewport, center it.
                //
                this._contentOffsetTransform.Y = (this.ContentViewportHeight - this._unScaledExtent.Height) / 2;
            else
                this._contentOffsetTransform.Y = -this.ContentOffsetY;
        }
    }

    /// <summary>
    /// Update the X coordinate of the zoom focus point in content coordinates.
    /// </summary>
    private void UpdateContentZoomFocusX()
    {
        ContentZoomFocusX = ContentOffsetX + (_constrainedContentViewportWidth / 2);
    }

    /// <summary>
    /// Update the Y coordinate of the zoom focus point in content coordinates.
    /// </summary>
    private void UpdateContentZoomFocusY()
    {
        ContentZoomFocusY = ContentOffsetY + (_constrainedContentViewportHeight / 2);
    }

    public double FitZoomValue => ViewportHelpers.FitZoom(ActualWidth, ActualHeight, _content?.ActualWidth, _content?.ActualHeight);
    public double FillZoomValue => ViewportHelpers.FillZoom(ActualWidth, ActualHeight, _content?.ActualWidth, _content?.ActualHeight);
    public double MinimumZoomClamped => ((MinimumZoomType == MinimumZoomTypeEnum.FillScreen) ? FillZoomValue
                                  : (MinimumZoomType == MinimumZoomTypeEnum.FitScreen) ? FitZoomValue
                                  : MinimumZoom).ToRealNumber();

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private enum CurrentZoomTypeEnum { Fill, Fit, Other }

    private CurrentZoomTypeEnum _currentZoomTypeEnum;

    private void SetCurrentZoomTypeEnum()
    {
        if (ViewportZoom.IsWithinOnePercent(FitZoomValue))
            _currentZoomTypeEnum = CurrentZoomTypeEnum.Fit;
        else if (ViewportZoom.IsWithinOnePercent(FillZoomValue))
            _currentZoomTypeEnum = CurrentZoomTypeEnum.Fill;
        else
            _currentZoomTypeEnum = CurrentZoomTypeEnum.Other;
    }
}