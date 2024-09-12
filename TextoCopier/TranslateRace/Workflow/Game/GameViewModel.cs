using Lyt.TranslateRace.Messaging;

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

    private readonly IRandomizer randomizer;
    private readonly IAnimationService animationService;
    private readonly TranslateRaceModel translateRaceModel;

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
    private TimeSpan translateTime;
    private bool hasCalledFriend; 

    private GameResult? gameResults;
    private Parameters? parameters;

    public GameViewModel(
        TranslateRaceModel translateRaceModel,
        IRandomizer randomizer, IAnimationService animationService)
    {
        this.translateRaceModel = translateRaceModel;
        this.randomizer = randomizer;
        this.animationService = animationService;
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

    public Team? CurrentTeam => this.isLeftTurn ? this.leftTeam : this.rightTeam;

    public TeamProgressViewModel? CurrentTeamProgressViewModel => this.isLeftTurn ? this.LeftTeamScore : this.RightTeamScore;

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

        this.Difficulty = this.parameters.Difficulty;
        this.leftTeam = this.parameters.LeftTeam;
        this.rightTeam = this.parameters.RightTeam;
        this.isLeftTurn = true;
        this.leftPlayerIndex = 0;
        this.rightPlayerIndex = 0;
        this.LeftTeamScore.Update(0);
        this.RightTeamScore.Update(0);
        this.leftTeam.Score = 0;
        this.rightTeam.Score = 0;

        this.BeginTurn();

        this.gameResults = new() { Difficulty = this.Difficulty };
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
    }

    private void UpdateUiComponents()
    {
        if ( this.CurrentTeam is not Team team )
        {
            throw new Exception("No team ???");
        }

        switch (this.TurnStep)
        {
            default:
            case GameStep.DifficultySelection:
                this.hasCalledFriend = false; 
                this.Options.Visible = true;
                this.Options.Update(team);
                this.Phrase.Visible = false;
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
                this.Phrase.Update(team, this.translateRaceModel.PickPhrase(this.phraseDifficulty));
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
                this.Evaluation.Update(team, this.phraseDifficulty);
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
                    team, this.phraseDifficulty, this.evaluationResult, this.translateTime, this.hasCalledFriend);
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

        this.TurnStep = GameStep.Translate;        
    }

    private void OnPlayerLifeline(PlayerLifelineMessage message)
    {
        if ((this.State != GameState.Running) || (this.TurnStep != GameStep.Translate))
        {
            return;
        }

        // TODO: Choose a player of pick at random ??? 
        this.hasCalledFriend = true;
    }

    private void OnTranslateRevealed(TranslateRevealedMessage message)
    {
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

        // TODO: Check Game Over 

        // TODO: Next Turn !!!

        this.TurnStep = GameStep.DifficultySelection;
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

    #region Bound properties 

    public string LeftTeamName { get => this.Get<string>()!; set => this.Set(value); }

    public string RightTeamName { get => this.Get<string>()!; set => this.Set(value); }

    public TeamProgressViewModel LeftTeamScore { get => this.Get<TeamProgressViewModel>()!; set => this.Set(value); }

    public TeamProgressViewModel RightTeamScore { get => this.Get<TeamProgressViewModel>()!; set => this.Set(value); }

    public TurnViewModel Turn { get => this.Get<TurnViewModel>()!; set => this.Set(value); }

    public OptionsViewModel Options { get => this.Get<OptionsViewModel>()!; set => this.Set(value); }

    public PhraseViewModel Phrase { get => this.Get<PhraseViewModel>()!; set => this.Set(value); }

    public EvaluationViewModel Evaluation { get => this.Get<EvaluationViewModel>()!; set => this.Set(value); }

    public CountdownViewModel Countdown { get => this.Get<CountdownViewModel>()!; set => this.Set(value); }

    public ScoreViewModel Score { get => this.Get<ScoreViewModel>()!; set => this.Set(value); }

    #endregion Bound properties 
}