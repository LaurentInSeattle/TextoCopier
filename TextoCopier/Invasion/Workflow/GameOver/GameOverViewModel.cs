﻿namespace Lyt.Invasion.Workflow.GameOver;

using static ViewActivationMessage;

public sealed class GameOverViewModel : Bindable<GameOverView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

    public GameOverViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster)
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    private void OnPlay(object? _) => this.Messenger.Publish(ActivatedView.Setup);

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public ICommand PlayCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
