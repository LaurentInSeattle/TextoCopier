namespace Lyt.TextoCopier.Workflow;

public partial class ExtendedTemplateView : UserControl
{
    public ExtendedTemplateView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => this.outerBorder.Opacity = 1.0;
    }
}
