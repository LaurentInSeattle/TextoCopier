namespace Lyt.Avalonia.Controls.Glyphs;

public partial class GlyphButton
{
#pragma warning disable IDE0059

/*
    #region Dependency Property Countdown

    /// <summary> Countdown Dependency Property </summary>
    public static readonly DependencyProperty CountdownProperty =
        DependencyProperty.Register("Countdown", typeof(double), typeof(GlyphButton),
            new FrameworkPropertyMetadata((double)4.2,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnCountdownChanged),
                new CoerceValueCallback(CoerceCountdown)));

    /// <summary> Gets or sets the Countdown property.</summary>
    public double Countdown
    {
        get => (double)this.GetValue(CountdownProperty);
        set => this.SetValue(CountdownProperty, value);
    }

    /// <summary> Handles changes to the Countdown property. </summary>
    private static void OnCountdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldCountdown = (double)e.OldValue;
        var newCountdown = target.Countdown;
        target.OnCountdownChanged(oldCountdown, newCountdown);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the Countdown property. </summary>
    protected virtual void OnCountdownChanged(double oldCountdown, double newCountdown)
    {
    }

    /// <summary> Coerces the Countdown value. </summary>
    private static object CoerceCountdown(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredCountdown = (double)value;
        // TODO
        return desiredCountdown;
    }

    #endregion Dependency Property Countdown

    #region Dependency Property Behaviour

    /// <summary> Behaviour Dependency Property </summary>
    public static readonly DependencyProperty BehaviourProperty =
        DependencyProperty.Register("Behaviour", typeof(ButtonBehaviour), typeof(GlyphButton),
            new FrameworkPropertyMetadata((ButtonBehaviour)ButtonBehaviour.Tap,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBehaviourChanged),
                new CoerceValueCallback(CoerceBehaviour)));

    /// <summary> Gets or sets the Behaviour property.</summary>
    public ButtonBehaviour Behaviour
    {
        get => (ButtonBehaviour)this.GetValue(BehaviourProperty);
        set => this.SetValue(BehaviourProperty, value);
    }

    /// <summary> Handles changes to the Behaviour property. </summary>
    private static void OnBehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldBehaviour = (ButtonBehaviour)e.OldValue;
        var newBehaviour = target.Behaviour;
        target.OnBehaviourChanged(oldBehaviour, newBehaviour);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the Behaviour property. </summary>
    protected virtual void OnBehaviourChanged(ButtonBehaviour oldBehaviour, ButtonBehaviour newBehaviour)
        => this.ChangeBehaviour(newBehaviour);

    /// <summary> Coerces the Behaviour value. </summary>
    private static object CoerceBehaviour(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredBehaviour = (ButtonBehaviour)value;
        // TODO
        return desiredBehaviour;
    }

    #endregion Dependency Property Behaviour

    #region Dependency Property Keys

    /// <summary> Keys Dependency Property </summary>
    public static readonly DependencyProperty KeysProperty =
        DependencyProperty.Register("Keys", typeof(string), typeof(GlyphButton),
            new FrameworkPropertyMetadata((string)string.Empty,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnKeysChanged),
                new CoerceValueCallback(CoerceKeys)));

    /// <summary> Gets or sets the Keys property.</summary>
    public string Keys
    {
        get => (string)this.GetValue(KeysProperty);
        set => this.SetValue(KeysProperty, value);
    }

    /// <summary> Handles changes to the Keys property. </summary>
    private static void OnKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldKeys = (string)e.OldValue;
        var newKeys = target.Keys;
        target.OnKeysChanged(oldKeys, newKeys);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the Keys property. </summary>
    protected virtual void OnKeysChanged(string oldKeys, string newKeys)
    {
        if (!string.IsNullOrEmpty(newKeys))
        {
            this.Behaviour = ButtonBehaviour.Keyboard;
        }
    }

    /// <summary> Coerces the Keys value. </summary>
    private static object CoerceKeys(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredKeys = (string)value;
        // TODO
        return desiredKeys;
    }

    #endregion Dependency Property Keys

    #region Dependency Property IsShifted

    /// <summary> IsShifted Dependency Property </summary>
    public static readonly DependencyProperty IsShiftedProperty =
        DependencyProperty.Register("IsShifted", typeof(bool), typeof(GlyphButton),
            new FrameworkPropertyMetadata((bool)false,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnIsShiftedChanged),
                new CoerceValueCallback(CoerceIsShifted)));

    /// <summary> Gets or sets the IsShifted property.</summary>
    public bool IsShifted
    {
        get => (bool)this.GetValue(IsShiftedProperty);
        set => this.SetValue(IsShiftedProperty, value);
    }

    /// <summary> Handles changes to the IsShifted property. </summary>
    private static void OnIsShiftedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldIsShifted = (bool)e.OldValue;
        var newIsShifted = target.IsShifted;
        target.OnIsShiftedChanged(oldIsShifted, newIsShifted);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the IsShifted property. </summary>
    protected virtual void OnIsShiftedChanged(bool oldIsShifted, bool newIsShifted)
    {
        if ((this.Behaviour == ButtonBehaviour.Keyboard) || (this.Behaviour == ButtonBehaviour.PopupKeyboard))
        {
            if (!string.IsNullOrEmpty(this.Text))
            {
                this.Text = newIsShifted ? this.Text.ToUpper() : this.Text.ToLower();
            }

            if (this.popupKeyboard is not null)
            {
                this.popupKeyboard.Shift(newIsShifted);
            }
        }
    }

    /// <summary> Coerces the IsShifted value. </summary>
    private static object CoerceIsShifted(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredIsShifted = (bool)value;
        // TODO
        return desiredIsShifted;
    }

    #endregion Dependency Property IsShifted

    #region Dependency Property Layout

    /// <summary> Layout Dependency Property </summary>
    public static readonly DependencyProperty LayoutProperty =
        DependencyProperty.Register("Layout", typeof(ButtonLayout), typeof(GlyphButton),
            new FrameworkPropertyMetadata((ButtonLayout)ButtonLayout.IconOnly,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnLayoutChanged),
                new CoerceValueCallback(CoerceLayout)));

    /// <summary> Gets or sets the Layout property.</summary>
    public ButtonLayout Layout
    {
        get => (ButtonLayout)this.GetValue(LayoutProperty);
        set => this.SetValue(LayoutProperty, value);
    }

    /// <summary> Handles changes to the Layout property. </summary>
    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldLayout = (ButtonLayout)e.OldValue;
        var newLayout = target.Layout;
        target.OnLayoutChanged(oldLayout, newLayout);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the Layout property. </summary>
    protected virtual void OnLayoutChanged(ButtonLayout oldLayout, ButtonLayout newLayout)
        => this.ChangeLayout(newLayout);

    /// <summary> Coerces the Layout value. </summary>
    private static object CoerceLayout(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredLayout = (ButtonLayout)value;
        // TODO
        return desiredLayout;
    }

    #endregion Dependency Property Layout

    #region Dependency Property ButtonBackground

    /// <summary> ButtonBackground Dependency Property </summary>
    public static readonly DependencyProperty ButtonBackgroundProperty =
        DependencyProperty.Register("ButtonBackground", typeof(ButtonBackground), typeof(GlyphButton),
            new FrameworkPropertyMetadata(ButtonBackground.None,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnButtonBackgroundChanged),
                new CoerceValueCallback(CoerceButtonBackground)));

    /// <summary> Gets or sets the ButtonBackground property.</summary>
    public ButtonBackground ButtonBackground
    {
        get => (ButtonBackground)this.GetValue(ButtonBackgroundProperty);
        set => this.SetValue(ButtonBackgroundProperty, value);
    }

    /// <summary> Handles changes to the ButtonBackground property. </summary>
    private static void OnButtonBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldButtonBackground = (ButtonBackground)e.OldValue;
        var newButtonBackground = target.ButtonBackground;
        target.OnButtonBackgroundChanged(oldButtonBackground, newButtonBackground);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the ButtonBackground property. </summary>
    protected virtual void OnButtonBackgroundChanged(ButtonBackground oldButtonBackground, ButtonBackground newButtonBackground)
    {
        switch (newButtonBackground)
        {
            default:
            case ButtonBackground.None:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin;
                this.viewBox.Margin = new Thickness(GlyphButton.ViewboxDefaultMargin);
                this.rectangleBackground.Visibility = Visibility.Hidden;
                break;

            case ButtonBackground.BorderOnly:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin + 2.0 + this.BackgroundBorderThickness;
                this.viewBox.Margin = new Thickness(this.viewboxMargin);
                this.rectangleBackground.Visibility = Visibility.Visible;
                this.rectangleBackground.Fill = Brushes.Transparent;
                break;

            case ButtonBackground.Rectangle:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin + 2.0 + this.BackgroundBorderThickness;
                this.viewBox.Margin = new Thickness(this.viewboxMargin);
                this.rectangleBackground.Visibility = Visibility.Visible;
                this.rectangleBackground.Fill = Brushes.Transparent;// todo
                break;

            case ButtonBackground.BorderlessRectangle:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin;
                this.viewBox.Margin = new Thickness(GlyphButton.ViewboxDefaultMargin);
                this.rectangleBackground.StrokeThickness = 0.0;
                this.rectangleBackground.Visibility = Visibility.Visible;
                this.rectangleBackground.Fill = Brushes.Transparent; // todo
                break;
        }
    }

    /// <summary> Coerces the ButtonBackground value. </summary>
    private static object CoerceButtonBackground(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredButtonBackground = (ButtonBackground)value;
        // TODO
        return desiredButtonBackground;
    }

    #endregion Dependency Property ButtonBackground

    #region Dependency Property BackgroundCornerRadius

    /// <summary> BackgroundCornerRadius Dependency Property </summary>
    public static readonly DependencyProperty BackgroundCornerRadiusProperty =
        DependencyProperty.Register("BackgroundCornerRadius", typeof(double), typeof(GlyphButton),
            new FrameworkPropertyMetadata((double)1.0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnBackgroundCornerRadiusChanged),
                new CoerceValueCallback(CoerceBackgroundCornerRadius)));

    /// <summary> Gets or sets the BackgroundCornerRadius property.</summary>
    public double BackgroundCornerRadius
    {
        get => (double)this.GetValue(BackgroundCornerRadiusProperty);
        set => this.SetValue(BackgroundCornerRadiusProperty, value);
    }

    /// <summary> Handles changes to the BackgroundCornerRadius property. </summary>
    private static void OnBackgroundCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldBackgroundCornerRadius = (double)e.OldValue;
        var newBackgroundCornerRadius = target.BackgroundCornerRadius;
        target.OnBackgroundCornerRadiusChanged(oldBackgroundCornerRadius, newBackgroundCornerRadius);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the BackgroundCornerRadius property. </summary>
    protected virtual void OnBackgroundCornerRadiusChanged(double oldBackgroundCornerRadius, double newBackgroundCornerRadius)
    {
        this.rectangleBackground.RadiusX = newBackgroundCornerRadius;
        this.rectangleBackground.RadiusY = newBackgroundCornerRadius;
        this.rectanglePopup.RadiusX = newBackgroundCornerRadius;
        this.rectanglePopup.RadiusY = newBackgroundCornerRadius;
    }

    /// <summary> Coerces the BackgroundCornerRadius value. </summary>
    private static object CoerceBackgroundCornerRadius(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredBackgroundCornerRadius = (double)value;
        return desiredBackgroundCornerRadius;
    }

    #endregion Dependency Property BackgroundCornerRadius

    #region Dependency Property BackgroundBorderThickness

    /// <summary> BackgroundBorderThickness Dependency Property </summary>
    public static readonly DependencyProperty BackgroundBorderThicknessProperty =
        DependencyProperty.Register("BackgroundBorderThickness", typeof(double), typeof(GlyphButton),
            new FrameworkPropertyMetadata((double)1.0,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundBorderThicknessChanged),
                new CoerceValueCallback(CoerceBackgroundBorderThickness)));

    /// <summary> Gets or sets the BackgroundBorderThickness property.</summary>
    public double BackgroundBorderThickness
    {
        get => (double)this.GetValue(BackgroundBorderThicknessProperty);
        set => this.SetValue(BackgroundBorderThicknessProperty, value);
    }

    /// <summary> Handles changes to the BackgroundBorderThickness property. </summary>
    private static void OnBackgroundBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldBackgroundBorderThickness = (double)e.OldValue;
        var newBackgroundBorderThickness = target.BackgroundBorderThickness;
        target.OnBackgroundBorderThicknessChanged(oldBackgroundBorderThickness, newBackgroundBorderThickness);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the BackgroundBorderThickness property. </summary>
    protected virtual void OnBackgroundBorderThicknessChanged(double oldBackgroundBorderThickness, double newBackgroundBorderThickness)
    {
        this.rectangleBackground.StrokeThickness = newBackgroundBorderThickness;
    }

    /// <summary> Coerces the BackgroundBorderThickness value. </summary>
    private static object CoerceBackgroundBorderThickness(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredBackgroundBorderThickness = (double)value;
        // TODO
        return desiredBackgroundBorderThickness;
    }

    #endregion Dependency Property BackgroundBorderThickness

    #region Dependency Property IsShown

    /// <summary> IsShown Dependency Property </summary>
    public static readonly DependencyProperty IsShownProperty =
        DependencyProperty.Register("IsShown", typeof(bool), typeof(GlyphButton),
            new FrameworkPropertyMetadata(true,
                FrameworkPropertyMetadataOptions.AffectsArrange,
                new PropertyChangedCallback(OnIsShownChanged),
                new CoerceValueCallback(CoerceIsShown)));

    /// <summary> Gets or sets the IsShown property.</summary>
    public bool IsShown
    {
        get => (bool)this.GetValue(IsShownProperty);
        set => this.SetValue(IsShownProperty, value);
    }

    /// <summary> Handles changes to the IsShown property. </summary>
    private static void OnIsShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        bool oldIsShown = (bool)e.OldValue;
        bool newIsShown = target.IsShown;
        target.OnIsShownChanged(oldIsShown, newIsShown);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the IsShown property. </summary>
    protected virtual void OnIsShownChanged(bool oldIsShown, bool newIsShown)
        => this.mainGrid.Visibility = newIsShown ? Visibility.Visible : Visibility.Collapsed;

    /// <summary> Coerces the IsShown value. </summary>
    private static object CoerceIsShown(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        bool desiredIsShown = (bool)value;
        // TODO
        return desiredIsShown;
    }

    #endregion Dependency Property IsShown

    #region Dependency Property IsDisabled

    /// <summary> IsDisabled Dependency Property </summary>
    public static readonly DependencyProperty IsDisabledProperty =
        DependencyProperty.Register("IsDisabled", typeof(bool), typeof(GlyphButton),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnIsDisabledChanged),
                new CoerceValueCallback(CoerceIsDisabled)));

    /// <summary> Gets or sets the IsDisabled property.</summary>
    public bool IsDisabled
    {
        get => (bool)this.GetValue(IsDisabledProperty);
        set => this.SetValue(IsDisabledProperty, value);
    }

    /// <summary> Handles changes to the IsDisabled property. </summary>
    private static void OnIsDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        bool oldIsDisabled = (bool)e.OldValue;
        bool newIsDisabled = target.IsDisabled;
        target.OnIsDisabledChanged(oldIsDisabled, newIsDisabled);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the IsDisabled property. </summary>
    protected virtual void OnIsDisabledChanged(bool oldIsDisabled, bool newIsDisabled) => this.UpdateVisualState();

    /// <summary> Coerces the IsDisabled value. </summary>
    private static object CoerceIsDisabled(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        bool desiredIsDisabled = (bool)value;
        // TODO
        return desiredIsDisabled;
    }

    #endregion Dependency Property IsDisabled

    #region Dependency Property GlyphSource

    /// <summary> GlyphSource Dependency Property </summary>
    public static readonly DependencyProperty GlyphSourceProperty =
        DependencyProperty.Register("GlyphSource", typeof(string), typeof(GlyphButton),
            new FrameworkPropertyMetadata(string.Empty,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnGlyphSourceChanged),
                new CoerceValueCallback(CoerceGlyphSource)));

    /// <summary> Gets or sets the GlyphSource property.</summary>
    public string GlyphSource
    {
        get => (string)this.GetValue(GlyphSourceProperty);
        set => this.SetValue(GlyphSourceProperty, value);
    }

    /// <summary> Handles changes to the GlyphSource property. </summary>
    private static void OnGlyphSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldGlyphSource = (string)e.OldValue;
        var newGlyphSource = target.GlyphSource;
        target.OnGlyphSourceChanged(oldGlyphSource, newGlyphSource);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the GlyphSource property. </summary>
    protected virtual void OnGlyphSourceChanged(string oldGlyphSource, string newGlyphSource)
    {
        this.icon.Source = newGlyphSource;
    }

    /// <summary> Coerces the GlyphSource value. </summary>
    private static object CoerceGlyphSource(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredGlyphSource = (string)value;
        // TODO
        return desiredGlyphSource;
    }

    #endregion Dependency Property GlyphSource

    #region Dependency Property GlyphAngle

    /// <summary> GlyphAngle Dependency Property </summary>
    public static readonly DependencyProperty GlyphAngleProperty =
        DependencyProperty.Register("GlyphAngle", typeof(double), typeof(GlyphButton),
            new FrameworkPropertyMetadata((double)0.0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnGlyphAngleChanged),
                new CoerceValueCallback(CoerceGlyphAngle)));

    /// <summary> Gets or sets the GlyphAngle property.</summary>
    public double GlyphAngle
    {
        get => (double)this.GetValue(GlyphAngleProperty);
        set => this.SetValue(GlyphAngleProperty, value);
    }

    /// <summary> Handles changes to the GlyphAngle property. </summary>
    private static void OnGlyphAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldGlyphAngle = (double)e.OldValue;
        var newGlyphAngle = target.GlyphAngle;
        target.OnGlyphAngleChanged(oldGlyphAngle, newGlyphAngle);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the GlyphAngle property. </summary>
    protected virtual void OnGlyphAngleChanged(double oldGlyphAngle, double newGlyphAngle)
    {
        this.icon.LayoutTransform = new RotateTransform(newGlyphAngle);
    }

    /// <summary> Coerces the GlyphAngle value. </summary>
    private static object CoerceGlyphAngle(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredGlyphAngle = (double)value;
        // TODO
        return desiredGlyphAngle;
    }

    #endregion Dependency Property GlyphAngle

    #region Dependency Property GlyphStrokeThickness

    /// <summary> StrokeThickness Dependency Property </summary>
    public static readonly DependencyProperty GlyphStrokeThicknessProperty =
        DependencyProperty.Register("GlyphStrokeThickness", typeof(double), typeof(GlyphButton),
            new FrameworkPropertyMetadata((double)2.0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnGlyphStrokeThicknessChanged),
                new CoerceValueCallback(CoerceGlyphStrokeThickness)));

    /// <summary> Gets or sets the GlyphStrokeThickness property.</summary>
    public double GlyphStrokeThickness
    {
        get => (double)this.GetValue(GlyphStrokeThicknessProperty);
        set => this.SetValue(GlyphStrokeThicknessProperty, value);
    }

    /// <summary> Handles changes to the GlyphStrokeThickness property. </summary>
    private static void OnGlyphStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldStrokeThickness = (double)e.OldValue;
        var newStrokeThickness = target.GlyphStrokeThickness;
        target.OnGlyphStrokeThicknessChanged(oldStrokeThickness, newStrokeThickness);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the GlyphStrokeThickness property. </summary>
    protected virtual void OnGlyphStrokeThicknessChanged(double oldStrokeThickness, double newStrokeThickness)
    {
        this.icon.StrokeThickness = newStrokeThickness;
        this.icon.UpdateImage();
    }

    /// <summary> Coerces the GlyphStrokeThickness value. </summary>
    private static object CoerceGlyphStrokeThickness(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredStroke = (double)value;
        // TODO
        return desiredStroke;
    }

    #endregion Dependency Property GlyphStrokeThickness

    #region Dependency Property PressedColor

    /// <summary> HotColor Dependency Property </summary>
    public static readonly DependencyProperty PressedColorProperty =
        DependencyProperty.Register("PressedColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkOrange,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnPressedColorChanged),
                new CoerceValueCallback(CoercePressedColor)));

    /// <summary> Gets or sets the PressedColor property.</summary>
    public SolidColorBrush PressedColor
    {
        get => (SolidColorBrush)this.GetValue(PressedColorProperty);
        set => this.SetValue(PressedColorProperty, value);
    }

    /// <summary> Handles changes to the PressedColor property. </summary>
    private static void OnPressedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldPressedColor = (SolidColorBrush)e.OldValue;
        var newPressedColor = target.PressedColor;
        target.OnPressedColorChanged(oldPressedColor, newPressedColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the PressedColor property. </summary>
    protected virtual void OnPressedColorChanged(SolidColorBrush oldPressedColor, SolidColorBrush newPressedColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the PressedColor value. </summary>
    private static object CoercePressedColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredPressedColor = (SolidColorBrush)value;
        // TODO
        return desiredPressedColor;
    }

    #endregion Dependency Property PressedColor

    #region Dependency Property HotColor

    /// <summary> HotColor Dependency Property </summary>
    public static readonly DependencyProperty HotColorProperty =
        DependencyProperty.Register("HotColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkOrange,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnHotColorChanged),
                new CoerceValueCallback(CoerceHotColor)));

    /// <summary> Gets or sets the HotColor property.</summary>
    public SolidColorBrush HotColor
    {
        get => (SolidColorBrush)this.GetValue(HotColorProperty);
        set => this.SetValue(HotColorProperty, value);
    }

    /// <summary> Handles changes to the HotColor property. </summary>
    private static void OnHotColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldHotColor = (SolidColorBrush)e.OldValue;
        var newHotColor = target.HotColor;
        target.OnHotColorChanged(oldHotColor, newHotColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the HotColor property. </summary>
    protected virtual void OnHotColorChanged(SolidColorBrush oldHotColor, SolidColorBrush newHotColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the HotColor value. </summary>
    private static object CoerceHotColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredHotColor = (SolidColorBrush)value;
        // TODO
        return desiredHotColor;
    }

    #endregion Dependency Property HotColor

    #region Dependency Property NormalColor

    /// <summary> NormalColor Dependency Property </summary>
    public static readonly DependencyProperty NormalColorProperty =
        DependencyProperty.Register("NormalColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.White,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnNormalColorChanged),
                new CoerceValueCallback(CoerceNormalColor)));

    /// <summary> Gets or sets the NormalColor property.</summary>
    public SolidColorBrush NormalColor
    {
        get => (SolidColorBrush)this.GetValue(NormalColorProperty);
        set => this.SetValue(NormalColorProperty, value);
    }

    /// <summary> Handles changes to the NormalColor property. </summary>
    private static void OnNormalColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldNormalColor = (SolidColorBrush)e.OldValue;
        var newNormalColor = target.NormalColor;
        target.OnNormalColorChanged(oldNormalColor, newNormalColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the NormalColor property. </summary>
    protected virtual void OnNormalColorChanged(SolidColorBrush oldNormalColor, SolidColorBrush newNormalColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the NormalColor value. </summary>
    private static object CoerceNormalColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredNormalColor = (SolidColorBrush)value;
        // TODO
        return desiredNormalColor;
    }

    #endregion Dependency Property NormalColor

    #region Dependency Property DisabledColor

    /// <summary> DisabledColor Dependency Property </summary>
    public static readonly DependencyProperty DisabledColorProperty =
        DependencyProperty.Register("DisabledColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkOrange,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnDisabledColorChanged),
                new CoerceValueCallback(CoerceDisabledColor)));

    /// <summary> Gets or sets the DisabledColor property.</summary>
    public SolidColorBrush DisabledColor
    {
        get => (SolidColorBrush)this.GetValue(DisabledColorProperty);
        set => this.SetValue(DisabledColorProperty, value);
    }

    /// <summary> Handles changes to the DisabledColor property. </summary>
    private static void OnDisabledColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldDisabledColor = (SolidColorBrush)e.OldValue;
        var newDisabledColor = target.DisabledColor;
        target.OnDisabledColorChanged(oldDisabledColor, newDisabledColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the DisabledColor property. </summary>
    protected virtual void OnDisabledColorChanged(SolidColorBrush oldDisabledColor, SolidColorBrush newDisabledColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the DisabledColor value. </summary>
    private static object CoerceDisabledColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredDisabledColor = (Brush)value;
        // TODO
        return desiredDisabledColor;
    }

    #endregion Dependency Property DisabledColor

    #region Dependency Property Text

    /// <summary> Text Dependency Property </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(GlyphButton),
            new FrameworkPropertyMetadata((string)string.Empty,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnTextChanged),
                new CoerceValueCallback(CoerceText)));

    /// <summary> Gets or sets the Text property.</summary>
    public string Text
    {
        get => (string)this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }

    /// <summary> Handles changes to the Text property. </summary>
    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldText = (string)e.OldValue;
        var newText = target.Text;
        target.OnTextChanged(oldText, newText);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the Text property. </summary>
    protected virtual void OnTextChanged(string oldText, string newText)
    {
        this.textBlock.Text = newText;
    }

    /// <summary> Coerces the Text value. </summary>
    private static object CoerceText(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredText = (string)value;
        // TODO
        return desiredText;
    }

    #endregion Dependency Property Text

    #region Dependency Property TextForeground

    /// <summary> TextForeground Dependency Property </summary>
    public static readonly DependencyProperty TextForegroundProperty =
        DependencyProperty.Register("TextForeground", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.AntiqueWhite,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnTextForegroundChanged),
                new CoerceValueCallback(CoerceTextForeground)));

    /// <summary> Gets or sets the TextForeground property.</summary>
    public SolidColorBrush TextForeground
    {
        get => (SolidColorBrush)this.GetValue(TextForegroundProperty);
        set => this.SetValue(TextForegroundProperty, value);
    }

    /// <summary> Handles changes to the TextForeground property. </summary>
    private static void OnTextForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldForeground = (SolidColorBrush)e.OldValue;
        var newForeground = target.TextForeground;
        target.OnTextForegroundChanged(oldForeground, newForeground);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the TextForeground property. </summary>
    protected virtual void OnTextForegroundChanged(SolidColorBrush oldForeground, SolidColorBrush newForeground)
    {
        this.textBlock.Foreground = newForeground;
    }

    /// <summary> Coerces the TextForeground value. </summary>
    private static object CoerceTextForeground(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredForeground = (SolidColorBrush)value;
        // TODO
        return desiredForeground;
    }

    #endregion Dependency Property TextForeground

    #region Dependency Property Typography

    /// <summary> Typography Dependency Property </summary>
    public static readonly DependencyProperty TypographyProperty =
        DependencyProperty.Register("Typography", typeof(Style), typeof(GlyphButton),
            new FrameworkPropertyMetadata((Style)new(),
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnTypographyChanged),
                new CoerceValueCallback(CoerceTypography)));

    /// <summary> Gets or sets the Typography property.</summary>
    public Style Typography
    {
        get => (Style)this.GetValue(TypographyProperty);
        set => this.SetValue(TypographyProperty, value);
    }

    /// <summary> Handles changes to the Typography property. </summary>
    private static void OnTypographyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldTypography = (Style)e.OldValue;
        var newTypography = target.Typography;
        target.OnTypographyChanged(oldTypography, newTypography);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the Typography property. </summary>
    protected virtual void OnTypographyChanged(Style oldTypography, Style newTypography)
    {
        this.textBlock.Style = newTypography;
    }

    /// <summary> Coerces the Typography value. </summary>
    private static object CoerceTypography(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredTypography = (Style)value;
        // TODO
        return desiredTypography;
    }

    #endregion Dependency Property Typography

    #region Dependency Property Click

    /// <summary> Click Dependency Property </summary>
    public static readonly DependencyProperty ClickProperty =
        DependencyProperty.Register("Click", typeof(RoutedEventDelegate), typeof(GlyphButton),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnClickChanged),
                new CoerceValueCallback(CoerceClick)));

    /// <summary> Gets or sets the Click property.</summary>
    public RoutedEventDelegate Click
    {
        get => (RoutedEventDelegate)this.GetValue(ClickProperty);
        set => this.SetValue(ClickProperty, value);
    }

    /// <summary> Handles changes to the Click property. </summary>
    private static void OnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldClick = (RoutedEventDelegate)e.OldValue;
        var newClick = target.Click;
        target.OnClickChanged(oldClick, newClick);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the Click property. </summary>
    protected virtual void OnClickChanged(RoutedEventDelegate oldClick, RoutedEventDelegate newClick)
    {
    }

    /// <summary> Coerces the Click value. </summary>
    private static object CoerceClick(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredClick = (RoutedEventDelegate)value;
        // TODO
        return desiredClick;
    }

    #endregion Dependency Property Click

    #region Dependency Property Command

    /// <summary> Command Dependency Property </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(GlyphButton),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.None, null, null));

    /// <summary> Gets or sets the Command property.</summary>
    public ICommand Command
    {
        get => (ICommand)this.GetValue(CommandProperty);
        set => this.SetValue(CommandProperty, value);
    }

    #endregion Dependency Property Command

    #region Dependency Property CommandParameter

    /// <summary> CommandParameter Dependency Property </summary>
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register("CommandParameter", typeof(object), typeof(GlyphButton),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnCommandParameterChanged),
                new CoerceValueCallback(CoerceCommandParameter)));

    /// <summary> Gets or sets the CommandParameter property.</summary>
    public object CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    /// <summary> Handles changes to the CommandParameter property. </summary>
    private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        object oldCommandParameter = e.OldValue;
        object newCommandParameter = target.CommandParameter;
        target.OnCommandParameterChanged(oldCommandParameter, newCommandParameter);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the CommandParameter property. </summary>
    protected virtual void OnCommandParameterChanged(object oldCommandParameter, object newCommandParameter)
    {
    }

    /// <summary> Coerces the CommandParameter value. </summary>
    private static object CoerceCommandParameter(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        object desiredCommandParameter = value;
        // TODO
        return desiredCommandParameter;
    }

    #endregion Dependency Property CommandParameter

    #region Dependency Property BackgroundPressedColor

    /// <summary> BackgroundPressedColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundPressedColorProperty =
        DependencyProperty.Register("BackgroundPressedColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkOrange,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundPressedColorChanged),
                new CoerceValueCallback(CoerceBackgroundPressedColor)));

    /// <summary> Gets or sets the BackgroundPressedColor property.</summary>
    public SolidColorBrush BackgroundPressedColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundPressedColorProperty);
        set => this.SetValue(BackgroundPressedColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundPressedColor property. </summary>
    private static void OnBackgroundPressedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldPressedColor = (SolidColorBrush)e.OldValue;
        var newPressedColor = target.BackgroundPressedColor;
        target.OnBackgroundPressedColorChanged(oldPressedColor, newPressedColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the PressedColor property. </summary>
    protected virtual void OnBackgroundPressedColorChanged(SolidColorBrush oldPressedColor, SolidColorBrush newPressedColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the PressedColor value. </summary>
    private static object CoerceBackgroundPressedColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredPressedColor = (SolidColorBrush)value;
        // TODO
        return desiredPressedColor;
    }

    #endregion Dependency Property PressedColor

    #region Dependency Property BackgroundHotColor

    /// <summary> BackgroundHotColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundHotColorProperty =
        DependencyProperty.Register("BackgroundHotColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkOrange,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundHotColorChanged),
                new CoerceValueCallback(CoerceBackgroundHotColor)));

    /// <summary> Gets or sets the BackgroundHotColor property.</summary>
    public SolidColorBrush BackgroundHotColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundHotColorProperty);
        set => this.SetValue(BackgroundHotColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundHotColor property. </summary>
    private static void OnBackgroundHotColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldHotColor = (SolidColorBrush)e.OldValue;
        var newHotColor = target.BackgroundHotColor;
        target.OnBackgroundHotColorChanged(oldHotColor, newHotColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the BackgroundHotColor property. </summary>
    protected virtual void OnBackgroundHotColorChanged(SolidColorBrush oldHotColor, SolidColorBrush newHotColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the BackgroundHotColor value. </summary>
    private static object CoerceBackgroundHotColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredHotColor = (SolidColorBrush)value;
        // TODO
        return desiredHotColor;
    }

    #endregion Dependency Property HotColor

    #region Dependency Property BackgroundNormalColor

    /// <summary> BackgroundNormalColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundNormalColorProperty =
        DependencyProperty.Register("BackgroundNormalColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkGray,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundNormalColorChanged),
                new CoerceValueCallback(CoerceBackgroundNormalColor)));

    /// <summary> Gets or sets the BackgroundNormalColor property.</summary>
    public SolidColorBrush BackgroundNormalColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundNormalColorProperty);
        set => this.SetValue(BackgroundNormalColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundNormalColor property. </summary>
    private static void OnBackgroundNormalColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldNormalColor = (SolidColorBrush)e.OldValue;
        var newNormalColor = target.BackgroundNormalColor;
        target.OnBackgroundNormalColorChanged(oldNormalColor, newNormalColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the NormalColor property. </summary>
    protected virtual void OnBackgroundNormalColorChanged(SolidColorBrush oldNormalColor, SolidColorBrush newNormalColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the BackgroundNormalColor value. </summary>
    private static object CoerceBackgroundNormalColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredNormalColor = (SolidColorBrush)value;
        // TODO
        return desiredNormalColor;
    }

    #endregion Dependency Property BackgroundNormalColor

    #region Dependency Property BackgroundDisabledColor

    /// <summary> BackgroundDisabledColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundDisabledColorProperty =
        DependencyProperty.Register("BackgroundDisabledColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkGray,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundDisabledColorChanged),
                new CoerceValueCallback(CoerceBackgroundDisabledColor)));

    /// <summary> Gets or sets the BackgroundDisabledColor property.</summary>
    public SolidColorBrush BackgroundDisabledColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundDisabledColorProperty);
        set => this.SetValue(BackgroundDisabledColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundDisabledColor property. </summary>
    private static void OnBackgroundDisabledColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldDisabledColor = (SolidColorBrush)e.OldValue;
        var newDisabledColor = target.BackgroundDisabledColor;
        target.OnBackgroundDisabledColorChanged(oldDisabledColor, newDisabledColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the BackgroundDisabledColor property. </summary>
    protected virtual void OnBackgroundDisabledColorChanged(SolidColorBrush oldDisabledColor, SolidColorBrush newDisabledColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the BackgroundDisabledColor value. </summary>
    private static object CoerceBackgroundDisabledColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredDisabledColor = (Brush)value;
        // TODO
        return desiredDisabledColor;
    }

    #endregion Dependency Property BackgroundDisabledColor

    #region Dependency Property BackgroundBorderPressedColor

    /// <summary> BackgroundBorderPressedColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundBorderPressedColorProperty =
        DependencyProperty.Register("BackgroundBorderPressedColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkOrange,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundBorderPressedColorChanged),
                new CoerceValueCallback(CoerceBackgroundBorderPressedColor)));

    /// <summary> Gets or sets the BackgroundBorderPressedColor property.</summary>
    public SolidColorBrush BackgroundBorderPressedColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundBorderPressedColorProperty);
        set => this.SetValue(BackgroundBorderPressedColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundBorderPressedColor property. </summary>
    private static void OnBackgroundBorderPressedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldPressedColor = (SolidColorBrush)e.OldValue;
        var newPressedColor = target.BackgroundBorderPressedColor;
        target.OnBackgroundBorderPressedColorChanged(oldPressedColor, newPressedColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the BackgroundBorderPressedColor property. </summary>
    protected virtual void OnBackgroundBorderPressedColorChanged(SolidColorBrush oldPressedColor, SolidColorBrush newPressedColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the BackgroundBorderPressedColor value. </summary>
    private static object CoerceBackgroundBorderPressedColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredPressedColor = (SolidColorBrush)value;
        // TODO
        return desiredPressedColor;
    }

    #endregion Dependency Property BackgroundBorderPressedColor

    #region Dependency Property BackgroundBorderHotColor

    /// <summary> BackgroundBorderHotColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundBorderHotColorProperty =
        DependencyProperty.Register("BackgroundBorderHotColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkOrange,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundBorderHotColorChanged),
                new CoerceValueCallback(CoerceBackgroundBorderHotColor)));

    /// <summary> Gets or sets the BackgroundBorderHotColor property.</summary>
    public SolidColorBrush BackgroundBorderHotColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundBorderHotColorProperty);
        set => this.SetValue(BackgroundBorderHotColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundBorderHotColor property. </summary>
    private static void OnBackgroundBorderHotColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldHotColor = (SolidColorBrush)e.OldValue;
        var newHotColor = target.BackgroundBorderHotColor;
        target.OnBackgroundBorderHotColorChanged(oldHotColor, newHotColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the BackgroundBorderHotColor property. </summary>
    protected virtual void OnBackgroundBorderHotColorChanged(SolidColorBrush oldHotColor, SolidColorBrush newHotColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the BackgroundBorderHotColor value. </summary>
    private static object CoerceBackgroundBorderHotColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredHotColor = (SolidColorBrush)value;
        // TODO
        return desiredHotColor;
    }

    #endregion Dependency Property BackgroundBorderHotColor

    #region Dependency Property BackgroundBorderNormalColor

    /// <summary> BackgroundBorderNormalColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundBorderNormalColorProperty =
        DependencyProperty.Register("BackgroundBorderNormalColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkGray,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundBorderNormalColorChanged),
                new CoerceValueCallback(CoerceBackgroundBorderNormalColor)));

    /// <summary> Gets or sets the BackgroundBorderNormalColor property.</summary>
    public SolidColorBrush BackgroundBorderNormalColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundBorderNormalColorProperty);
        set => this.SetValue(BackgroundBorderNormalColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundBorderNormalColor property. </summary>
    private static void OnBackgroundBorderNormalColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldNormalColor = (SolidColorBrush)e.OldValue;
        var newNormalColor = target.BackgroundBorderNormalColor;
        target.OnBackgroundBorderNormalColorChanged(oldNormalColor, newNormalColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the NormalColor property. </summary>
    protected virtual void OnBackgroundBorderNormalColorChanged(SolidColorBrush oldNormalColor, SolidColorBrush newNormalColor)
    {
        this.rectanglePopup.Stroke = newNormalColor;
        this.UpdateVisualState();
    }

    /// <summary> Coerces the BackgroundNormalColor value. </summary>
    private static object CoerceBackgroundBorderNormalColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredNormalColor = (SolidColorBrush)value;
        // TODO
        return desiredNormalColor;
    }

    #endregion Dependency Property BackgroundBorderNormalColor

    #region Dependency Property BackgroundBorderDisabledColor

    /// <summary> BackgroundBorderDisabledColor Dependency Property </summary>
    public static readonly DependencyProperty BackgroundBorderDisabledColorProperty =
        DependencyProperty.Register("BackgroundBorderDisabledColor", typeof(SolidColorBrush), typeof(GlyphButton),
            new FrameworkPropertyMetadata(Brushes.DarkGray,
                FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnBackgroundBorderDisabledColorChanged),
                new CoerceValueCallback(CoerceBackgroundBorderDisabledColor)));

    /// <summary> Gets or sets the BackgroundBorderDisabledColor property.</summary>
    public SolidColorBrush BackgroundBorderDisabledColor
    {
        get => (SolidColorBrush)this.GetValue(BackgroundBorderDisabledColorProperty);
        set => this.SetValue(BackgroundBorderDisabledColorProperty, value);
    }

    /// <summary> Handles changes to the BackgroundBorderDisabledColor property. </summary>
    private static void OnBackgroundBorderDisabledColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (GlyphButton)d;
        var oldDisabledColor = (SolidColorBrush)e.OldValue;
        var newDisabledColor = target.BackgroundBorderDisabledColor;
        target.OnBackgroundBorderDisabledColorChanged(oldDisabledColor, newDisabledColor);
    }

    /// <summary> Provides derived classes an opportunity to handle changes to the BackgroundBorderDisabledColor property. </summary>
    protected virtual void OnBackgroundBorderDisabledColorChanged(SolidColorBrush oldDisabledColor, SolidColorBrush newDisabledColor)
        => this.UpdateVisualState();

    /// <summary> Coerces the BackgroundBorderDisabledColor value. </summary>
    private static object CoerceBackgroundBorderDisabledColor(DependencyObject d, object value)
    {
        var target = (GlyphButton)d;
        var desiredDisabledColor = (Brush)value;
        // TODO
        return desiredDisabledColor;
    }

    #endregion Dependency Property BackgroundBorderDisabledColor
*/
#pragma warning restore IDE0059
}
