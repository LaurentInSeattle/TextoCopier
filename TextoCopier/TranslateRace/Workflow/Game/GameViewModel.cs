namespace Lyt.TranslateRace.Workflow.Game;

public sealed class GameViewModel : Bindable<GameView>
{
    public enum GameState
    {
        Idle,
        Running,
        Over,
    }

    public sealed class Parameters(Team leftTeam, Team rightTeam)
    {
        public Team LeftTeam { get; private set; } = leftTeam;

        public Team RightTeam { get; private set; } = rightTeam;

        public GameDifficulty Difficulty { get; set; }
    }

    // TODO: Add more !
    private static readonly string[] beingNice =
    [
        "Che Bello!" , "Dunque!",  "Potente!" , "Strabiliante!" , "Stupefacente!",
        "Ben Fatto" , "Bene Bene.." , "Grande" , "Imponente", "Maestoso",
        "Eccezionale", "Stupendo!" , "Assordante" , "Bello..." , "Grandioso" ,
        "Spaventoso!" , "Forza!"
    ];

    // TODO: Add more !
    private static readonly string[] beingMean =
    [
        "Merda!",  "Cazzo!" , "Cazzata!" , "Accidenti!", "Maledizione" ,
        "Errore" , "Sbaglio" , "Mancanza", "Fallo", "Pecca",
        "Deficiente!" , "Stupido!" , "Cretino!" , "Scemo" ,  "Ottuso...",
        "Terribile"
    ];
    
    private readonly IRandomizer randomizer;
    private readonly IAnimationService animationService;
    private readonly TranslateRaceModel translateRaceModel;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;
    private readonly DispatcherTimer dispatcherTimer;

    private DateTime gameStart;
    private DateTime gameEnd;
    private int bonusMilliseconds;
    private int malusMilliseconds;
    private int missedWordsCount;
    private int matchedWordsCount;

    private Team? leftTeam;
    private Team? rightTeam;
    private bool isLeftTurn;
    private int leftPlayerIndex;
    private int rightPlayerIndex;

    private GameResult? gameResults;
    private Parameters? parameters;
    private Queue<Tuple<string, string>>? wordQueue;

