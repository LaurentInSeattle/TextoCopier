namespace Lyt.Invasion.Workflow.Gameplay;

using Lyt.Avalonia.Mvvm.Behaviors.Visual;

public partial class GameView : UserControl, IView
{
    public GameView()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) =>
        {
            var animator = App.GetRequiredService<IAnimationService>();
            new AppearsOnMouseOverBehavior(animator).Attach(this.ZoomController); 

            if (this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            } 
        };
    } 
}