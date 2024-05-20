using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Remote.Protocol.Input;
using NoahMedical.Trident.Controls.Glyphs;

namespace Lyt.Avalonia.Controls.Glyphs; 

public partial class GlyphButton : UserControl
{
    private const double ViewboxDefaultMargin = 8.0;
    private const double KeyboardPressMilliseconds = 600.0;
    private const double LongPressMilliseconds = 950.0;
    private const int KeyboardTimerIntervalMilliseconds = 75;
    private const int LongPressTimerIntervalMilliseconds = 75;
    private const int CountdownTimerIntervalMilliseconds = 100;
    private const int ContinuousTimerIntervalMilliseconds = 100;

    private readonly Brush darkColor;

    // private PopupKeyboard? popupKeyboard;

    private DispatcherTimer? timer;
    private bool isOver;
    private DateTime pressedAt;
    private bool isPressed;
    private bool isLongPressActivated;
    private double viewboxMargin;

    public GlyphButton()
    {
        this.InitializeComponent();
        this.eventingRectangle.PointerPressed += this.OnPointerPressed;
        this.eventingRectangle.PointerReleased += this.OnPointerReleased;
        this.eventingRectangle.PointerEntered += this.OnPointerEnter;
        this.eventingRectangle.PointerExited += this.OnPointerLeave; 

        this.darkColor = new SolidColorBrush(Color.FromArgb(a: 0x80, r: 20, g: 20, b: 20));
        this.Loaded += this.OnLoaded;
    }

    ~GlyphButton()
    {
        this.eventingRectangle.PointerPressed -= this.OnPointerPressed;
        this.eventingRectangle.PointerReleased -= this.OnPointerReleased;
        this.eventingRectangle.PointerEntered -= this.OnPointerEnter;
        this.eventingRectangle.PointerExited -= this.OnPointerLeave;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        //this.OnButtonBackgroundChanged(ButtonBackground.None, this.ButtonBackground);
        //this.ChangeLayout(this.Layout);
        //this.ChangeBehaviour(this.Behaviour);
        //this.OnTypographyChanged(new Style(), this.Typography);
        //this.DismissPopupKeyboard();
        //this.icon.UpdateImage();
        //this.UpdateVisualState();
    }

    public GlyphButton? ParentGlyphButton { get; set; }

    /*
     
    private bool HasIcon
        => (this.icon is not null) &&
            (this.Layout == ButtonLayout.IconOnly ||
            this.Layout == ButtonLayout.IconTextRightSide ||
            this.Layout == ButtonLayout.IconTextBelow);

    private bool HasText
        => (this.textBlock is not null) &&
            (this.Layout == ButtonLayout.IconTextRightSide ||
            this.Layout == ButtonLayout.IconTextBelow ||
            this.Layout == ButtonLayout.TextOnly);

    private bool HasBackgroundRectangle
        => this.ButtonBackground == ButtonBackground.Rectangle ||
            this.ButtonBackground == ButtonBackground.BorderlessRectangle;

    private bool HasBackgroundBorder
        => this.ButtonBackground == ButtonBackground.Rectangle ||
            this.ButtonBackground == ButtonBackground.BorderOnly;

    */

    #region Visual States 
     
    private void UpdateVisualState()
    {
        //if (this.isPressed && !this.IsDisabled)
        //{
        //    this.SetPressedVisualState();
        //}
        //else if (!this.isPressed && this.isOver && !this.IsDisabled)
        //{
        //    this.SetHotVisualState();
        //}
        //else
        //{
        //    if (this.IsDisabled)
        //    {
        //        this.SetDisabledVisualState();
        //    }
        //    else
        //    {
        //        this.SetNormalVisualState();
        //    }
        //}
    }

    /*

    private void SetPressedVisualState()
    {
        this.rectangle.Fill = Brushes.Transparent;
        if (this.HasIcon)
        {
            this.icon.Foreground = this.PressedColor;
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = this.PressedColor;
        }

        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundPressedColor;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderPressedColor;
        }
    }

    private void SetHotVisualState()
    {
        this.rectangle.Fill = Brushes.Transparent;
        if (this.HasIcon)
        {
            this.icon.Foreground = this.HotColor;
            this.viewBox.Margin = new Thickness(this.viewboxMargin + 2.0);
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = this.HotColor;
        }

        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundHotColor;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderHotColor;
        }
    }

    private void SetNormalVisualState()
    {
        this.rectangle.Fill = Brushes.Transparent;
        if (this.HasIcon)
        {
            this.icon.Foreground = this.NormalColor;
            this.viewBox.Margin = new Thickness(this.viewboxMargin);
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = this.TextForeground;
        }

        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundNormalColor;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderNormalColor;
        }
    }

    private void SetDisabledVisualState()
    {
        if (this.HasIcon)
        {
            this.icon.Foreground = this.DisabledColor;
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = this.DisabledColor;
        }


        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundDisabledColor;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderDisabledColor;
        }

        this.rectangle.Fill = this.IsMouseOver ? this.darkColor : Brushes.Transparent;
    }

    */
    #endregion Visual States 

