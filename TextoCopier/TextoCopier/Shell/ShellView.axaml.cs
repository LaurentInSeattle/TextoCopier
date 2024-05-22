namespace Lyt.TextoCopier.Shell;

public partial class ShellView : UserControl
{
    public ShellView() => this.InitializeComponent();

    public void OnButtonClick(object? sender, RoutedEventArgs args)
    {
        Debug.WriteLine("Click Button");
    }

    public void OnGlyphButtonClick(object? sender, RoutedEventArgs args)
    {
        Debug.WriteLine("Click Glyph Button");
    }
}
