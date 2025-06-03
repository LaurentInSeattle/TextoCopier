namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class GameViewModel : ViewModel<GameView>
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
    }

    private readonly IRandomizer randomizer;
    private readonly TranslateRaceModel translateRaceModel;

    private bool isViewLoaded;

    private Team? leftTeam;
    private Team? rightTeam;
    private bool isLeftTurn;
    private int leftPlayerIndex;
    private int rightPlayerIndex;
    private GameStep gameStep;
    private PhraseDifficulty phraseDifficulty;
    private EvaluationResult evaluationResult;
    private TimeSpan translateTime;
    private bool hasCalledFriend;

    private Parameters? parameters;

    [ObservableProperty]
    private string? leftTeamName;

    [ObservableProperty]
    private string? rightTeamName;

    [ObservableProperty]
    private TeamProgressViewModel leftTeamScore;

    [ObservableProperty]
    private TeamProgressViewModel rightTeamScore;

    [ObservableProperty]
    private TurnViewModel? turn;

    [ObservableProperty]
    private OptionsViewModel? options;

    [ObservableProperty]
    private PhraseViewModel? phrase;

    [ObservableProperty]
    private EvaluationViewModel? evaluation;

    [ObservableProperty]
    private CountdownViewModel? countdown;

    [ObservableProperty]
    private ScoreViewModel? score;

    public GameViewModel(TranslateRaceModel translateRaceModel, IRandomizer randomizer)
    {
        this.translateRaceModel = translateRaceModel;
        this.randomizer = randomizer;
        this.State = GameState.Idle;

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
        this.Messenger.Subscribe<TranslateRevealedMessage>(this.OnTranslateRevealed);
        this.Messenger.Subscribe<EvaluationResultMessage>(this.OnEvaluationResult);
        this.Messenger.Subscribe<ScoringCompleteMessage>(this.OnScoringComplete);
        this.Messenger.Subscribe<ScoreUpdateMessage>(this.OnScoreUpdate);
    }

    public GameStep TurnStep
    {
        get => this.gameStep;
        private set
        {
            if (this.gameStep != value)
            {
                this.Logger.Info("Game Step: from " + this.gameStep.ToString() + " to " + value.ToString());
                this.gameStep = value;
                this.UpdateUiComponents();
            }
        }
    }

    public Team CurrentTeam
    {
        get
        {
            if (this.leftTeam is null || this.rightTeam is null)
            {
                throw new Exception("Null Teams ???");
            }

            return this.isLeftTurn ? this.leftTeam : this.rightTeam;
        }
    }

    public Team NextTeam
    {
        get
        {
            if (this.leftTeam is null || this.rightTeam is null)
            {
                throw new Exception("Null Teams ???");
            }

            return this.isLeftTurn ? this.rightTeam : this.leftTeam;
        }
    }

    public Player CurrentPlayer
    {
        get
        {
            if (this.leftTeam is null || this.rightTeam is null)
            {
                throw new Exception("Null Teams ???");
            }

            return
                this.isLeftTurn ?
                    this.leftTeam.At(this.leftPlayerIndex) :
                    this.rightTeam.At(this.rightPlayerIndex);
        }
    }

    public Player NextPlayer
    {
        get
        {
            if (this.leftTeam is null || this.rightTeam is null)
            {
                throw new Exception("Null Teams ???");
            }

            return
                this.isLeftTurn ?
                    this.rightTeam.At(this.rightPlayerIndex) :
                    this.leftTeam.At(this.leftPlayerIndex);
        }
    }
    public TeamProgressViewModel? CurrentTeamProgressViewModel => this.isLeftTurn ? this.LeftTeamScore : this.RightTeamScore;

    public GameState State { get; private set; }

    #region Loading and Activation 

    public override void OnViewLoaded()
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
        this.View.PhraseView.IsVisible = false;
        this.Phrase = vmPhrase;

        var vmEvaluation = new EvaluationViewModel();
        vmEvaluation.Bind(this.View.EvaluationView);
        this.View.EvaluationView.IsVisible = false;
        this.Evaluation = vmEvaluation;

        var vmCountdown = new CountdownViewModel();
        vmCountdown.Bind(this.View.CountdownView);
        this.View.CountdownView.IsVisible = false;
        this.Countdown = vmCountdown;

        var vmScore = new ScoreViewModel(this.randomizer);
        vmScore.Bind(this.View.ScoreView);
        this.View.ScoreView.IsVisible = false;
        this.Score = vmScore;

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

        this.leftTeam = this.parameters.LeftTeam;
        this.rightTeam = this.parameters.RightTeam;
        this.isLeftTurn = true;
        this.leftPlayerIndex = this.leftTeam.FirstPlayerIndex;
        this.rightPlayerIndex = this.rightTeam.FirstPlayerIndex;
        this.LeftTeamScore.Update(0);
        this.RightTeamScore.Update(0);
        this.leftTeam.Score = 0;
        this.rightTeam.Score = 0;

        this.BeginTurn();

        this.State = GameState.Running;
    }

    private void BeginTurn()
    {
        this.Turn = new TurnViewModel(this.CurrentTeam, this.CurrentPlayer, this.NextTeam, this.NextPlayer);
        this.TurnStep = GameStep.DifficultySelection;
    }

    private void NextTurn()
    {
        int nextPlayerIndex = FindNextPlayerIndex(this.CurrentTeam, this.CurrentPlayer);
        if (this.isLeftTurn)
        {
            this.leftPlayerIndex = nextPlayerIndex;
        }
        else
        {
            this.rightPlayerIndex = nextPlayerIndex;
        }

        this.isLeftTurn = !this.isLeftTurn;
    }

    private static int FindNextPlayerIndex(Team team, Player player)
    {
        var orderedPlayers = (from p in team.Players orderby p.Index ascending select p).ToList();
        var nextPlayer = (from p in orderedPlayers where p.Index > player.Index select p).FirstOrDefault();
        if (nextPlayer is null)
        {
            // Loop around 
            int teamFirstPlayerIndex = team.FirstPlayerIndex;
            nextPlayer = (from p in orderedPlayers where p.Index > teamFirstPlayerIndex select p).FirstOrDefault();
            if (nextPlayer is null)
            {
                throw new Exception("Unexpected: No Players ???");
            }
        }

        return nextPlayer.Index;
    }

    private void UpdateUiComponents()
    {
        if ((this.Phrase is null) ||
            (this.Options is null) ||
            (this.Evaluation is null) ||
            (this.Countdown is null) ||
            (this.Score is null) ||
            (this.Turn is null))
        {
            throw new Exception("Unexpected: Missing View Models");
        }

        switch (this.TurnStep)
        {
            default:
            case GameStep.DifficultySelection:

                this.Phrase.Visible = false;
                this.hasCalledFriend = false;
                this.Options.Visible = true;
                this.Options.Update(this.CurrentTeam);
                this.Evaluation.Visible = false;
                this.Countdown.Visible = false;
                this.Score.Visible = false;
                break;

            case GameStep.ThemeSelection:
                this.Options.Visible = false;
                this.Phrase.Visible = false;
                this.Evaluation.Visible = false;
                this.Countdown.Visible = false;
                this.Score.Visible = false;
                break;

            case GameStep.Translate:
                this.Options.Visible = false;
                this.Phrase.Visible = true;
                this.Evaluation.Visible = false;
                this.translateTime = TimeSpan.Zero;
                this.Countdown.Visible = true;
                this.Countdown.Start();
                this.Score.Visible = false;
                break;

            case GameStep.Evaluate:
                this.Options.Visible = false;
                this.Phrase.Visible = true;
                this.Evaluation.Update(this.CurrentTeam, this.phraseDifficulty);
                this.Evaluation.Visible = true;
                this.Countdown.Visible = false;
                this.Score.Visible = false;
                break;

            case GameStep.Score:
                this.Options.Visible = false;
                this.Phrase.Visible = true;
                this.Evaluation.Visible = false;
                this.Countdown.Visible = false;
                this.Score.Show(
                    this.CurrentTeam, this.phraseDifficulty, this.evaluationResult, this.translateTime, this.hasCalledFriend);
                this.Score.Visible = true;
                break;
        }
    }

    private void OnPlayerDrop(PlayerDropMessage message)
    {
        if (this.State != GameState.Running)
        {
            return;
        }

        // Must use current player before we removed it 
        int nextPlayerIndex = FindNextPlayerIndex(this.CurrentTeam, this.CurrentPlayer);
        if (!this.CurrentTeam.Drop(this.CurrentPlayer))
        {
            throw new Exception("Cant drop ??? ");
        }

        if (this.isLeftTurn)
        {
            this.leftPlayerIndex = nextPlayerIndex;
        }
        else
        {
            this.rightPlayerIndex = nextPlayerIndex;
        }

        // Dont change current team, but refresh both teams
        this.Turn = new TurnViewModel(this.CurrentTeam, this.CurrentPlayer, this.NextTeam, this.NextPlayer);
    }

    private void OnPlayerLifeline(PlayerLifelineMessage message)
    {
        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.Translate))
        {
            return;
        }

        // We called a friend: points to be discounted 
        this.hasCalledFriend = true;

        // For now , we choose the next player
        int nextPlayerIndex = FindNextPlayerIndex(this.CurrentTeam, this.CurrentPlayer);

        // Dont remove current player, just make current the one we just picked 
        if (this.isLeftTurn)
        {
            this.leftPlayerIndex = nextPlayerIndex;
        }
        else
        {
            this.rightPlayerIndex = nextPlayerIndex;
        }

        // Dont change current team, but refresh both teams
        this.Turn = new TurnViewModel(this.CurrentTeam, this.CurrentPlayer, this.NextTeam, this.NextPlayer);
    }

    private void OnDifficultyChoice(DifficultyChoiceMessage message)
    {
        if ((this.Phrase is null) ||
            (this.Options is null) ||
            (this.Evaluation is null) ||
            (this.Countdown is null) ||
            (this.Score is null) ||
            (this.Turn is null))
        {
            throw new Exception("Unexpected: Missing View Models");
        }

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

        this.Phrase.Update(this.CurrentTeam, phrase);
        this.TurnStep = GameStep.Translate;
    }

    private void OnTranslateRevealed(TranslateRevealedMessage message)
    {
        if ((this.Phrase is null) ||
            (this.Options is null) ||
            (this.Evaluation is null) ||
            (this.Countdown is null) ||
            (this.Score is null) ||
            (this.Turn is null))
        {
            throw new Exception("Unexpected: Missing View Models");
        }

        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.Translate))
        {
            return;
        }

        this.translateTime = this.Countdown.TimeUsed;
        this.Countdown.Stop();
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

        // Check Game Over 
        if (this.CurrentTeam.Score >= TranslateRaceModel.WinScore)
        {
            this.GameOver();
        }
        else
        {
            // Save player performance 
            ++this.CurrentPlayer.Traductions;
            this.CurrentPlayer.Points = message.ScoreUpdate;

            // Next Turn !!!
            this.NextTurn();
            this.BeginTurn();
        }
    }

    private void OnScoreUpdate(ScoreUpdateMessage message)
    {
        if ((this.State != GameState.Running) ||
            (this.TurnStep != GameStep.Score) ||
            this.CurrentTeamProgressViewModel is null)
        {
            return;
        }

        this.CurrentTeamProgressViewModel.Update(message.Score);
    }

    private void GameOver()
    {
        this.SaveGame();
        this.State = GameState.Over;

        // All view models should hide at the end 
        this.Messenger.Publish(ViewActivationMessage.ActivatedView.GameOver);
    }

    private void SaveGame()
    {
        this.translateRaceModel.WinningTeam = this.CurrentTeam;
        this.translateRaceModel.LosingTeam = this.NextTeam; 
        this.translateRaceModel.Save();
    }
}