namespace Lyt.Avalonia.Mvvm.Dialogs;

public partial class ModalHostControl : UserControl
{
    private readonly Panel panel;

    private readonly Action<bool> onClose;

    public ModalHostControl(Panel panel, Action<bool> onClose)
    {
        this.InitializeComponent();
        this.panel = panel;
        this.onClose = onClose;
    }

    public void Close(bool dialogResult)
    {
        this.ContentGrid.Children.Clear();
        this.panel.Children.Remove(this);
        this.onClose(dialogResult);
    }
}
