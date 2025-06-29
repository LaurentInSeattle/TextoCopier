namespace Lyt.TranslateRace.Workflow.Setup;

public partial class NewParticipantView : View
{
    public NewParticipantView() : base()
        => this.NameTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    ~NewParticipantView()
    {
        this.NameTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (this.DataContext is NewParticipantViewModel newParticipantViewModel)
        {
            newParticipantViewModel.OnEditing();
            e.Handled = true;
        }
    }
}
