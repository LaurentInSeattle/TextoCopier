namespace Lyt.TranslateRace.Workflow.Game;

public sealed class GameViewModel : Bindable<GameView>
{
    public enum GameState
    {
        Idle,
        Running,
        Over,
    }

    public sealed class Parameters
    {

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
    private readonly TranslateRaceModel wordsModel;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;
    private readonly DispatcherTimer dispatcherTimer;

    private DateTime gameStart;
    private DateTime gameEnd;
    private int bonusMilliseconds;
    private int malusMilliseconds;
    private int missedWordsCount;
    private int matchedWordsCount;

    private GameResult? gameResults;
    private Parameters? parameters;
    private Queue<Tuple<string, string>>? wordQueue;

    public GameViewModel(
        TranslateRaceModel wordsModel, 
        IRandomizer randomizer, IAnimationService animationService)
    {
        this.wordsModel = wordsModel;
        this.randomizer = randomizer;
        this.animationService = animationService;
        this.beNice = new Chooser<string>(this.randomizer, GameViewModel.beingNice);
        this.beMean = new Chooser<string>(this.randomizer, GameViewModel.beingMean);
        this.State = GameState.Idle;
        this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120), IsEnabled = false };
        this.dispatcherTimer.Tick += this.OnDispatcherTimerTick;
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
        this.Start(parameters);
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
        this.Difficulty = parameters.Difficulty;
        this.gameResults = new() { Difficulty = this.Difficulty } ;
        this.TimeLeft = string.Empty;
        this.WordsDiscovered = string.Format("{0}/{1}", 0, this.WordCount);
        this.gameStart = DateTime.Now;
        this.bonusMilliseconds = 0;
        this.malusMilliseconds = 0;
        this.matchedWordsCount = 0;
        this.missedWordsCount = 0;
        this.wordQueue = new(this.WordCount);
        var words = this.wordsModel.RandomPicks(5 + this.WordCount);
        foreach (string word in words)
        {
            string translated;
            try
            {
                translated = this.wordsModel.TranslateToEnglish(word);
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
        this.wordsModel.Add(this.gameResults);
        this.wordsModel.Save();
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

    private Grid GameGrid
        => this.Difficulty switch
        {
            GameDifficulty.Medium => this.View.MediumGrid,
            GameDifficulty.Hard => this.View.DifficultGrid,
            _ => this.View.EasyGrid, // Easy 
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

    public string TimeLeft { get => this.Get<string>()!; [DoNotLog] set => this.Set(value); }

    public string WordsDiscovered { get => this.Get<string>()!; set => this.Set(value); }

    public string Bonus { get => this.Get<string>()!; set => this.Set(value); }

    public Brush BonusColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public string Comment { get => this.Get<string>()!; set => this.Set(value); }

    public Brush CommentColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public float CountDownTotal { get => this.Get<float>(); set => this.Set(value); }

    public float CountDownValue { get => this.Get<float>(); [DoNotLog] set => this.Set(value); }

    #endregion Bound properties 
}