namespace Lyt.TranslateRace.Workflow.Setup;

public partial class PlayerView : UserControl
{
    public PlayerView()
    {
        this.InitializeComponent();
        this.SetInitialState(); 
        this.PointerEntered += this.OnPointerEnter;
        this.PointerExited += this.OnPointerLeave;
        this.Loaded += (s, e) => { this.SetVisibility(visible: false); };
    }

    ~PlayerView()
    {
        this.PointerEntered -= this.OnPointerEnter;
        this.PointerExited -= this.OnPointerLeave;
    }

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        if ((sender is PlayerView view) && (this == view))
        {
            this.SetVisibility(visible: true);
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        if ((sender is PlayerView view) && (this == view))
        {
            this.SetVisibility(visible: false);
        }
    }

    private void SetInitialState()
    {
        this.outerBorder.BorderThickness = new Thickness(0.0);
        this.leftButton.IsVisible = false;
        this.centerButton.IsVisible = false;
        this.rightButton.IsVisible = false;
    }

    private void SetVisibility(bool visible)
    {
        this.outerBorder.BorderThickness = new Thickness(visible ? 1.0 : 0.0);
        if (this.DataContext is PlayerViewModel vm)
        {
            this.leftButton.IsVisible = visible && vm.LeftIsVisible;
            this.centerButton.IsVisible = visible && vm.CenterIsVisible;
        }

        this.rightButton.IsVisible = visible;
    }
}