namespace Lyt.TranslateRace.Workflow.Game;

public sealed class GameViewModel : Bindable<GameView>
{
    public enum GameState
    {
        Idle,
        Running,
        Over,
    }

    public enum GameStep
    {
        Idle,
        DifficultySelection,
        ThemeSelection,
        Translate,
        Evaluate,
        Score,
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
        // "Deficiente!" , "Stupido!" , "Cretino!" ,  // Maybe too mean...
        "Merda!",  "Cazzo!" , "Cazzata!" , "Accidenti!", "Maledizione" ,
        "Errore" , "Sbaglio" , "Mancanza", "Fallo", "Pecca",
        "Scemo" ,  "Ottuso...",
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
    private bool isViewLoaded;

    private Team? leftTeam;
    private Team? rightTeam;
    private bool isLeftTurn;
    private int leftPlayerIndex;
    private int rightPlayerIndex;
    private GameStep gameStep;
    private PhraseDifficulty phraseDifficulty;
    private EvaluationResult evaluationResult;

    private GameResult? gameResults;
    private Parameters? parameters;

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

        this.Messenger.Subscribe<DifficultyChoiceMessage>(this.OnDifficultyChoice);
        this.Messenger.Subscribe<PlayerDropMessage>(this.OnPlayerDrop);
        this.Messenger.Subscribe<PlayerLifelineMessage>(this.OnPlayerLifeline);
        this.Messenger.Subscribe<TranslateCompleteMessage>(this.OnTranslateComplete);
        this.Messenger.Subscribe<EvaluationResultMessage>(this.OnEvaluationResult);
        this.Messenger.Subscribe<ScoringCompleteMessage>(this.OnScoringComplete);
    }

    public GameStep TurnStep
    {
        get => this.gameStep;
        private set
        {
            if (this.gameStep != value)
            {
                this.gameStep = value;
                this.UpdateUiComponentsVisibility();
            }
        }
    }

    public Team? CurrentTeam => this.isLeftTurn ? this.leftTeam : this.rightTeam; 

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

        var vmOptions = new OptionsViewModel();
        vmOptions.Bind(this.View.OptionsView);
        this.Options = vmOptions;

        var vmPhrase = new PhraseViewModel();
        vmPhrase.Bind(this.View.PhraseView);
        this.Phrase = vmPhrase;

        var vmEvaluation = new EvaluationViewModel();
        vmEvaluation.Bind(this.View.EvaluationView);
        this.Evaluation = vmEvaluation;

