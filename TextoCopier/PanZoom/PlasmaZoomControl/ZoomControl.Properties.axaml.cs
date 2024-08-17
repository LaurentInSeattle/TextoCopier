namespace Lyt.Avalonia.Zoom;

public partial class ZoomControl : UserControl
{
    /// <summary> Zoomable content  </summary>
    public static readonly StyledProperty<Control?> ZoomableContentProperty =
        AvaloniaProperty.Register<ZoomControl, Control?>(nameof(ZoomableContent), defaultValue: null);

    /// <summary> Gets or sets the ZoomableContent property.</summary>
    public Control? ZoomableContent
    {
        get => this.GetValue(ZoomableContentProperty);
        set => this.SetValue(ZoomableContentProperty, value);
    }

    public static readonly StyledProperty<double> TranslateXProperty =
        AvaloniaProperty.Register<ZoomControl, double>(nameof(TranslateX), defaultValue: 0.0);

    public double TranslateX
    {
        get => this.GetValue(TranslateXProperty);
        set => this.SetValue(TranslateXProperty, value);
    }

    public static readonly StyledProperty<double> TranslateYProperty =
        AvaloniaProperty.Register<ZoomControl, double>(nameof(TranslateY), defaultValue: 0.0);

    public double TranslateY
    {
        get => this.GetValue(TranslateXProperty);
        set => this.SetValue(TranslateXProperty, value);
    }

    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<ZoomControl, double>(
            nameof(Zoom),
            defaultValue: 0.5,
            inherits: false,
            defaultBindingMode: BindingMode.TwoWay,
            validate: null,
            coerce: CoerceZoom,
            enableDataValidation: false);

    public double Zoom
    {
        get => this.GetValue(ZoomProperty);
        set { this.SetValue(ZoomProperty, value); }
    }

    private static double CoerceZoom(AvaloniaObject sender, double newZoom)
    {
        if (sender is ZoomControl zoomControl)
        {
            double oldValue = zoomControl.GetValue(ZoomProperty);
            Debug.WriteLine("Zoom: " + oldValue.ToString("F3") + " - > " + newZoom.ToString("F3"));
            zoomControl.UpdateTransforms(oldValue);
        } 

        return newZoom;
    }
}
