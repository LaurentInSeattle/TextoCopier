namespace Lyt.Invasion.Workflow.Setup;

using static ViewActivationMessage;

public sealed class SetupViewModel : Bindable<SetupView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

    private GameOptions gameOptions;

    public SetupViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster)
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;

        this.PlayCommand = new Command(this.OnPlay);
        this.ExitCommand = new Command(this.OnExit);

        this.gameOptions = new GameOptions
        {
            MapSize = MapSize.Large,
            Difficulty = GameDifficulty.Fair,
            Players =
            [
                 new PlayerInfo { Name = "Laurent", IsHuman =true, Color = "Red"},
                 new PlayerInfo { Name = "Annalisa", IsHuman =true, Color = "Blue"},
                 // new PlayerInfo { Name = "Oksana", Color = "Yellow"},
                 // new PlayerInfo { Name = "Irina", Color = "Magenta"},
            ],
        };
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

