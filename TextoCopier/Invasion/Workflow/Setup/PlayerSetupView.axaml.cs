namespace Lyt.Invasion.Workflow.Setup;

public partial class PlayerSetupView : UserControl, IView
{
    public PlayerSetupView()
    {
        this.InitializeComponent();
        this.Loaded += (s,e) => { this.NameTextBox.Focus(); }; 
    } 
}