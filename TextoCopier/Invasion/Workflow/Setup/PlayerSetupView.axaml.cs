namespace Lyt.Invasion.Workflow.Setup;

public partial class PlayerSetupView : UserControl
{
    public PlayerSetupView()
    {
        this.InitializeComponent();
        this.Loaded += (s,e) => { this.NameTextBox.Focus(); }; 
    } 
}