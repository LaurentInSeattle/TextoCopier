namespace Lyt.Avalonia.PanZoom;

public partial class PanZoomControl : UserControl
{
    // Definitions for dependency properties.
    //
    /// <summary>
    /// This allows the same property name be used for direct and indirect access to this control.
    /// </summary>
    public PanZoomControl ZoomAndPanContent => this;

    ///// <summary> <name> Styled Property </summary>
    //public static readonly StyledProperty<type> <name>Property =
    //    AvaloniaProperty.Register<PanZoomControl, <type>>(nameof(<name>), defaultValue: false);

    ///// <summary> Gets or sets the <name> property.</summary>
    //public <type> <name>
    //{
    //    get => this.GetValue(<name>Property);
    //    set
    //    {
    //        this.SetValue(<name>Property, value);
    //    }
    //}



    /// <summary>
    /// AnimationDuration Styled Property 
    /// The duration of the animations (in seconds) started by calling AnimatedZoomTo and the other animation methods.
    /// </summary>
    public static readonly StyledProperty<double> AnimationDurationProperty =
    AvaloniaProperty.Register<PanZoomControl, double>(nameof(AnimationDuration), defaultValue: 0.4);

    /// <summary> Gets or sets the AnimationDuration property.</summary>
    public double AnimationDuration
    {
        get => this.GetValue(AnimationDurationProperty);
        set
        {
            this.SetValue(AnimationDurationProperty, value);
        }
    }

    /// <summary>ZoomAndPanInitialPosition Styled Property </summary>
    public static readonly StyledProperty<InitialPositionEnum> ZoomAndPanInitialPositionProperty =
        AvaloniaProperty.Register<PanZoomControl, InitialPositionEnum>(
            nameof(ZoomAndPanInitialPosition), defaultValue: InitialPositionEnum.Default);

    /// <summary> Gets or sets the ZoomAndPanInitialPosition property.</summary>
    public InitialPositionEnum ZoomAndPanInitialPosition
    {
        get => this.GetValue(ZoomAndPanInitialPositionProperty);
        set
        {
            this.SetValue(ZoomAndPanInitialPositionProperty, value);
            // zoomAndPanControl.SetZoomAndPanInitialPosition();
        }
    }


    /// <summary> Get/set the X offset (in content coordinates) of the view on the content. </summary>
    public double ContentOffsetX
    {
        get { return (double)GetValue(ContentOffsetXProperty); }
        set { SetValue(ContentOffsetXProperty, value); }
    }
    public static readonly DependencyProperty ContentOffsetXProperty = DependencyProperty.Register("ContentOffsetX",
        typeof(double), typeof(ZoomAndPanControl), 
        new FrameworkPropertyMetadata(0.0, ContentOffsetX_PropertyChanged, ContentOffsetX_Coerce));

