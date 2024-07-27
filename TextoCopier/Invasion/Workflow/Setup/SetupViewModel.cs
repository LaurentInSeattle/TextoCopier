namespace Lyt.Invasion.Workflow.Setup;

using static ViewActivationMessage;

public enum PlayersSetup : int
{
    Duel = 2,
    Triad = 3,
    Clash = 4,
}

public enum AiPlayersSetup : int
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
}

public sealed class SetupViewModel : Bindable<SetupView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

    private readonly GameOptions gameOptions;

    public SetupViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster)
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;

        this.NotifyPropertyChanged(nameof(this.PlayerCount));

        this.PlayerCount = PlayersSetup.Duel;
        this.AiPlayerCount = AiPlayersSetup.One;
        this.Size = MapSize.Medium;
        this.Difficulty = GameDifficulty.Fair;
        this.DebugVisible = Debugger.IsAttached;
        this.gameOptions = new GameOptions();
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    private void OnPlayerCountChanged(PlayersSetup _, PlayersSetup newPlayersSetup)
    {
        int maxAiPlayers = (int)newPlayersSetup - 1;
        this.AiPlayerThree = maxAiPlayers == 3;
        this.AiPlayerTwo = maxAiPlayers == 3 || maxAiPlayers == 2;
        this.AiPlayerOne = maxAiPlayers == 3 || maxAiPlayers == 2 || maxAiPlayers == 1;
        this.AiPlayerCount = AiPlayersSetup.None;
    }

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    private void OnNext(object? _)
    {
        this.gameOptions.MapSize = this.Size;
        this.gameOptions.Difficulty = this.Difficulty;
        var players = this.gameOptions.Players;
        players.Clear();
        int ais = (int)this.AiPlayerCount;
        int humans = (int)this.PlayerCount - ais;
        this.Logger.Info(string.Format("Humans: {0} - Computer AI's: {1}", humans, ais));

        for (int i = 0; i < ais; i++)
        {
            players.Add(new PlayerInfo());
        }

        for (int i = 0; i < humans; i++)
        {
            players.Add(new PlayerInfo() { IsHuman = true });
        }

        this.Messenger.Publish(ActivatedView.PlayerSetup, this.gameOptions);
    }

    public bool DebugVisible { get => this.Get<bool>(); set => this.Set(value); }

    public PlayersSetup PlayerCount { get => this.Get<PlayersSetup>(); set => this.Set(value); }

    public AiPlayersSetup AiPlayerCount { get => this.Get<AiPlayersSetup>(); set => this.Set(value); }

    public bool AiPlayerZero { get => this.Get<bool>(); set => this.Set(value); }

    public bool AiPlayerOne { get => this.Get<bool>(); set => this.Set(value); }

    public bool AiPlayerTwo { get => this.Get<bool>(); set => this.Set(value); }

    public bool AiPlayerThree { get => this.Get<bool>(); set => this.Set(value); }

    public MapSize Size { get => this.Get<MapSize>(); set => this.Set(value); }

    public GameDifficulty Difficulty { get => this.Get<GameDifficulty>(); set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}