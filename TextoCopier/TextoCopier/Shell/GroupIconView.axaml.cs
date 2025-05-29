namespace Lyt.TextoCopier.Shell;

public partial class GroupIconView : UserControl, IView
{
    public GroupIconView()
    {
        this.InitializeComponent();
        this.DataContextChanged += (s,e) =>
        {
            if (this.DataContext is GroupIconViewModel groupIconViewModel)
            {
                // There should be no reason 
                this.Icon.GlyphSource = groupIconViewModel.IconGlyphSource;
            }
        };
    }
}

