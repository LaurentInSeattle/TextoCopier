namespace Lyt.Avalonia.Controls.Glyphs; 

public sealed class VisualState : AvaloniaObject
{
    /// <summary> Normal Styled Property </summary>
    public static readonly StyledProperty<IBrush> NormalProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Normal), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Normal property.</summary>
    public IBrush Normal
    {
        get => this.GetValue(NormalProperty);
        set => this.SetValue(NormalProperty, value);
    }

    /// <summary> Selected Styled Property </summary>
    public static readonly StyledProperty<IBrush> SelectedProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Selected), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Selected property.</summary>
    public IBrush Selected
    {
        get => this.GetValue(SelectedProperty);
        set => this.SetValue(SelectedProperty, value);
    }

    /// <summary> Disabled Styled Property </summary>
    public static readonly StyledProperty<IBrush> DisabledProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Disabled), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Disabled property.</summary>
    public IBrush Disabled
    {
        get => this.GetValue(DisabledProperty);
        set => this.SetValue(DisabledProperty, value);
    }

    /// <summary> Pressed Styled Property </summary>
    public static readonly StyledProperty<IBrush> PressedProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Pressed), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Pressed property.</summary>
    public IBrush Pressed
    {
        get => this.GetValue(PressedProperty);
        set => this.SetValue(PressedProperty, value);
    }

    /// <summary> Hot Styled Property </summary>
    public static readonly StyledProperty<IBrush> HotProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Hot), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Hot property.</summary>
    public IBrush Hot
    {
        get => this.GetValue(HotProperty);
        set => this.SetValue(HotProperty, value);
    }
}
