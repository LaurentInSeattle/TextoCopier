namespace Avalonia.Controls.PanAndZoom;

/// <summary> Zoom changed event handler. </summary>
/// <param name="sender">The sender object.</param>
/// <param name="e">Zoom changed event arguments.</param>
public delegate void ZoomChangedEventHandler(object sender, ZoomChangedEventArgs e);

/// <summary> Zoom changed event arguments. </summary>
/// <remarks> Initializes a new instance of the <see cref="ZoomChangedEventArgs"/> class. </remarks>
public class ZoomChangedEventArgs(double zoomX, double zoomY, double offsetX, double offsetY) : EventArgs
{
    /// <summary> Gets the zoom ratio for x axis. </summary>
    public double ZoomX { get; } = zoomX;

    /// <summary> Gets the zoom ratio for y axis. </summary>
    public double ZoomY { get; } = zoomY;

    /// <summary> Gets the pan offset for x axis. </summary>
    public double OffsetX { get; } = offsetX;

    /// <summary> Gets the pan offset for y axis. </summary>
    public double OffsetY { get; } = offsetY;
}