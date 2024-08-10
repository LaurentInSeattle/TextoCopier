namespace Lyt.WordRush.Workflow.Game;

public partial class WordBlockView : UserControl
{
    public WordBlockView()
    {
        this.InitializeComponent();
        this.PointerPressed += this.OnPointerPressed;
        this.PointerEntered += this.OnPointerEnter;
        this.PointerExited += this.OnPointerLeave;
    }

    ~WordBlockView()
    {
        this.PointerPressed -= this.OnPointerPressed;
        this.PointerEntered -= this.OnPointerEnter;
        this.PointerExited -= this.OnPointerLeave;
    }

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        if (this.DataContext is WordBlockViewModel wordBlockViewModel)
        {
            wordBlockViewModel.OnEnter();
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        if (this.DataContext is WordBlockViewModel wordBlockViewModel)
        {
            wordBlockViewModel.OnLeave();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        if (this.DataContext is WordBlockViewModel wordBlockViewModel)
        {
            wordBlockViewModel.OnClick();
        }
    }
}
