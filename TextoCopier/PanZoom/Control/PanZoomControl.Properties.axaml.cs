namespace Lyt.Avalonia.PanZoom;

public partial class PanZoomControl : UserControl
{
    // Definitions for dependency properties.

    /// <summary>
    /// This allows the same property name be used for direct and indirect access to this control.
    /// </summary>
    public PanZoomControl ZoomAndPanContent => this;

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

    /// <summary> ContentOffsetX Styled Property  Get/set the X offset (in content coordinates) of the view on the content. </summary>
    public static readonly StyledProperty<double> ContentOffsetXProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ContentOffsetX), defaultValue: 0.0);

    /// <summary> Gets or sets the ContentOffsetX property.</summary>
    public double ContentOffsetX
    {
        get => this.GetValue(ContentOffsetXProperty);
        set
        {
            this.SetValue(ContentOffsetXProperty, value);
        }
    }

    /// <summary> Event raised when the 'ContentOffsetX' property has changed value. </summary>
    private void ContentOffsetX_PropertyChanged()
    {
        var c = this;
        // TODO 
        //c.UpdateTranslationX();

        //if (!c._disableContentFocusSync)
        //{
        //    // Normally want to automatically update content focus when content offset changes.
        //    // Although this is disabled using 'disableContentFocusSync' when content offset changes due to in-progress zooming.
        //    //
        //    c.UpdateContentZoomFocusX();
        //} 

        //// Raise an event to let users of the control know that the content offset has changed.
        //c.ContentOffsetXChanged?.Invoke(c, EventArgs.Empty);

        //if (!c._disableScrollOffsetSync)
        //{
        //    // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
        //    c.ScrollOwner?.InvalidateScrollInfo();
        //} 
    }

    /// <summary> Method called to clamp the 'ContentOffsetX' value to its valid range. </summary>
    private object ContentOffsetX_Coerce(object baseValue)
    {
        var c = this;
        var value = (double)baseValue;
        var minOffsetX = 0.0;
        var maxOffsetX = Math.Max(0.0, c._unScaledExtent.Width - c._constrainedContentViewportWidth);
        value = Math.Min(Math.Max(value, minOffsetX), maxOffsetX);
        return value;
    }

    /// <summary> ContentOffsetY Styled Property  Get/set the Y offset (in content coordinates) of the view on the content. </summary>
    public static readonly StyledProperty<double> ContentOffsetYProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ContentOffsetY), defaultValue: 0.0);

    /// <summary> Gets or sets the ContentOffsetY property.</summary>
    public double ContentOffsetY
    {
        get => this.GetValue(ContentOffsetXProperty);
        set
        {
            this.SetValue(ContentOffsetXProperty, value);
        }
    }

    /// <summary> Event raised when the 'ContentOffsetY' property has changed value. </summary>
    private void ContentOffsetY_PropertyChanged()
    {
        var c = this;
        // TODO 
        //c.UpdateTranslationY();

        //if (!c._disableContentFocusSync)
        //{
        //    // Normally want to automatically update content focus when content offset changes.
        //    // Although this is disabled using 'disableContentFocusSync' when content offset changes due to in-progress zooming.
        //    //
        //    c.UpdateContentZoomFocusY();
        //}

        //// Raise an event to let users of the control know that the content offset has changed.
        //c.ContentOffsetYChanged?.Invoke(c, EventArgs.Empty);

        //if (!c._disableScrollOffsetSync)
        //{
        //    // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
        //    c.ScrollOwner?.InvalidateScrollInfo();
        //}
    }

    /// <summary> Method called to clamp the 'ContentOffsetX' value to its valid range. </summary>
    private object ContentOffsetY_Coerce(object baseValue)
    {
        var c = this;
        var value = (double)baseValue;
        var minOffsetY = 0.0;
        var maxOffsetY = Math.Max(0.0, c._unScaledExtent.Width - c._constrainedContentViewportWidth);
        value = Math.Min(Math.Max(value, minOffsetY), maxOffsetY);
        return value;
    }

    /// <summary> ContentViewportHeight Styled Property: Get the viewport height, in content coordinates. </summary>
    public static readonly StyledProperty<double> ContentViewportHeightProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ContentViewportHeight), defaultValue: 0.0);

    /// <summary> Gets or sets the ContentViewportHeight property.</summary>
    public double ContentViewportHeight
    {
        get => this.GetValue(ContentViewportHeightProperty);
        set
        {
            this.SetValue(ContentViewportHeightProperty, value);
        }
    }

    /// <summary> ContentViewportWidth Styled Property: Get the viewport Width, in content coordinates. </summary>
    public static readonly StyledProperty<double> ContentViewportWidthProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ContentViewportWidth), defaultValue: 0.0);

    /// <summary> Gets or sets the ContentViewportWidth property.</summary>
    public double ContentViewportWidth
    {
        get => this.GetValue(ContentViewportWidthProperty);
        set
        {
            this.SetValue(ContentViewportWidthProperty, value);
        }
    }

    /// <summary> 
    /// ContentZoomFocusX Styled Property 
    /// The X coordinate of the content focus, this is the point that we are focusing on when zooming. 
    ///</summary>
    public static readonly StyledProperty<double> ContentZoomFocusXProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ContentZoomFocusX), defaultValue: 0.0);

    /// <summary> Gets or sets the ContentZoomFocusX property.</summary>
    public double ContentZoomFocusX
    {
        get => this.GetValue(ContentZoomFocusXProperty);
        set
        {
            this.SetValue(ContentZoomFocusXProperty, value);
        }
    }

    /// <summary> 
    /// ContentZoomFocusY Styled Property 
    /// The Y coordinate of the content focus, this is the point that we are focusing on when zooming. 
    ///</summary>
    public static readonly StyledProperty<double> ContentZoomFocusYProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ContentZoomFocusY), defaultValue: 0.0);

    /// <summary> Gets or sets the ContentZoomFocusY property.</summary>
    public double ContentZoomFocusY
    {
        get => this.GetValue(ContentZoomFocusYProperty);
        set
        {
            this.SetValue(ContentZoomFocusYProperty, value);
        }
    }

    /// <summary> MaximumZoomStyled Property: Get/set the maximum value for 'ViewportZoom'. </summary>
    public static readonly StyledProperty<double> MaximumZoomProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(MaximumZoom), defaultValue: 10.0);

    /// <summary> Gets or sets the MaximumZoom property.</summary>
    public double MaximumZoom
    {
        get => this.GetValue(MaximumZoomProperty);
        set
        {
            this.SetValue(MaximumZoomProperty, value);
            // => MinimumOrMaximumZoom_PropertyChanged
        }
    }

    /// <summary> MinimumZoom Styled Property: Get/set the MinimumZoom value for 'ViewportZoom'. </summary>
    public static readonly StyledProperty<double> MinimumZoomProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(MinimumZoom), defaultValue: 0.1);

    /// <summary> Gets or sets the MinimumZoom property.</summary>
    public double MinimumZoom
    {
        get => this.GetValue(MinimumZoomProperty);
        set
        {
            this.SetValue(MinimumZoomProperty, value);
            // => MinimumOrMaximumZoom_PropertyChanged
        }
    }

    /// <summary> MinimumZoomType Styled Property:  Get/set the MinimumZoomType </summary>
    public static readonly StyledProperty<MinimumZoomTypeEnum> MinimumZoomTypeProperty =
        AvaloniaProperty.Register<PanZoomControl, MinimumZoomTypeEnum>(nameof(MinimumZoomType), defaultValue: MinimumZoomTypeEnum.MinimumZoom);

    /// <summary> Gets or sets the MinimumZoomType property.</summary>
    public MinimumZoomTypeEnum MinimumZoomType
    {
        get => this.GetValue(MinimumZoomTypeProperty);
        set
        {
            this.SetValue(MinimumZoomTypeProperty, value);
        }
    }

    /// <summary>  
    /// IsMouseWheelScrollingEnabled Styled Property: Set to 'true' to enable the mouse wheel to scroll the 
    /// zoom and pan control. This is set to 'false' by default. 
    /// </summary>
    public static readonly StyledProperty<bool> IsMouseWheelScrollingEnabledProperty =
        AvaloniaProperty.Register<PanZoomControl, bool>(nameof(IsMouseWheelScrollingEnabled), defaultValue: false);

    /// <summary> Gets or sets the IsMouseWheelScrollingEnabled property.</summary>
    public bool IsMouseWheelScrollingEnabled
    {
        get => this.GetValue(IsMouseWheelScrollingEnabledProperty);
        set
        {
            this.SetValue(IsMouseWheelScrollingEnabledProperty, value);
        }
    }

    /// <summary> MousePosition Styled Property: Get/set the MousePosition. </summary>
    public static readonly StyledProperty<Point?> MousePositionProperty =
        AvaloniaProperty.Register<PanZoomControl, Point?>(nameof(MousePosition), defaultValue: null);

    /// <summary> Gets or sets the MousePosition property.</summary>
    public Point? MousePosition
    {
        get => this.GetValue(MousePositionProperty);
        set
        {
            this.SetValue(MousePositionProperty, value);
            // => MinimumOrMaximumZoom_PropertyChanged 
        }
    }

    /// <summary> </summary>
    public static readonly StyledProperty<bool> UseAnimationsProperty =
        AvaloniaProperty.Register<PanZoomControl, bool>(nameof(UseAnimations), defaultValue: false);

    /// <summary> Gets or sets the UseAnimations property.</summary>
    public bool UseAnimations
    {
        get => this.GetValue(UseAnimationsProperty);
        set
        {
            this.SetValue(UseAnimationsProperty, value);
        }
    }

    /// <summary>
    /// ViewportZoom Styled Property: 
    /// This is used for binding a slider to control the zoom. Cannot use the InternalViewportZoom because of all the 
    /// assumptions in when the this property is changed. THIS IS NOT USED FOR THE ANIMATIONS
    /// </summary>
    public static readonly StyledProperty<double> ViewportZoomProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ViewportZoom), defaultValue: 1.0);

    /// <summary> Gets or sets the ViewportZoom property.</summary>
    public double ViewportZoom
    {
        get => this.GetValue(ViewportZoomProperty);
        set
        {
            this.SetValue(ViewportZoomProperty, value);
            // => ViewportZoom_PropertyChanged 
        }
    }

    /// <summary>
    /// ViewportZoomFocusX Styled Property: The X coordinate of the viewport focus, this is the point in the viewport (in viewport coordinates) 
    /// that the content focus point is locked to while zooming in.
    /// </summary>
    public static readonly StyledProperty<double> ViewportZoomFocusXProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ViewportZoomFocusX), defaultValue: 0.0);

    /// <summary> Gets or sets the ViewportZoomFocusX property.</summary>
    public double ViewportZoomFocusX
    {
        get => this.GetValue(ViewportZoomFocusXProperty);
        set
        {
            this.SetValue(ViewportZoomFocusXProperty, value);
        }
    }

    /// <summary>
    /// ViewportZoomFocusY Styled Property: The Y coordinate of the viewport focus, this is the point in the viewport (in viewport coordinates) 
    /// that the content focus point is locked to while zooming in.
    /// </summary>
    public static readonly StyledProperty<double> ViewportZoomFocusYProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(ViewportZoomFocusY), defaultValue: 0.0);

    /// <summary> Gets or sets the ViewportZoomFocusY property.</summary>
    public double ViewportZoomFocusY
    {
        get => this.GetValue(ViewportZoomFocusYProperty);
        set
        {
            this.SetValue(ViewportZoomFocusYProperty, value);
        }
    }

    /// <summary>
    /// This is required for the animations, but has issues if set by something like a slider.
    /// ViewportZoom Styled Property: 
    /// This is used for binding a slider to control the zoom. Cannot use the InternalViewportZoom because of all the 
    /// assumptions in when the this property is changed. THIS IS NOT USED FOR THE ANIMATIONS
    /// </summary>
    public static readonly StyledProperty<double> InternalViewportZoomProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(nameof(InternalViewportZoom), defaultValue: 1.0);

    /// <summary> Gets or sets the InternalViewportZoom property.</summary>
    public double InternalViewportZoom
    {
        get => this.GetValue(InternalViewportZoomProperty);
        set
        {
            this.SetValue(InternalViewportZoomProperty, value);
            // => InternalViewportZoom_PropertyChanged 
            // InternalViewportZoom_PropertyChanged, InternalViewportZoom_Coerce
        }
    }

    /// <summary> Event raised when the 'ViewportZoom' property has changed value. </summary>
    private void InternalViewportZoom_PropertyChanged()
    {
        var c = this;

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
        //c.OnPropertyChanged(new DependencyPropertyChangedEventArgs(ViewportZoomProperty, c.ViewportZoom, c.InternalViewportZoom));
        //c.ScrollOwner?.InvalidateScrollInfo();
        //c.SetCurrentZoomTypeEnum();
        //c.RaiseCanExecuteChanged();
    }

    /// <summary> Method called to clamp the 'InternalViewportZoom' value to its valid range. </summary>
    private object InternalViewportZoom_Coerce(object baseValue)
    {
        var c = this;
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
}