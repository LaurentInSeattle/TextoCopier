namespace Lyt.TranslateRace.Workflow.Game;

public partial class GameView : UserControl, IView
{
    public GameView()
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