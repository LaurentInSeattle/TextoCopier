namespace Lyt.Avalonia.PanZoom;

public partial class PanZoomControl : UserControl // Scroll 
{
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
    /// Set to 'true' when the vertical scrollbar is enabled.
    /// </summary>
    public bool CanVerticallyScroll { get; set; } = false;

    /// <summary>
    /// Set to 'true' when the vertical scrollbar is enabled.
    /// </summary>
    public bool CanHorizontallyScroll { get; set; } = false;

    /// <summary>
    /// The width of the content (with 'ViewportZoom' applied).
    /// </summary>
    public double ExtentWidth => _unScaledExtent.Width * InternalViewportZoom;

    /// <summary>
    /// The height of the content (with 'ViewportZoom' applied).
    /// </summary>
    public double ExtentHeight => _unScaledExtent.Height * InternalViewportZoom;

    /// <summary>
    /// Get the width of the viewport onto the content.
    /// </summary>
    public double ViewportWidth => _viewport.Width;

    /// <summary>
    /// Get the height of the viewport onto the content.
    /// </summary>
    public double ViewportHeight => _viewport.Height;

    /// <summary>
    /// Reference to the ScrollViewer that is wrapped (in XAML) around the ZoomAndPanControl.
    /// Or set to null if there is no ScrollViewer.
    /// </summary>
    public ScrollViewer ScrollOwner { get; set; } = null;

    /// <summary>
    /// The offset of the horizontal scrollbar.
    /// </summary>
    public double HorizontalOffset => ContentOffsetX * InternalViewportZoom;

    /// <summary>
    /// The offset of the vertical scrollbar.
    /// </summary>
    public double VerticalOffset => ContentOffsetY * InternalViewportZoom;

    /// <summary>
    /// Called when the offset of the horizontal scrollbar has been set.
    /// </summary>
    public void SetHorizontalOffset(double offset)
    {
        if (_disableScrollOffsetSync) return;

        try
        {
            _disableScrollOffsetSync = true;
            ContentOffsetX = offset / InternalViewportZoom;

            // Undo / Redo 
            // DelayedSaveZoom750Miliseconds();
        }
        finally
        {
            _disableScrollOffsetSync = false;
        }
    }

    /// <summary>
    /// Called when the offset of the vertical scrollbar has been set.
    /// </summary>
    public void SetVerticalOffset(double offset)
    {
        if (_disableScrollOffsetSync) return;

        try
        {
            _disableScrollOffsetSync = true;
            ContentOffsetY = offset / InternalViewportZoom;

            // Undo / Redo 
            // DelayedSaveZoom750Miliseconds();
        }
        finally
        {
            _disableScrollOffsetSync = false;
        }
    }

    /// <summary>
    /// Shift the content offset one line up.
    /// </summary>
    public void LineUp()
    {
        // Undo / Redo 
        // DelayedSaveZoom750Miliseconds();

        ContentOffsetY -= (ContentViewportHeight / 10);
    }

    /// <summary>
    /// Shift the content offset one line down.
    /// </summary>
    public void LineDown()
    {
        // Undo / Redo 
        // DelayedSaveZoom750Miliseconds();
        
        ContentOffsetY += (ContentViewportHeight / 10);
    }

    /// <summary>
    /// Shift the content offset one line left.
    /// </summary>
    public void LineLeft()
    {
        // Undo / Redo 
        // DelayedSaveZoom750Miliseconds();

        ContentOffsetX -= (ContentViewportWidth / 10);
    }

    /// <summary>
    /// Shift the content offset one line right.
    /// </summary>
    public void LineRight()
    {
        // Undo / Redo 
        // DelayedSaveZoom750Miliseconds();
        ContentOffsetX += (ContentViewportWidth / 10);
    }

    /// <summary>
    /// Shift the content offset one page up.
    /// </summary>
    public void PageUp()
    {
        // Undo / Redo 
        // DelayedSaveZoom1500Miliseconds();
        ContentOffsetY -= ContentViewportHeight;
    }

    /// <summary>
    /// Shift the content offset one page down.
    /// </summary>
    public void PageDown()
    {
        // Undo / Redo 
        // DelayedSaveZoom1500Miliseconds();
        ContentOffsetY += ContentViewportHeight;
    }

    /// <summary>
    /// Shift the content offset one page left.
    /// </summary>
    public void PageLeft()
    {
        // Undo / Redo 
        // DelayedSaveZoom1500Miliseconds();
        ContentOffsetX -= ContentViewportWidth;
    }

    /// <summary>
    /// Shift the content offset one page right.
    /// </summary>
    public void PageRight()
    {
        // Undo / Redo 
        // DelayedSaveZoom1500Miliseconds();
        ContentOffsetX += ContentViewportWidth;
    }

    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelDown()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineDown();
        }
    }

    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelLeft()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineLeft();
        }
    }

    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelRight()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineRight();
        }
    }

    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelUp()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineUp();
        }
    }

    /// <summary> Bring the specified rectangle to view. </summary>
    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
        // TODO: Broken ---  In use ??? 
        // 
        if (_content.IsVisualAncestorOf(visual))
        {
            // Transforms the specified bounding box and returns an axis-aligned bounding box that is
            // exactly large enough to contain it.
            // var transformedRect = visual.TransformToAncestor(_content).TransformBounds(rectangle);

            //var matrix = visual.TransformToVisual(_content);
            //var transformedBounds = matrix.                // new TransformedBounds( rectangle , )
            var transformedRect = rectangle;

            var viewportRect = new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight);
            if (!transformedRect.Contains(viewportRect))
            {
                double horizOffset = 0;
                double vertOffset = 0;

                if (transformedRect.Left < viewportRect.Left)
                {
                    // Want to move viewport left.
                    horizOffset = transformedRect.Left - viewportRect.Left;
                }
                else if (transformedRect.Right > viewportRect.Right)
                {
                    // Want to move viewport right.
                    horizOffset = transformedRect.Right - viewportRect.Right;
                }

                if (transformedRect.Top < viewportRect.Top)
                {
                    // Want to move viewport up.
                    vertOffset = transformedRect.Top - viewportRect.Top;
                }
                else if (transformedRect.Bottom > viewportRect.Bottom)
                {
                    // Want to move viewport down.
                    vertOffset = transformedRect.Bottom - viewportRect.Bottom;
                }

                SnapContentOffsetTo(new Point(ContentOffsetX + horizOffset, ContentOffsetY + vertOffset));
            }
        }

        return rectangle;
    }
}