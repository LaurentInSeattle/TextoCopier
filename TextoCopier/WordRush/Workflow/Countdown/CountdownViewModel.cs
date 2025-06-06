﻿namespace Lyt.WordRush.Workflow.Countdown;

using static GameViewModel;

public sealed partial class CountdownViewModel : ViewModel<CountdownView>
{
    private Parameters? parameters;

    [ObservableProperty]
    private string comment;

    [ObservableProperty]
    private Brush commentColor;

    public CountdownViewModel()
    {
        this.Comment = string.Empty;
        this.CommentColor = ColorTheme.ValidUiText;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not Parameters parameters)
        {
            throw new ArgumentException("Invalid activation parameters.");
        }

        this.Profiler.FullGcCollect();
        this.parameters = parameters;
        this.Comment = string.Empty;
        this.StartCountdown();
    }

    private void StartCountdown()
    {
        // pronti, partenza, via 
        Schedule.OnUiThread(200,
            () =>
            {
                this.Comment = "Pronti ?";
                this.CommentColor = ColorTheme.ValidUiText;
            }, DispatcherPriority.Normal);

        Schedule.OnUiThread(1_400,
            () =>
            {
                this.Comment = "Partenza...";
                this.CommentColor = ColorTheme.BoxPresent;
            }, DispatcherPriority.Normal);

        Schedule.OnUiThread(2_600,
            () =>
            {
                this.Comment = "Vai!!!";
                this.CommentColor = ColorTheme.UiText;
            }, DispatcherPriority.Normal);

        Schedule.OnUiThread(3_800,
            () =>
            {
                this.Messenger.Publish( ViewActivationMessage.ActivatedView.Game, this.parameters );
            }, DispatcherPriority.Normal);
    }
}
