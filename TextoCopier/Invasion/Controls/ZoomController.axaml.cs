namespace Lyt.Invasion.Controls;

public partial class ZoomController : UserControl
{
    private readonly IMessenger messenger;

    public ZoomController()
    {
        this.InitializeComponent();
        this.messenger = App.GetRequiredService<IMessenger>();
        this.Opacity = 1.0;
        this.Slider.Minimum = 1.0;
        this.Slider.Maximum = 2.25; 
        this.Slider.Value = 1.0; 
    }

    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
        => this.messenger.Publish(new ZoomRequestMessage(e.NewValue));  

    private void OnButtonMaxClick(object? sender, RoutedEventArgs e)
        => this.Slider.Value = this.Slider.Maximum;

    private void OnButtonMinClick(object? sender, RoutedEventArgs e)
        => this.Slider.Value = this.Slider.Minimum;
}