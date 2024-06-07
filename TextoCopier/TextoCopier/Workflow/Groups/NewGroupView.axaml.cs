namespace Lyt.TextoCopier.Workflow;

public partial class NewGroupView : UserControl
{
    public NewGroupView()
    {
        this.InitializeComponent();
        this.NameTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.DescriptionTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.IconNameTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~NewGroupView()
    {
        this.NameTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
        this.DescriptionTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
        this.IconNameTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if ( this.DataContext is NewGroupViewModel newGroupViewModel)
        {
            newGroupViewModel.OnEditing();
            e.Handled = true;
        }
    }
}
