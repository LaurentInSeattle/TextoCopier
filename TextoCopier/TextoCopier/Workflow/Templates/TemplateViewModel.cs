namespace Lyt.TextoCopier.Workflow.Templates;

public sealed partial class TemplateViewModel : ViewModel<TemplateView>
{
    private readonly string groupName;
    private readonly Template template;
    private readonly Panel? parentPanel;
    private readonly IDialogService dialogService;

    [ObservableProperty]
    private string maskedValue;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string value;

    public TemplateViewModel(string groupName, Template template, Panel? parentPanel)
    {
        this.groupName = groupName;
        this.template = template;
        this.parentPanel = parentPanel;
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();

        this.Name = template.Name;
        this.Value = template.Value;
        this.MaskedValue = template.ShouldHide ? "••••••••••" : this.Value;
    }

    public bool ShowLink => this.template.IsLink;

    public bool ShowView
        => (this.parentPanel is not null) && (this.template.ShouldHide || this.template.Value.Length > 28);

    [RelayCommand]
    public async Task OnCopy()
    {
        this.Logger.Info("Clicked on Copy!");
        var startupWindow = ApplicationBase.GetRequiredService<Window>();
        if (startupWindow.Clipboard is IClipboard clipboard)
        {
            await clipboard.SetTextAsync(this.Value);
            this.Logger.Info("Copied to clipboard.");
        }
    }

    [RelayCommand]
    public void OnEdit()
    {
        this.Logger.Info("Clicked on Edit!");
        this.Messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.EditTemplate, this.template));
    }

    [RelayCommand]
    public void OnDelete()
    {
        this.Logger.Info("Clicked on Delete!");
        var templatesModel = ApplicationBase.GetRequiredService<TemplatesModel>();
        templatesModel.DeleteTemplate(this.groupName, this.template.Name, out string message);
        if (!string.IsNullOrWhiteSpace(message))
        {
            this.Logger.Warning(message);
        }
    }

    [RelayCommand]
    public void OnLink()
    {
        this.Logger.Info("Clicked on Link!");
        string webUrl = this.Value;
        if (!string.IsNullOrWhiteSpace(webUrl))
        {
            if (!WebUtilities.OpenWebUrl(webUrl, out string message))
            {
                this.Logger.Warning(message);
                var localizer = ApplicationBase.GetRequiredService<LocalizerModel>();
                IToaster toaster = ApplicationBase.GetRequiredService<IToaster>();
                toaster.Show(
                    localizer.Lookup("TemplateView.LinkNotFound.Title"), localizer.Lookup("TemplateView.LinkNotFound.Hint"),
                    12_000, InformationLevel.Warning);
            }
        }
    }

    [RelayCommand]
    public void OnView(ButtonTag buttonTag)
    {
        switch (buttonTag)
        {
            default:
            case ButtonTag.None:
            case ButtonTag.CountdownBegin:
            case ButtonTag.CountdownCancel:
            case ButtonTag.CountdownComplete:
            case ButtonTag.CountinuousContinue:
                return;

            case ButtonTag.CountinuousBegin:
                //Debugger.Break();
                this.ShowExtendedTemplate();
                break;
            case ButtonTag.CountinuousEnd:
                this.DismissExtendedTemplate();
                break;
        }
    }

    private void ShowExtendedTemplate()
    {
        if (this.parentPanel is null)
        {
            return;
        }

        this.Logger.Info("Clicked on View Begin!");

        var vm = new ExtendedTemplateViewModel();
        var view  = vm.CreateViewAndBind();
        vm.DuplicateFrom(this);

        this.dialogService.Show<ExtendedTemplateView>(this.parentPanel, view);
    }

    private void DismissExtendedTemplate()
    {
        if (this.dialogService.IsModal)
        {
            this.dialogService.Dismiss();
        }
    }
}