    #region Pointer Handling

    private void OnPointerEnter(object? sender, PointerEventArgs args) => this.Enter();

    private void OnPointerLeave(object? sender, PointerEventArgs args) => this.Leave();

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args) => this.Down();

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs args) => this.Up(args);

    private void Enter()
    {
        this.isOver = true;
        this.UpdateVisualState();
    }

    private void Leave()
    {
        this.isOver = false;
        this.isPressed = false;
        //if ((this.Behaviour != ButtonBehaviour.Tap) || (this.Behaviour != ButtonBehaviour.PopupKeyboard))
        //{
        //    this.StopTimer();
        //}

        //if (this.isLongPressActivated)
        //{
        //    this.isLongPressActivated = false;
        //    this.IsDisabled = false;
        //}

        //if (this.Behaviour == ButtonBehaviour.Countdown)
        //{
        //    this.ActivateCommand(new(), ButtonTag.CountdownCancel);
        //}

        //if (this.Behaviour == ButtonBehaviour.Continuous)
        //{
        //    this.ActivateCommand(new(), ButtonTag.CountinuousEnd);
        //}

        this.UpdateVisualState();
    }

    private void Down()
    {
        this.isPressed = true;
        if (this.isLongPressActivated)
        {
            this.isLongPressActivated = false;
            // this.IsDisabled = false;
        }

        this.UpdateVisualState();
        //if ((this.Behaviour != ButtonBehaviour.Tap) || (this.Behaviour != ButtonBehaviour.PopupKeyboard))
        //{
        //    this.pressedAt = DateTime.Now;
        //    if (this.Behaviour == ButtonBehaviour.LongPress)
        //    {
        //        this.StartTimer(GlyphButton.LongPressTimerIntervalMilliseconds);
        //    }
        //    else if (this.Behaviour == ButtonBehaviour.Keyboard)
        //    {
        //        this.StartTimer(GlyphButton.KeyboardTimerIntervalMilliseconds);
        //    }
        //    else if (this.Behaviour == ButtonBehaviour.Countdown)
        //    {
        //        this.ActivateCommand(new(), ButtonTag.CountdownBegin);
        //        this.StartTimer(GlyphButton.CountdownTimerIntervalMilliseconds);
        //    }
        //    else if (this.Behaviour == ButtonBehaviour.Continuous)
        //    {
        //        this.ActivateCommand(new(), ButtonTag.CountinuousBegin);
        //        this.StartTimer(GlyphButton.ContinuousTimerIntervalMilliseconds);
        //    }
        //}
    }

    private void Up(PointerReleasedEventArgs args)
    {
        this.isPressed = false;
        //if (this.isLongPressActivated)
        //{
        //    this.isLongPressActivated = false;
        //    this.IsDisabled = false;
        //}

        this.UpdateVisualState();

        //if (this.Behaviour == ButtonBehaviour.Tap)
        //{
        //    this.ActivateCommand(rea);
        //}
        //else if (this.Behaviour == ButtonBehaviour.PopupKeyboard)
        //{
        //    if (this.Tag is string tag)
        //    {
        //        if (this.IsShifted)
        //        {
        //            tag = tag.ToUpper();
        //        }

        //        this.ActivateCommand(new(), tag);
        //    }
        //    else
        //    {
        //        this.ActivateCommand(new());
        //    }

        //    if (this.ParentGlyphButton is not null)
        //    {
        //        this.ParentGlyphButton.DismissPopupKeyboard();
        //    }
        //}
        //else if (this.Behaviour == ButtonBehaviour.Keyboard)
        //{
        //    if (this.timer is not null)
        //    {
        //        // Timer is still running so this is just treated as a regular tap
        //        if (this.Tag is string tag)
        //        {
        //            if (this.IsShifted)
        //            {
        //                tag = tag.ToUpper();
        //            }

        //            this.ActivateCommand(new(), tag);
        //        }
        //        else
        //        {
        //            this.ActivateCommand(new());
        //        }

        //        this.StopTimer();
        //    }

        //    // If the timer is running this is just in case... If not we have to do that 
        //    this.DismissPopupKeyboard();
        //}
        //else if ((this.Behaviour == ButtonBehaviour.Countdown) && (this.timer is not null))
        //{
        //    this.ActivateCommand(new(), ButtonTag.CountdownCancel);
        //    this.StopTimer();
        //}
        //else if (this.Behaviour == ButtonBehaviour.Continuous)
        //{
        //    this.ActivateCommand(new(), ButtonTag.CountinuousEnd);
        //    this.StopTimer();
        //}
        //else
        //{
        //    this.StopTimer();
        //}
    }

    #endregion Pointer Handling
}