    /// <summary>
    /// Event raised when the 'ContentOffsetX' property has changed value.
    /// </summary>
    private static void ContentOffsetX_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomAndPanControl)o;

        c.UpdateTranslationX();

        if (!c._disableContentFocusSync)
            //
            // Normally want to automatically update content focus when content offset changes.
            // Although this is disabled using 'disableContentFocusSync' when content offset changes due to in-progress zooming.
            //
            c.UpdateContentZoomFocusX();
        //
        // Raise an event to let users of the control know that the content offset has changed.
        //
        c.ContentOffsetXChanged?.Invoke(c, EventArgs.Empty);

        if (!c._disableScrollOffsetSync)
            //
            // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
            //
            c.ScrollOwner?.InvalidateScrollInfo();
    }

    /// <summary>
    /// Method called to clamp the 'ContentOffsetX' value to its valid range.
    /// </summary>
    private static object ContentOffsetX_Coerce(DependencyObject d, object baseValue)
    {
        var c = (ZoomAndPanControl)d;
        var value = (double)baseValue;
        var minOffsetX = 0.0;
        var maxOffsetX = Math.Max(0.0, c._unScaledExtent.Width - c._constrainedContentViewportWidth);
        value = Math.Min(Math.Max(value, minOffsetX), maxOffsetX);
        return value;
    }

    /// <summary> Get/set the Y offset (in content coordinates) of the view on the content. </summary>
    public double ContentOffsetY
    {
        get { return (double)GetValue(ContentOffsetYProperty); }
        set { SetValue(ContentOffsetYProperty, value); }
    }
    public static readonly DependencyProperty ContentOffsetYProperty = DependencyProperty.Register("ContentOffsetY",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0, ContentOffsetY_PropertyChanged, ContentOffsetY_Coerce));

    /// <summary>
    /// Event raised when the 'ContentOffsetY' property has changed value.
    /// </summary>
    private static void ContentOffsetY_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomAndPanControl)o;

        c.UpdateTranslationY();

        if (!c._disableContentFocusSync)
            //
            // Normally want to automatically update content focus when content offset changes.
            // Although this is disabled using 'disableContentFocusSync' when content offset changes due to in-progress zooming.
            //
            c.UpdateContentZoomFocusY();
        if (!c._disableScrollOffsetSync)
            //
            // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
            //
            c.ScrollOwner?.InvalidateScrollInfo();
        //
        // Raise an event to let users of the control know that the content offset has changed.
        //
        c.ContentOffsetYChanged?.Invoke(c, EventArgs.Empty);
    }

    /// <summary>
    /// Method called to clamp the 'ContentOffsetY' value to its valid range.
    /// </summary>
    private static object ContentOffsetY_Coerce(DependencyObject d, object baseValue)
    {
        var c = (ZoomAndPanControl)d;
        var value = (double)baseValue;
        var minOffsetY = 0.0;
        var maxOffsetY = Math.Max(0.0, c._unScaledExtent.Height - c._constrainedContentViewportHeight);
        value = Math.Min(Math.Max(value, minOffsetY), maxOffsetY);
        return value;
    }

    /// <summary> Get the viewport height, in content coordinates. </summary>
    public double ContentViewportHeight
    {
        get { return (double)GetValue(ContentViewportHeightProperty); }
        set { SetValue(ContentViewportHeightProperty, value); }
    }
    public static readonly DependencyProperty ContentViewportHeightProperty = DependencyProperty.Register("ContentViewportHeight",
         typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

    /// <summary> Get the viewport width, in content coordinates. </summary>
    public double ContentViewportWidth
    {
        get { return (double)GetValue(ContentViewportWidthProperty); }
        set { SetValue(ContentViewportWidthProperty, value); }
    }
    public static readonly DependencyProperty ContentViewportWidthProperty = DependencyProperty.Register("ContentViewportWidth",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

    /// <summary> The X coordinate of the content focus, this is the point that we are focusing on when zooming. </summary>
    public double ContentZoomFocusX
    {
        get { return (double)GetValue(ContentZoomFocusXProperty); }
        set { SetValue(ContentZoomFocusXProperty, value); }
    }
    public static readonly DependencyProperty ContentZoomFocusXProperty = DependencyProperty.Register("ContentZoomFocusX",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

    /// <summary> The Y coordinate of the content focus, this is the point that we are focusing on when zooming. </summary>
    public double ContentZoomFocusY
    {
        get { return (double)GetValue(ContentZoomFocusYProperty); }
        set { SetValue(ContentZoomFocusYProperty, value); }
    }
    public static readonly DependencyProperty ContentZoomFocusYProperty = DependencyProperty.Register("ContentZoomFocusY",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

    /// <summary> Set to 'true' to enable the mouse wheel to scroll the zoom and pan control. This is set to 'false' by default. </summary>
    public bool IsMouseWheelScrollingEnabled
    {
        get { return (bool)GetValue(IsMouseWheelScrollingEnabledProperty); }
        set { SetValue(IsMouseWheelScrollingEnabledProperty, value); }
    }
    public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty = DependencyProperty.Register("IsMouseWheelScrollingEnabled",
        typeof(bool), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(false));

    /// <summary> Get/set the maximum value for 'ViewportZoom'. </summary>
    public double MaximumZoom
    {
        get { return (double)GetValue(MaximumZoomProperty); }
        set { SetValue(MaximumZoomProperty, value); }
    }
    public static readonly DependencyProperty MaximumZoomProperty = DependencyProperty.Register("MaximumZoom",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(10.0, MinimumOrMaximumZoom_PropertyChanged));

    /// <summary> Get/set the MinimumZoomType </summary>
    public MinimumZoomTypeEnum MinimumZoomType
    {
        get { return (MinimumZoomTypeEnum)GetValue(MinimumZoomTypeProperty); }
        set { SetValue(MinimumZoomTypeProperty, value); }
    }
    public static readonly DependencyProperty MinimumZoomTypeProperty = DependencyProperty.Register("MinimumZoomType",
        typeof(MinimumZoomTypeEnum), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(MinimumZoomTypeEnum.MinimumZoom));

    /// <summary> Get/set the MinimumZoom value for 'ViewportZoom'. </summary>
    public double MinimumZoom
    {
        get { return (double)GetValue(MinimumZoomProperty); }
        set { SetValue(MinimumZoomProperty, value); }
    }
    public static readonly DependencyProperty MinimumZoomProperty = DependencyProperty.Register("MinimumZoom",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.1, MinimumOrMaximumZoom_PropertyChanged));

    /// <summary> Get/set the MousePosition. </summary>
    public Point? MousePosition
    {
        get { return (Point?)GetValue(MousePositionProperty); }
        set { SetValue(MousePositionProperty, value); }
    }
    public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition",
        typeof(Point?), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(null, MinimumOrMaximumZoom_PropertyChanged));

    /// <summary>
    /// This is used for binding a slider to control the zoom. Cannot use the InternalUseAnimations because of all the 
    /// assumptions in when the this property is changed. THIS IS NOT USED FOR THE ANIMATIONS
    /// </summary>
    public bool UseAnimations
    {
        get { return (bool)GetValue(UseAnimationsProperty); }
        set { SetValue(UseAnimationsProperty, value); }
    }
    public static readonly DependencyProperty
        UseAnimationsProperty = DependencyProperty.Register("UseAnimations",
        typeof(bool), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// This is used for binding a slider to control the zoom. Cannot use the InternalViewportZoom because of all the 
    /// assumptions in when the this property is changed. THIS IS NOT USED FOR THE ANIMATIONS
    /// </summary>
    public double ViewportZoom
    {
        get { return (double)GetValue(ViewportZoomProperty); }
        set { SetValue(ViewportZoomProperty, value); }
    }
    public static readonly DependencyProperty ViewportZoomProperty = DependencyProperty.Register("ViewportZoom",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(1.0, ViewportZoom_PropertyChanged));

    /// <summary>
    /// The X coordinate of the viewport focus, this is the point in the viewport (in viewport coordinates) 
    /// that the content focus point is locked to while zooming in.
    /// </summary>
    public double ViewportZoomFocusX
    {
        get { return (double)GetValue(ViewportZoomFocusXProperty); }
        set { SetValue(ViewportZoomFocusXProperty, value); }
    }
    public static readonly DependencyProperty ViewportZoomFocusXProperty = DependencyProperty.Register("ViewportZoomFocusX",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

    /// <summary>
    /// The Y coordinate of the viewport focus, this is the point in the viewport (in viewport coordinates) 
    /// that the content focus point is locked to while zooming in.
    /// </summary>
    public double ViewportZoomFocusY
    {
        get { return (double)GetValue(ViewportZoomFocusYProperty); }
        set { SetValue(ViewportZoomFocusYProperty, value); }
    }
    public static readonly DependencyProperty ViewportZoomFocusYProperty = DependencyProperty.Register("ViewportZoomFocusY",
        typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));
}