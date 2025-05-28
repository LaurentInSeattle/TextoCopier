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

public sealed partial class SetupViewModel : ViewModel<SetupView>
{
    private readonly InvasionModel invasionModel;

    private readonly GameOptions gameOptions;

    [ObservableProperty]
    private bool debugVisible;

    [ObservableProperty]
    private PlayersSetup playerCount;

    [ObservableProperty]
    private AiPlayersSetup aiPlayerCount;

    [ObservableProperty]
    private bool aiPlayerZero;

    [ObservableProperty]
    private bool aiPlayerOne;

    [ObservableProperty]
    private bool aiPlayerTwo;

    [ObservableProperty]
    private bool aiPlayerThree;

    [ObservableProperty]
    private MapSize size;

    [ObservableProperty]
    private GameDifficulty difficulty;

    public SetupViewModel(InvasionModel invasionModel)
    {
        this.invasionModel = invasionModel;

        this.PlayerCount = PlayersSetup.Duel;
        this.AiPlayerCount = AiPlayersSetup.One;
        this.Size = MapSize.Medium;
        this.Difficulty = GameDifficulty.Fair;
        this.DebugVisible = Debugger.IsAttached;
        this.gameOptions = new GameOptions();
    }

    partial void OnPlayerCountChanged(PlayersSetup value)
    {
        int maxAiPlayers = (int)value - 1;
        this.AiPlayerThree = maxAiPlayers == 3;
        this.AiPlayerTwo = maxAiPlayers == 3 || maxAiPlayers == 2;
        this.AiPlayerOne = maxAiPlayers == 3 || maxAiPlayers == 2 || maxAiPlayers == 1;
        this.AiPlayerCount = AiPlayersSetup.None;
    }

    [RelayCommand]
    public void OnExit() => this.Messenger.Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnNext()
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
}