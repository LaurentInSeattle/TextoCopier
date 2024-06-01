using Lyt.TextoCopier.Utilities;

namespace Lyt.TextoCopier.Workflow;

public sealed class TemplateViewModel : Bindable<TemplateView>
{
    private readonly string groupName;
    private readonly Template template;
    private readonly TemplatesModel templatesModel;

    public TemplateViewModel(string groupName, Template template)
    {
        this.groupName = groupName;
        this.template = template;
        this.templatesModel = ApplicationBase.GetRequiredService<TemplatesModel>();

        base.DisablePropertyChangedLogging = true;

        this.Name = template.Name;
        this.Value = template.Value;
        this.CopyCommand = new Command(this.OnCopy);
        this.EditCommand = new Command(this.OnEdit);
        this.DeleteCommand = new Command(this.OnDelete);
        this.LinkCommand = new Command(this.OnLink);
        this.ViewCommand = new Command(this.OnView);
    }

    public bool ShowLink => this.template.IsLink;

    public bool ShowView => this.template.ShouldHide || this.template.Value.Length > 28;

    private async void OnCopy(object? _)
    {
        this.Logger.Info("Clicked on Copy!");
        var startupWindow = ApplicationBase.GetRequiredService<Window>();
        if (startupWindow.Clipboard is IClipboard clipboard)
        {
            await clipboard.SetTextAsync(this.Value);
            this.Logger.Info("Copied to clipboard.");
        }
    }

    private void OnEdit(object? _)
    {
        this.Logger.Info("Clicked on Edit!");
        //this.templatesModel.EditTemplateValue(Name, this.Value);
    }

    private void OnDelete(object? _)
    {
        this.Logger.Info("Clicked on Delete!");
        this.templatesModel.DeleteTemplate(this.groupName, this.template.Name, out string message);
        if ( !string.IsNullOrWhiteSpace(message))
        {
            this.Logger.Warning(message);
        }
    }

    private void OnLink(object? _)
    {
        this.Logger.Info("Clicked on Link!");
        string webUrl = this.Value;
        if (!string.IsNullOrWhiteSpace(webUrl))
        {
            WebUtilities.OpenWebUrl(webUrl);
        }
    }

    private void OnView(object? _)
    {
        this.Logger.Info("Clicked on View!");
        //this.templatesModel.EditTemplateValue(Name, this.Value);
    }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Value { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand CopyCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand EditCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand LinkCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ViewCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
