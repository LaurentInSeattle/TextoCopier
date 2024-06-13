namespace Lyt.TextoCopier.Workflow;

public partial class NewEditGroupView : UserControl
{
    public NewEditGroupView()
    {
        this.InitializeComponent();
        this.NameTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.DescriptionTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.IconNameTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~NewEditGroupView()
    {
        this.NameTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
        this.DescriptionTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
        this.IconNameTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if ( this.DataContext is NewEditGroupViewModel newEditGroupViewModel)
        {
            newEditGroupViewModel.OnEditing();
            e.Handled = true;
        }
    }
}