    public GameViewModel(
        TranslateRaceModel translateRaceModel, 
        IRandomizer randomizer, IAnimationService animationService)
    {
        this.translateRaceModel = translateRaceModel;
        this.randomizer = randomizer;
        this.animationService = animationService;
        this.beNice = new Chooser<string>(this.randomizer, GameViewModel.beingNice);
        this.beMean = new Chooser<string>(this.randomizer, GameViewModel.beingMean);
        this.State = GameState.Idle;
        this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120), IsEnabled = false };
        this.dispatcherTimer.Tick += this.OnDispatcherTimerTick;

        this.LeftTeamName = Team.LeftName;
        this.RightTeamName = Team.RightName;
        this.LeftTeamScore =
            new TeamProgressViewModel(
                this.LeftTeamName, TranslateRaceModel.WinScore, ColorTheme.LeftBackground, ColorTheme.LeftForeground);
        this.LeftTeamScore.Update(0);
        this.RightTeamScore =
            new TeamProgressViewModel(
                this.RightTeamName, TranslateRaceModel.WinScore, ColorTheme.RightBackground, ColorTheme.RightForeground);
        this.RightTeamScore.Update(0);
    }

    public GameState State { get; private set; }

    public GameDifficulty Difficulty { get; private set; }

    #region Loading and Activation 

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("GameViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        var vm = new OptionsViewModel();
        vm.Bind(this.View.OptionsView);
        this.Options = vm;
        this.Logger.Debug("GameViewModel: OnViewLoaded complete");
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not Parameters parameters)
        {
            throw new ArgumentException("Invalid activation parameters.");
        }

        this.parameters = parameters;
        Schedule.OnUiThread(
            100,
            () =>
            {
                this.Start(parameters);
            }, DispatcherPriority.Normal);
    }

    public override void Deactivate()
    {
        base.Deactivate();

        this.wordQueue = null;
        this.Profiler.FullGcCollect();
    }

    #endregion Loading and Activation 

    private void OnDispatcherTimerTick(object? sender, EventArgs e)
    {
        if (this.State != GameState.Running)
        {
            return;
        }

        int duration = this.DurationMilliseconds + this.bonusMilliseconds - this.malusMilliseconds;
        int elapsed = (int)((DateTime.Now - this.gameStart).TotalMilliseconds);
        int left = duration - elapsed;
        this.CountDownTotal = (float)duration;
        this.CountDownValue = (float)left;
        var timeLeft = TimeSpan.FromMilliseconds(left);
        this.TimeLeft = string.Format("{0}:{1:D2}", timeLeft.Minutes, timeLeft.Seconds);

        if (((timeLeft.Minutes < 0) || (timeLeft.Seconds < 0)) ||
            ((timeLeft.Minutes == 0) && (timeLeft.Seconds == 0)))
        {
            this.GameOver();
            return;
        }

    }

    private void Start(Parameters parameters)
    {
        this.LeftTeamScore.Update(0);
        this.RightTeamScore.Update(0);
        this.leftTeam = parameters.LeftTeam;
        this.rightTeam = parameters.RightTeam;
        this.isLeftTurn = true;
        this.leftPlayerIndex = 0;
        this.rightPlayerIndex = 0;
        this.BeginTurn();

        this.Difficulty = parameters.Difficulty;
        this.gameResults = new() { Difficulty = this.Difficulty } ;
        this.TimeLeft = string.Empty;
        this.gameStart = DateTime.Now;
        this.bonusMilliseconds = 0;
        this.malusMilliseconds = 0;
        this.matchedWordsCount = 0;
        this.missedWordsCount = 0;
        this.wordQueue = new(this.WordCount);

        var words = this.translateRaceModel.RandomPicks(5 + this.WordCount);
        foreach (string word in words)
        {
            string translated;
            try
            {
                translated = this.translateRaceModel.TranslateToEnglish(word);
            }
            catch (Exception ex)
            {
                this.Logger.Warning("Game View Model: Start: " + ex.ToString());
                continue;
            }

            translated = translated.Trim();
            string trimmedWord = word.Trim();
            Debug.WriteLine(trimmedWord + "  -  " + translated);
            this.wordQueue.Enqueue(new Tuple<string, string>(trimmedWord, translated));

            if (this.wordQueue.Count == this.WordCount)
            {
                break;
            }
        }

        this.State = GameState.Running;

        Schedule.OnUiThread(
            500,
            () =>
            {
                this.dispatcherTimer.IsEnabled = true;
                this.dispatcherTimer.Start();
            }, DispatcherPriority.Normal);
    }

    private void BeginTurn()
    {
        if (this.leftTeam is null || this.rightTeam is null)
        {
            throw new Exception("Null Teams ???");
        }

        Team team = this.isLeftTurn ? this.leftTeam : this.rightTeam;
        Player player = this.isLeftTurn ? team.Players[this.leftPlayerIndex] : team.Players[this.rightPlayerIndex];
        Team nextTeam = !this.isLeftTurn ? this.leftTeam : this.rightTeam;
        Player nextPlayer = this.isLeftTurn ? nextTeam.Players[this.leftPlayerIndex] : nextTeam.Players[this.rightPlayerIndex];
        this.Turn = new TurnViewModel(team, player, nextTeam, nextPlayer);
        this.Options.Update(team, player); 
    }

    private void GameOver()
    {
        this.SaveGame();
        this.State = GameState.Over;
        this.dispatcherTimer.Stop();
        this.dispatcherTimer.IsEnabled = false;
        this.CountDownValue = 0.0f;

        // All view models should hide at the end 

        this.Messenger.Publish(ViewActivationMessage.ActivatedView.GameOver, this.gameResults);
    }

    private void SaveGame ()
    {
        if (this.gameResults is null)
        {
            throw new ArgumentNullException("no game results");
        }

        this.gameEnd = DateTime.Now;
        this.gameResults.GameDuration = this.gameEnd - this.gameStart;
        this.gameResults.WordCount = this.WordCount;
        this.gameResults.MatchedWordsCount = this.matchedWordsCount;
        this.gameResults.MissedWordsCount = this.missedWordsCount;
        this.gameResults.ClicksCount = 0; 
        this.gameResults.IsWon = this.WordCount == this.matchedWordsCount;
        this.translateRaceModel.Add(this.gameResults);
        this.translateRaceModel.Save();
    }

    private void PopMessage(string text, Brush brush)
    {
        // TODO: Fade in and out
        // this.View.CountDownBarControl.Opacity = 0.15;
        this.Comment = text;
        this.CommentColor = brush;

        Schedule.OnUiThread(
            1500,
            () =>
            {
                // this.View.CountDownBarControl.Opacity = 1.0;
                this.Comment = string.Empty;
            }, DispatcherPriority.Background);
    }

    #region Properties calculated from game parameters 

    private int BonusMilliseconds
        => this.Difficulty switch
        {
            GameDifficulty.Medium => 2_000,
            GameDifficulty.Hard => 1_000,
            _ => 5_000, // Easy 
        };

    private int MalusMilliseconds
        => this.Difficulty switch
        {
            GameDifficulty.Medium => 5_000,
            GameDifficulty.Hard => 7_000,
            _ => 3_000, // Easy 
        };

    private int RowCount
        => this.Difficulty switch
        {
            GameDifficulty.Medium => 5,
            GameDifficulty.Hard => 6,
            _ => 4, // Easy 
        };

    private int WordCount
        => this.Difficulty switch
        {
            GameDifficulty.Medium => 30,
            GameDifficulty.Hard => 40,
            // _ => 8, // DEBUG !!! 
            _ => 20, // Easy 
        };

    private int DurationMilliseconds
        => this.Difficulty switch
        {
            GameDifficulty.Medium => 105_000, // 1 minute 45 sec
            GameDifficulty.Hard => 120_000, // 2 minutes 
            _ => 90_000, // Easy : 1 minute 30 sec
        };

    #endregion Properties calculated from game parameters 

    #region Bound properties 

    public string LeftTeamName { get => this.Get<string>()!; set => this.Set(value); }

    public string RightTeamName { get => this.Get<string>()!; set => this.Set(value); }

    public TeamProgressViewModel LeftTeamScore { get => this.Get<TeamProgressViewModel>()!; set => this.Set(value); }

    public TeamProgressViewModel RightTeamScore { get => this.Get<TeamProgressViewModel>()!; set => this.Set(value); }

    public TurnViewModel Turn { get => this.Get<TurnViewModel>()!; set => this.Set(value); }

    public OptionsViewModel Options { get => this.Get<OptionsViewModel>()!; set => this.Set(value); }

    public string Comment { get => this.Get<string>()!; set => this.Set(value); }

    public Brush CommentColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public float CountDownTotal { get => this.Get<float>(); set => this.Set(value); }

    public float CountDownValue { get => this.Get<float>(); [DoNotLog] set => this.Set(value); }

    public string TimeLeft { get => this.Get<string>()!; [DoNotLog] set => this.Set(value); }
    #endregion Bound properties 
}