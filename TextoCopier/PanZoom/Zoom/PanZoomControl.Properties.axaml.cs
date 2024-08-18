namespace Lyt.Avalonia.Controls.PanZoom;

public partial class PanZoomControl : UserControl
{
    /// <summary> Zoomable content  </summary>
    public static readonly StyledProperty<Control?> ZoomableContentProperty =
        AvaloniaProperty.Register<PanZoomControl, Control?>(
            nameof(ZoomableContent),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceContent,
            enableDataValidation: false);

    /// <summary> Gets or sets the ZoomableContent property.</summary>
    public Control? ZoomableContent    {
        get => this.GetValue(ZoomableContentProperty);
        set => this.SetValue(ZoomableContentProperty, value);
    }

    private static Control? CoerceContent(AvaloniaObject sender, Control? newContent)
    {
        if (sender is PanZoomControl zoomControl)
        {
            if (newContent is null)
            {
                Debug.WriteLine("Null Content!");
                return zoomControl.ZoomableContent;
            }
            else
            {
                Debug.WriteLine("Zoom: " + newContent.ToString());
                zoomControl.UpdateContent(newContent);
                return newContent;
            }
        }

        return null;
    }

    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(
            nameof(Zoom),
            defaultValue: 0.5,
            inherits: false,
            defaultBindingMode: BindingMode.TwoWay,
            validate: null,
            coerce: CoerceZoom,
            enableDataValidation: false);

    public double Zoom { get => this.GetValue(ZoomProperty); set => this.SetValue(ZoomProperty, value); }

    private static double CoerceZoom(AvaloniaObject sender, double newZoom)
    {
        if (sender is PanZoomControl zoomControl)
        {
            double oldValue = zoomControl.GetValue(ZoomProperty);
            Debug.WriteLine("Zoom: " + oldValue.ToString("F3") + " - > " + newZoom.ToString("F3"));
            if (newZoom <= 0.000_001)
            {
                return oldValue;
            }

            zoomControl.UpdateZoom(newZoom);
        }

        return newZoom;
    }
}
