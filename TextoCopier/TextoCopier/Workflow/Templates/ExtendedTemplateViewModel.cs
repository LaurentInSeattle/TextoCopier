namespace Lyt.TextoCopier.Workflow;

public sealed partial class ExtendedTemplateViewModel : ViewModel<ExtendedTemplateView> 
{
    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? stringValue;

    public void DuplicateFrom(TemplateViewModel templateViewModel)
    {
        this.Name = templateViewModel.Name;
        this.StringValue = templateViewModel.Value;
    }
}
