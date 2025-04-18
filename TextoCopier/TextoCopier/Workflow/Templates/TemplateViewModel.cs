﻿namespace Lyt.TextoCopier.Workflow.Templates;

public sealed class TemplateViewModel : Bindable<TemplateView>
{
    private readonly string groupName;
    private readonly Template template;
    private readonly Panel? parentPanel;
    private readonly IDialogService dialogService;

    public TemplateViewModel(string groupName, Template template, Panel? parentPanel)
    {
        this.groupName = groupName;
        this.template = template;
        this.parentPanel = parentPanel;
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();

        base.DisablePropertyChangedLogging = true;

        this.Name = template.Name;
        this.Value = template.Value;
        this.MaskedValue = template.ShouldHide ? "••••••••••" : this.Value;
    }

    public bool ShowLink => this.template.IsLink;

    public bool ShowView
        => (this.parentPanel is not null) && (this.template.ShouldHide || this.template.Value.Length > 28);

#pragma warning disable IDE0051 // Remove unused private members
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
        this.Messenger.Publish(new ViewActivationMessage(ViewActivationMessage.ActivatedView.EditTemplate, this.template));
    }

    private void OnDelete(object? _)
    {
        this.Logger.Info("Clicked on Delete!");
        var templatesModel = ApplicationBase.GetRequiredService<TemplatesModel>();
        templatesModel.DeleteTemplate(this.groupName, this.template.Name, out string message);
        if (!string.IsNullOrWhiteSpace(message))
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

    private void OnView(object? parameter)
    {
        if (parameter is not ButtonTag buttonTag)
        {
            return;
        }

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

#pragma warning restore IDE0051 // Remove unused private members

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

    public string MaskedValue { get => this.Get<string>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Value { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand CopyCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand EditCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand LinkCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ViewCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
