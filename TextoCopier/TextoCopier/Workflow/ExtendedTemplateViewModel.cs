namespace Lyt.TextoCopier.Workflow;

public sealed class ExtendedTemplateViewModel : Bindable<ExtendedTemplateView> 
{
    public void DuplicateFrom (TemplateViewModel templateViewModel)
    {
        this.Name = templateViewModel.Name;
        this.Value = templateViewModel.Value;
    }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Value { get => this.Get<string>()!; set => this.Set(value); }
}