        this.isViewLoaded = true;
        this.Logger.Debug("GameViewModel: OnViewLoaded complete");
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not Parameters parameters)
        {
            throw new ArgumentException("Invalid activation parameters.");
        }

        // Wait to make sure the view is loaded
        this.DelayedStart(parameters);
    }

    private async void DelayedStart(Parameters parameters)
    {
        int retries = 10;
        while (!this.isViewLoaded)
        {
            await Task.Delay(120);
            --retries;
            if (retries < 0)
            {
                throw new Exception("View not loaded ??? ");
            }
        }

        this.parameters = parameters;
        Dispatch.OnUiThread(this.Start);
    }

    public override void Deactivate()
    {
        base.Deactivate();

        this.Profiler.FullGcCollect();
    }

    #endregion Loading and Activation 

    private void Start()
    {
        if (this.parameters is null)
        {
            throw new ArgumentException("No parameters ???");
        }

        this.Difficulty = this.parameters.Difficulty;
        this.leftTeam = this.parameters.LeftTeam;
        this.rightTeam = this.parameters.RightTeam;
        this.isLeftTurn = true;
        this.leftPlayerIndex = 0;
        this.rightPlayerIndex = 0;
        this.LeftTeamScore.Update(0);
        this.RightTeamScore.Update(0);

        this.BeginTurn();

        this.gameResults = new() { Difficulty = this.Difficulty };
        this.TimeLeft = string.Empty;
        this.gameStart = DateTime.Now;

        this.State = GameState.Running;
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
        this.TurnStep = GameStep.DifficultySelection;
        this.Options.Update(team);
        this.Phrase.Update(team, this.translateRaceModel.PickPhrase(PhraseDifficulty.Insane));
        this.Evaluation.Update(team);
    }

    private void UpdateUiComponentsVisibility()
    {
        switch (this.TurnStep)
        {
            default:
            case GameStep.DifficultySelection:
                this.Options.Visible = true;
                this.Phrase.Visible = false;
                this.Evaluation.Visible = false;
                this.TimerIsVisible = false;
                break;

            case GameStep.ThemeSelection:
                this.Options.Visible = false;
                this.Phrase.Visible = false;
                this.Evaluation.Visible = false;
                this.TimerIsVisible = false;
                break;
            case GameStep.Translate:
                this.Options.Visible = false;
                this.Phrase.Visible = true;
                this.Evaluation.Visible = false;
                this.TimerIsVisible = true;
                break;
            case GameStep.Evaluate:
                this.Options.Visible = false;
                this.Phrase.Visible = true;
                this.Evaluation.Visible = true;
                this.TimerIsVisible = false;
                break;
            case GameStep.Score:
                this.Options.Visible = false;
                this.Phrase.Visible = true;
                this.Evaluation.Visible = false;
                this.TimerIsVisible = false;
                break;
        }
    }

    private void OnPlayerDrop(PlayerDropMessage message)
    {
        if (this.State != GameState.Running)
        {
            return;
        }

        // TODO 
    }

    private void OnDifficultyChoice(DifficultyChoiceMessage message)
    {
        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.DifficultySelection))
        {
            return;
        }

        this.phraseDifficulty = message.PhraseDifficulty;
        var phrase = this.translateRaceModel.PickPhrase(this.phraseDifficulty);
        var team = this.CurrentTeam;
        if (phrase is null || team is null)
        {
            throw new Exception("No phrase, no team ???");
        }

        this.Phrase.Update(team, phrase);
        // TODO: Launch timer 
        this.TurnStep = GameStep.Translate;        
    }

    private void OnPlayerLifeline(PlayerLifelineMessage message)
    {
        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.Translate))
        {
            return;
        }

        // TODO: Choose a player of pick at random ??? 
    }


    private void OnTranslateComplete(TranslateCompleteMessage message)
    {
        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.Translate))
        {
            return;
        }

        this.TurnStep = GameStep.Evaluate;
    }

    private void OnEvaluationResult(EvaluationResultMessage message)
    {
        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.Evaluate))
        {
            return;
        }

        this.evaluationResult = message.Result;
        this.TurnStep = GameStep.Score;
    }

    private void OnScoringComplete(ScoringCompleteMessage message)
    {
        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.Score))
        {
            return;
        }

        // TODO: Check Game Over 

        // TODO: Next Turn !!!

        this.TurnStep = GameStep.DifficultySelection;
    }

    #region Timer 

    private void OnDispatcherTimerTick(object? sender, EventArgs e)
    {
        if (this.State != GameState.Running)
        {
            return;
        }

        int duration = this.DurationMilliseconds;
        int elapsed = (int)((DateTime.Now - this.gameStart).TotalMilliseconds);
        int left = duration - elapsed;
        this.CountDownTotal = (float)duration;
        this.CountDownValue = (float)left;
        var timeLeft = TimeSpan.FromMilliseconds(left);
        this.TimeLeft = string.Format("{0}:{1:D2}", timeLeft.Minutes, timeLeft.Seconds);

        if (((timeLeft.Minutes < 0) || (timeLeft.Seconds < 0)) ||
            ((timeLeft.Minutes == 0) && (timeLeft.Seconds == 0)))
        {
            this.StopTimer();
            return;
        }
    }

    private void StartTimer()
    {
        Schedule.OnUiThread(
            500,
            () =>
            {
                this.dispatcherTimer.IsEnabled = true;
                this.dispatcherTimer.Start();
            }, DispatcherPriority.Normal);
    }

    private void StopTimer()
    {
        this.dispatcherTimer.Stop();
        this.dispatcherTimer.IsEnabled = false;
        this.CountDownValue = 0.0f;
    }

    #endregion Timer 

    private void GameOver()
    {
        this.SaveGame();
        this.State = GameState.Over;

        // All view models should hide at the end 
        this.Messenger.Publish(ViewActivationMessage.ActivatedView.GameOver, this.gameResults);
    }

    private void SaveGame()
    {
        if (this.gameResults is null)
        {
            throw new ArgumentNullException("no game results");
        }

        this.gameEnd = DateTime.Now;
        this.gameResults.GameDuration = this.gameEnd - this.gameStart;
        this.gameResults.IsWon = true;
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

    public PhraseViewModel Phrase { get => this.Get<PhraseViewModel>()!; set => this.Set(value); }

    public EvaluationViewModel Evaluation { get => this.Get<EvaluationViewModel>()!; set => this.Set(value); }

    public string Comment { get => this.Get<string>()!; set => this.Set(value); }

    public Brush CommentColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public float CountDownTotal { get => this.Get<float>(); set => this.Set(value); }

    public float CountDownValue { get => this.Get<float>(); [DoNotLog] set => this.Set(value); }

    public bool TimerIsVisible { get => this.Get<bool>(); set => this.Set(value); }

    public string TimeLeft { get => this.Get<string>()!; [DoNotLog] set => this.Set(value); }

    #endregion Bound properties 
}