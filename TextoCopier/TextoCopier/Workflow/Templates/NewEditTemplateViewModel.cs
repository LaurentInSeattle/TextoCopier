﻿namespace Lyt.TextoCopier.Workflow;

public sealed class NewEditTemplateViewModel : Bindable<NewEditTemplateView>
{
    private readonly IMessenger messenger;

    public NewEditTemplateViewModel()
    {
        this.messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.CloseCommand = new Command(this.OnClose);
        this.SaveCommand = new Command(this.OnSave);
    }

    public Template? EditedTemplate { get; private set; }

    public bool IsEditing => this.EditedTemplate != null;

    public override void Activate(object? activationParameter)
    {
        // TODO: 2 bools 
        if (activationParameter is Template template)
        {
            this.EditedTemplate = template;
            this.Name = template.Name;
            this.Value = template.Value;
        }
        else
        {
            this.EditedTemplate = null;
            this.Name = string.Empty;
            this.Value = string.Empty;
        }

        this.OnEditing();
    }

    public override void Deactivate() => this.EditedTemplate = null;

    private void OnSave(object? _)
    {
        if (this.Save(out string message))
        {
            this.OnClose(_);
        }
        else
        {
            this.ValidationMessage = message;
            this.SaveButtonIsDisabled = true;
        }
    }

    private void OnClose(object? _)
        => this.messenger.Publish(new ViewActivationMessage(ViewActivationMessage.StaticView.GoBack));

    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : message;
        this.SaveButtonIsDisabled = !validated;
    }

    private bool Validate(out string message)
    {
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        if( model.SelectedGroup is null)
        {
            message = TemplatesModel.NoSuchGroup;
            this.Logger.Info("model.SelectedGroup is null: "  + message);
            return false; 
        }

        string groupName = model.SelectedGroup.Name; 
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedGroup is not null
            return model.ValidateTemplateForEdit(groupName, this.Name, this.EditedTemplate!.Name, this.Value, out message);
        }
        else
        {
            return model.ValidateTemplateForAdd(groupName, this.Name, this.Value, out message);
        }
    }

    private bool Save(out string message)
    {
        if (!this.Validate(out message))
        {
            return false;
        }

        // Save to model 
        var model = ApplicationBase.GetRequiredService<TemplatesModel>();
        if (model.SelectedGroup is null)
        {
            message = TemplatesModel.NoSuchGroup;
            this.Logger.Info("model.SelectedGroup is null: " + message);
            return false;
        }

        string groupName = model.SelectedGroup.Name;
        if (this.IsEditing)
        {
            // if IsEditing, then this.EditedTemplate is not null
            // if (model.EditTemplate(groupName, this.Name, this.EditedTemplate!.Name, this.Value, this.IsLink, this.ShouldHide, out message)
            {
                return true;
            }
        }
        else
        {
            if (model.AddTemplate(groupName, this.Name, this.Value, this.IsLink, this.ShouldHide, out message))
            {
                return true;
            }
        }

        return false;
    }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Value { get => this.Get<string>()!; set => this.Set(value); }

    public bool IsLink { get => this.Get<bool>()!; set => this.Set(value); }

    public bool ShouldHide { get => this.Get<bool>()!; set => this.Set(value); }

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }

    public bool SaveButtonIsDisabled
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.View.SaveButton.IsDisabled = value;
        }
    }

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}