namespace Lyt.TextoCopier.Workflow.Templates;

public partial class NewEditTemplateView : UserControl
{
    public NewEditTemplateView()
    {
        this.InitializeComponent();
        this.NameTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.ValueTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~NewEditTemplateView()
    {
        this.NameTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
        this.ValueTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (this.DataContext is NewEditTemplateViewModel newEditTemplateViewModel)
        {
            newEditTemplateViewModel.OnEditing();
            e.Handled = true;
        }
    }
}
