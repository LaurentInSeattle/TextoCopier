namespace Lyt.TranslateRace.Shell;

public partial class ShellView : UserControl, IView
{
    public ShellView()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
    }
}
