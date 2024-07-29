
namespace Lyt.Invasion.Workflow.Gameplay;

public partial class GameView : UserControl
{
    public GameView()
    {
        this.InitializeComponent();
        this.Map.PointerMoved += this.OnGameViewPointerMoved;
        this.Map.PointerPressed += this.OnGameViewPointerPressed;
    }

    ~GameView()
    {
        this.Map.PointerMoved -= this.OnGameViewPointerMoved;
        this.Map.PointerPressed -= this.OnGameViewPointerPressed;
    }

    private void OnGameViewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Image)
        {
            return;
        }

        if (this.DataContext is GameViewModel gameViewModel)
        {
            // Send pixel position on the map to view model
            gameViewModel.OnPointerPressedOnMap(e.GetPosition(this.Map), e.KeyModifiers);
            e.Handled = true;
        }
    }

    private void OnGameViewPointerMoved(object? sender, PointerEventArgs e)
    {
        if ( sender is not Image)
        {
            return;
        }

        if (this.DataContext is GameViewModel gameViewModel)
        {
            // Send pixel position on the map to view model
            gameViewModel.OnPointerMovedOnMap(e.GetPosition(this.Map), e.KeyModifiers);
            e.Handled = true;
        }
    }
}