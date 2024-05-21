using NoahMedical.Trident.Controls.Glyphs;

namespace Lyt.Avalonia.Controls.Glyphs;

public partial class GlyphButton
{
    #region Dependency Property Countdown

    /// <summary> Countdown Styled Property </summary>
    public static readonly StyledProperty<double> CountdownProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(Countdown),
            defaultValue: 8.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceCountdown,
            enableDataValidation: false);


    /// <summary> Gets or sets the Countdown property.</summary>
    public double Countdown
    {
        get => this.GetValue(CountdownProperty);
        set => this.SetValue(CountdownProperty, value);
    }

    /// <summary> Coerces the Countdown value. </summary>
    private static double CoerceCountdown(AvaloniaObject sender, double newCountdown) => newCountdown;

    #endregion Dependency Property Countdown

    #region Dependency Property Behaviour

    /// <summary> Behaviour Styled Property </summary>
    public static readonly StyledProperty<ButtonBehaviour> BehaviourProperty =
        AvaloniaProperty.Register<GlyphButton, ButtonBehaviour>(
            nameof(Behaviour),
            defaultValue: ButtonBehaviour.Tap,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBehaviour,
            enableDataValidation: false);


    /// <summary> Gets or sets the Behaviour property.</summary>
    public ButtonBehaviour Behaviour
    {
        get => this.GetValue(BehaviourProperty);
        set
        {
            this.SetValue(BehaviourProperty, value);
            this.ChangeBehaviour(value);
        }
    }

    /// <summary> Coerces the Behaviour value. </summary>
    private static ButtonBehaviour CoerceBehaviour(AvaloniaObject sender, ButtonBehaviour newBehaviour) => newBehaviour;

    #endregion Dependency Property Behaviour

    #region Dependency Property Layout

    /// <summary> Layout Styled Property </summary>
    public static readonly StyledProperty<ButtonLayout> LayoutProperty =
        AvaloniaProperty.Register<GlyphButton, ButtonLayout>(
            nameof(Layout),
            defaultValue: ButtonLayout.IconOnly,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceLayout,
            enableDataValidation: false);


    /// <summary> Gets or sets the Layout property.</summary>
    public ButtonLayout Layout
    {
        get => this.GetValue(LayoutProperty);
        set
        {
            this.SetValue(LayoutProperty, value);
            this.ChangeLayout(value);
        }
    }

    /// <summary> Coerces the Behaviour value. </summary>
    private static ButtonLayout CoerceLayout(AvaloniaObject sender, ButtonLayout newLayout) => newLayout;

    #endregion Dependency Property Layout

    #region Dependency Property IsShown

    /// <summary> IsShown Styled Property </summary>
    public static readonly StyledProperty<bool> IsShownProperty =
        AvaloniaProperty.Register<GlyphButton, bool>(
            nameof(IsShown),
            defaultValue: true,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceIsShown,
            enableDataValidation: false);

    /// <summary> Gets or sets the IsShown property.</summary>
    public bool IsShown
    {
        get => this.GetValue(IsShownProperty);
        set
        {
            this.SetValue(IsShownProperty, value);
            this.mainGrid.IsVisible = value;
        }
    }

    /// <summary> Coerces the IsShown value. </summary>
    private static bool CoerceIsShown(AvaloniaObject sender, bool newIsShown) => newIsShown;

    #endregion Dependency Property IsShown

    #region Dependency Property IsDisabled

    /// <summary> IsDisabled Styled Property </summary>
    public static readonly StyledProperty<bool> IsDisabledProperty =
        AvaloniaProperty.Register<GlyphButton, bool>(
            nameof(IsDisabled),
            defaultValue: true,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceIsDisabled,
            enableDataValidation: false);

    /// <summary> Gets or sets the IsDisabled property.</summary>
    public bool IsDisabled
    {
        get => this.GetValue(IsDisabledProperty);
        set
        {
            this.SetValue(IsDisabledProperty, value);
            this.UpdateVisualState();
        }
    }

    /// <summary> Coerces the IsDisabled value. </summary>
    private static bool CoerceIsDisabled(AvaloniaObject sender, bool newIsDisabled) => newIsDisabled;

    #endregion Dependency Property IsDisabled

    #region Dependency Property Text

    /// <summary> Text Styled Property </summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<GlyphButton, string>(
            nameof(Text),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceText,
            enableDataValidation: false);

    /// <summary> Gets or sets the Text property.</summary>
    public string Text
    {
        get => this.GetValue(TextProperty);
        set
        {
            this.SetValue(TextProperty, value);
            this.textBlock.Text = value;
        }
    }

    /// <summary> Coerces the Text value. </summary>
    private static string CoerceText(AvaloniaObject sender, string newText) => newText;

    #endregion Dependency Property Text

    /*
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

    */
    #region Dependency Property Typography

    /// <summary> Typography Styled Property </summary>
    public static readonly StyledProperty<ControlTheme> TypographyProperty =
        AvaloniaProperty.Register<GlyphButton, ControlTheme>(
            nameof(Typography),
            defaultValue: new ControlTheme(),
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: null,
            enableDataValidation: false);

    /// <summary> Gets or sets the Typography property.</summary>
    public ControlTheme Typography
    {
        get => this.GetValue(TypographyProperty);
        set
        {
            this.SetValue(TypographyProperty, value);
            this.textBlock.Theme = value;
            var resources = this.textBlock.Resources; 
            this.textBlock.Resources = new ResourceDictionary() ;
            this.textBlock.Resources = resources;

            bool applied = this.textBlock.ApplyStyling();  // Does nothing
            this.textBlock.InvalidateVisual();             // Does not help 

            var mi = typeof(StyledElement).GetMethod(
                "ApplyControlTheme", BindingFlags.Instance | BindingFlags.NonPublic, new Type[] { } ) ;
            if ( mi != null )
            {
                mi.Invoke( this.textBlock, new object[] { }); // Does nothing
                this.textBlock.InvalidateVisual();            // Does not help 
            }
        }
    }

    #endregion Dependency Property Typography
    /*
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
*/

    /*
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
    */

    #region Dependency Property BackgroundCornerRadius

    /// <summary> BackgroundCornerRadius Styled Property </summary>
    public static readonly StyledProperty<double> BackgroundCornerRadiusProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(BackgroundCornerRadius),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBackgroundCornerRadius,
            enableDataValidation: false);


    /// <summary> Gets or sets the BackgroundCornerRadius property.</summary>
    public double BackgroundCornerRadius
    {
        get => this.GetValue(BackgroundCornerRadiusProperty);
        set
        {
            this.SetValue(BackgroundCornerRadiusProperty, value);
            this.rectangleBackground.RadiusX = value;
            this.rectangleBackground.RadiusY = value;
            this.rectanglePopup.RadiusX = value;
            this.rectanglePopup.RadiusY = value;
        }
    }

    /// <summary> Coerces the BackgroundCornerRadius value. </summary>
    private static double CoerceBackgroundCornerRadius(AvaloniaObject sender, double newBackgroundCornerRadius)
        => newBackgroundCornerRadius;

    #endregion Dependency Property BackgroundCornerRadius

    #region Dependency Property BackgroundBorderThickness

    /// <summary> BackgroundBorderThickness Styled Property </summary>
    public static readonly StyledProperty<double> BackgroundBorderThicknessProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(BackgroundBorderThickness),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBackgroundBorderThickness,
            enableDataValidation: false);


    /// <summary> Gets or sets the BackgroundBorderThickness property.</summary>
    public double BackgroundBorderThickness
    {
        get => this.GetValue(BackgroundBorderThicknessProperty);
        set
        {
            this.SetValue(BackgroundBorderThicknessProperty, value);
            this.rectangleBackground.StrokeThickness = value;
        }
    }

    /// <summary> Coerces the BackgroundBorderThickness value. </summary>
    private static double CoerceBackgroundBorderThickness(AvaloniaObject sender, double newBackgroundBorderThickness)
    {
        return newBackgroundBorderThickness;
    }

    #endregion Dependency Property BackgroundBorderThickness

    #region Dependency Property GlyphSource

    /// <summary> GlyphSource Styled Property </summary>
    public static readonly StyledProperty<string> GlyphSourceProperty =
        AvaloniaProperty.Register<GlyphButton, string>(
            nameof(GlyphSource),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceGlyphSource,
            enableDataValidation: false);

    /// <summary> Gets or sets the GlyphSource property.</summary>
    public string GlyphSource
    {
        get => this.GetValue(GlyphSourceProperty);
        set
        {
            this.SetValue(GlyphSourceProperty, value);
            this.icon.Source = value;
        }
    }

    /// <summary> Coerces the GlyphSource value. </summary>
    private static string CoerceGlyphSource(AvaloniaObject sender, string newText) => newText;

    #endregion Dependency Property GlyphSource

    #region Dependency Property GlyphAngle

    /// <summary> GlyphAngle Styled Property </summary>
    public static readonly StyledProperty<double> GlyphAngleProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(GlyphAngle),
            defaultValue: 0.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceGlyphAngle,
            enableDataValidation: false);


    /// <summary> Gets or sets the GlyphAngle property.</summary>
    public double GlyphAngle
    {
        get => this.GetValue(GlyphAngleProperty);
        set
        {
            this.SetValue(GlyphAngleProperty, value);
            // See: https://stackoverflow.com/questions/70116300/layouttransform-scaletransform-in-avalonia
            // TODO: Verify the rotation 
            this.icon.RenderTransform = new RotateTransform(value);
        }
    }

    /// <summary> Coerces the GlyphAngle value. </summary>
    private static double CoerceGlyphAngle(AvaloniaObject sender, double newGlyphAngle)
    {
        return newGlyphAngle;
    }

    #endregion Dependency Property GlyphAngle

    #region Dependency Property GlyphStrokeThickness

    /// <summary> GlyphStrokeThickness Styled Property </summary>
    public static readonly StyledProperty<double> GlyphStrokeThicknessProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(GlyphStrokeThickness),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceGlyphStrokeThickness,
            enableDataValidation: false);


    /// <summary> Gets or sets the GlyphStrokeThickness property.</summary>
    public double GlyphStrokeThickness
    {
        get => this.GetValue(GlyphStrokeThicknessProperty);
        set
        {
            this.SetValue(GlyphStrokeThicknessProperty, value);
            this.icon.StrokeThickness = value;
            this.icon.UpdateImage();
        }
    }

    /// <summary> Coerces the GlyphStrokeThickness value. </summary>
    private static double CoerceGlyphStrokeThickness(AvaloniaObject sender, double newGlyphStrokeThickness)
    {
        return newGlyphStrokeThickness;
    }

    #endregion Dependency Property GlyphStrokeThickness

    /*
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
}
