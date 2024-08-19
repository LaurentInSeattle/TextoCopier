namespace Lyt.Invasion.Controls;

public partial class ZoomController : UserControl
{
    private readonly IMessenger messenger;
    private readonly IAnimationService animationService;
    private bool leaving;

    public ZoomController()
    {
        this.InitializeComponent();
        this.messenger = App.GetRequiredService<IMessenger>();
        this.animationService = App.GetRequiredService<IAnimationService>();
        this.Opacity = 1.0;
        this.Slider.Minimum = 1.0;
        this.Slider.Maximum = 2.25; 
        this.Slider.Value = 1.0; 
        this.PointerEntered += this.OnPointerEnter;
        this.PointerExited += this.OnPointerLeave;
    }

    ~ZoomController()
    {
        this.PointerEntered -= this.OnPointerEnter;
        this.PointerExited -= this.OnPointerLeave;
    }

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        this.leaving = false; 
        this.animationService.FadeIn(this, 0.2);
    } 

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        this.leaving = true; 
        Schedule.OnUiThread(5000, () =>
        {
            if (this.leaving && ( this.Opacity == 1.0)) 
            {
                this.animationService.FadeOut(this, 0.3);
            }
        }, DispatcherPriority.Background);
    } 

    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
        => this.messenger.Publish(new ZoomRequestMessage(e.NewValue));  

    private void OnButtonMaxClick(object? sender, RoutedEventArgs e)
        => this.Slider.Value = this.Slider.Maximum;

    private void OnButtonMinClick(object? sender, RoutedEventArgs e)
        => this.Slider.Value = this.Slider.Minimum;
}