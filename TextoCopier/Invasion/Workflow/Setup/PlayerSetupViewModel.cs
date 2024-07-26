namespace Lyt.Invasion.Workflow.Setup;

using static ViewActivationMessage;

//{
//    MapSize = MapSize.Large,
//    Difficulty = GameDifficulty.Fair,
//    Players =
//    [
//         new PlayerInfo { Name = "Laurent", IsHuman =true, Color = "Crimson"},
//         new PlayerInfo { Name = "Annalisa", IsHuman =true, Color = "DarkTurquoise"},
//         new PlayerInfo { Name = "Oksana", Color = "DarkOrange"},
//         new PlayerInfo { Name = "Irina", Color = "HotPink"},
//    ],
//};

public sealed class PlayerSetupViewModel : Bindable<PlayerSetupView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

    private GameOptions gameOptions;

#pragma warning disable CS8618 
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Some non-nullable fields (ex: gameOptions)and properties get assigned when the view model is activated 
    public PlayerSetupViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster)
#pragma warning restore CS8618
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;

        this.PlayCommand = new Command(this.OnPlay);
        this.ExitCommand = new Command(this.OnExit);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not GameOptions gameOptions)
        {
            throw new ArgumentNullException(nameof(activationParameters));
        }

        this.gameOptions = gameOptions;
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    private void OnPlay(object? _) => this.Messenger.Publish(ActivatedView.Game, this.gameOptions);

    public ICommand PlayCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
