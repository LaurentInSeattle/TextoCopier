namespace PanZoom;

public partial class MainWindow : Window
{
    public MainWindow() 
    { 
        this.InitializeComponent();
        new MainWindowViewModel().Bind(this);
    }
}