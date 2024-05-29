namespace Lyt.TextoCopier.Workflow;

public partial class TemplateView : UserControl
{
    public TemplateView()
    {
        this.InitializeComponent();
        this.PointerEntered += this.OnPointerEnter;
        this.PointerExited += this.OnPointerLeave;
        this.SetVisibility(visible: false);
    }

    ~TemplateView()
    {
        this.PointerEntered -= this.OnPointerEnter;
        this.PointerExited -= this.OnPointerLeave;
    }

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        if ((sender is TemplateView view) && (this == view))
        {
            this.SetVisibility(visible: true);
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        if ((sender is TemplateView view) && (this == view))
        {
            this.SetVisibility(visible: false);
        }
    }

    private void SetVisibility(bool visible)
    {
        this.outerBorder.BorderThickness = new Thickness(visible ? 1.0 : 0.0); 
        this.copyButton.IsVisible = visible;
        this.editButton.IsVisible = visible;
        this.deleteButton.IsVisible = visible;
    }
}
