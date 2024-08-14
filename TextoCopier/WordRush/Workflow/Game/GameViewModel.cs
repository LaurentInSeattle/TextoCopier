namespace Lyt.WordRush.Workflow.Game;

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
    private readonly WordsModel wordsModel;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;
    private readonly DispatcherTimer dispatcherTimer;

    private DateTime gameStart;
    private DateTime gameEnd;
    private int bonusMilliseconds;
    private int malusMilliseconds;
    private int missedWordsCount;
    private int matchedWordsCount;
    private int clicksCount;

    private GameResult? gameResults;
    private Parameters? parameters;
    private Grid? selectedGrid;
    private Queue<Tuple<string, string>>? wordQueue;
    private List<WordBlockViewModel>? leftColumn;
    private List<WordBlockViewModel>? rightColumn;
    private WordBlockViewModel? selectedWord;

    public GameViewModel(
        WordsModel wordsModel, 
        IRandomizer randomizer, IAnimationService animationService)
    {
        this.wordsModel = wordsModel;
        this.randomizer = randomizer;
        this.animationService = animationService;
        this.beNice = new Chooser<string>(this.randomizer, GameViewModel.beingNice);
        this.beMean = new Chooser<string>(this.randomizer, GameViewModel.beingMean);
        this.Messenger.Subscribe<WordClickMessage>(this.OnWordClick);
        this.State = GameState.Idle;
        this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120), IsEnabled = false };
        this.dispatcherTimer.Tick += this.OnDispatcherTimerTick;
    }

    public GameState State { get; private set; }

    public GameDifficulty Difficulty { get; private set; }

    public bool HasSelection => this.selectedWord is not null;

    #region Loading and Activation 

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("GameViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Why do I need to do that ??? 
        this.View.CountDownBarControl.DataContext = this;

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
        this.Profiler.FullGcCollect();
        this.Start(parameters);
    }

    public override void Deactivate()
    {
        base.Deactivate();

        this.selectedGrid = null;
        this.wordQueue = null;
        this.leftColumn = null;
        this.rightColumn = null;
        this.selectedWord = null;
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

        if (this.wordQueue is null)
        {
            throw new Exception("Words ! ");
        }

        int available = (from vm in this.leftColumn where vm.IsAvailable select vm).Count();
        if ((available == this.RowCount) && (this.wordQueue.Count == 0))
        {
            this.GameOver();
            return;
        }

        if ((available > 1) && (this.wordQueue.Count > 0))
        {
            this.RefillGrid();
        }
    }

    private async void Start(Parameters parameters)
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
        var words = this.wordsModel.RandomPicks(5 + this.WordCount, []);
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

        await this.PopulateGrid();
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
        this.View.CountDownBarControl.AdjustForegroundSize();

        this.View.CountDownBarControl.IsVisible = false;

        // All view models should hide at the end 
        foreach (var vm in this.leftColumn!)
        {
            vm.Show(show: false);
        }

        foreach (var vm in this.rightColumn!)
        {
            vm.Show(show: false);
        }

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
        this.gameResults.ClicksCount = this.clicksCount;
        this.gameResults.IsWon = this.WordCount == this.matchedWordsCount;
        this.wordsModel.Add(this.gameResults);
        this.wordsModel.Save();
    }

    private void OnWordClick(WordClickMessage message)
    {
        if (this.State != GameState.Running)
        {
            return;
        }

        ++this.clicksCount;
        if (this.HasSelection)
        {
            // We already have a selection 
            if (this.selectedWord!.Language == message.Language)
            {
                // Same languages: just change selection 
                this.selectedWord.Select(select: false);
                this.selectedWord = message.WordBlockViewModel;
                this.selectedWord.Select();
            }
            else
            {
                // Distinct languages: do we have a match ? 
                var clickedVm = message.WordBlockViewModel;
                if (this.selectedWord.MatchWord == clickedVm.OriginalWord)
                {
                    // Match! 
                    this.Score(clickedVm, isGood: true);
                }
                else
                {
                    // Fail 
                    this.Score(clickedVm, isGood: false);
                }
            }
        }
        else
        {
            // First click: No selection, create one 
            this.selectedWord = message.WordBlockViewModel;
            this.selectedWord.Select();
        }
    }

    private void Score(WordBlockViewModel wordBlockViewModel, bool isGood)
    {
        if (!this.HasSelection)
        {
            return;
        }

        this.Logger.Debug("Score: " + (isGood ? "Match " : " Fail"));

        // Adjust colors 
        this.selectedWord!.BackgroundBrush = isGood ? ColorTheme.BoxExact : ColorTheme.UiText;
        wordBlockViewModel.BackgroundBrush = isGood ? ColorTheme.BoxExact : ColorTheme.UiText;
        this.selectedWord!.ForegroundBrush = ColorTheme.Text;
        wordBlockViewModel.ForegroundBrush = ColorTheme.Text;

        bool popMessage = this.randomizer.NextBool();
        if (isGood)
        {
            ++this.matchedWordsCount;

            // Fade out if correct
            this.selectedWord!.Fadeout();
            wordBlockViewModel.Fadeout();

            this.WordsDiscovered = string.Format("{0}/{1}", this.matchedWordsCount, this.WordCount);
            this.bonusMilliseconds += this.BonusMilliseconds;
            if (popMessage)
            {
                this.PopMessage(this.beNice.Next(), ColorTheme.ValidUiText);
            }
        }
        else
        {
            ++ this.missedWordsCount;

            // Remove Red after a bit 
            WordBlockViewModel wasSelectedWord = this.selectedWord;
            Schedule.OnUiThread(1500, () =>
            {
                if (wasSelectedWord.BackgroundBrush == ColorTheme.UiText)
                {
                    wasSelectedWord.BackgroundBrush = ColorTheme.BoxAbsent;
                }

                if (wordBlockViewModel.BackgroundBrush == ColorTheme.UiText)
                {
                    wordBlockViewModel.BackgroundBrush = ColorTheme.BoxAbsent;
                }
            }, DispatcherPriority.Normal);

            this.malusMilliseconds += this.MalusMilliseconds;
            if (popMessage && (this.Difficulty == GameDifficulty.Hard))
            {
                // We are mean only on Hard difficulty 
                this.PopMessage(this.beMean.Next(), ColorTheme.UiText);
            }
        }

        // Now unselected 
        this.selectedWord = null;
    }

    private void PopMessage(string text, Brush brush)
    {
        // TODO: Fade in and out
        this.View.CountDownBarControl.Opacity = 0.15;
        this.Comment = text;
        this.CommentColor = brush;

        Schedule.OnUiThread(
            1500,
            () =>
            {
                this.View.CountDownBarControl.Opacity = 1.0;
                this.Comment = string.Empty;
            }, DispatcherPriority.Background);
    }

    #region Populating the word options 

    private async Task PopulateGrid()
    {
        this.selectedGrid = this.GameGrid;

        if ((this.wordQueue is null) || (this.selectedGrid is null) || (this.gameResults is null))
        {
            this.Logger.Fatal("No words...");
            throw new Exception("No words...");
        }

        this.View.EasyGrid.IsVisible = this.Difficulty == GameDifficulty.Easy;
        this.View.MediumGrid.IsVisible = this.Difficulty == GameDifficulty.Medium;
        this.View.DifficultGrid.IsVisible = this.Difficulty == GameDifficulty.Hard;

        int rowCount = this.RowCount;
        var rightColumnIndices = new List<int>(rowCount);
        for (int i = 0; i < rowCount; ++i)
        {
            rightColumnIndices.Add(i);
        }

        this.randomizer.Shuffle(rightColumnIndices);

        this.selectedGrid.Children.Clear();
        this.leftColumn = new(rowCount);
        this.rightColumn = new(rowCount);
        for (int i = 0; i < rowCount; ++i)
        {
            await Task.Delay(500);
            var pair = this.wordQueue.Dequeue();
            this.gameResults.Words.Add(pair.Item1);
            this.FillOne(pair, i, rightColumnIndices[i]);
        }
    }

    private void RefillGrid()
    {
        if ((this.wordQueue is null) || (this.selectedGrid is null) || (this.gameResults is null))
        {
            this.Logger.Fatal("No words...");
            throw new Exception("No words...");
        }

        var leftAvailable = (from vm in this.leftColumn where vm.IsAvailable select vm).FirstOrDefault();
        var rightAvailable = (from vm in this.rightColumn where vm.IsAvailable select vm).FirstOrDefault();
        if (leftAvailable is null || rightAvailable is null)
        {
            throw new Exception("No spots???");
        }

        var pair = this.wordQueue.Dequeue();
        string italian = pair.Item1;
        string english = pair.Item2;
        leftAvailable.Setup(italian, english, Language.Italian);
        rightAvailable.Setup(english, italian, Language.English);
        this.gameResults.Words.Add(italian);
    }

    private void FillOne(Tuple<string, string> pair, int rowLeft, int rowRight)
    {
        WordBlockViewModel CreateWordBlock(int col, int row)
        {
            var vm = new WordBlockViewModel(this.animationService, this.randomizer);
            vm.CreateViewAndBind();
            WordBlockView view = vm.View;
            this.selectedGrid!.Children.Add(view);
            view.SetValue(Grid.ColumnProperty, col);
            view.SetValue(Grid.RowProperty, row);
            view.SetValue(Grid.MarginProperty, new Thickness(20, 12, 20, 12));
            return vm;
        }

        string italian = pair.Item1;
        string english = pair.Item2;

        var leftVm = CreateWordBlock(0, rowLeft);
        this.leftColumn!.Add(leftVm);
        leftVm.Setup(italian, english, Language.Italian);

        var rightVm = CreateWordBlock(1, rowRight);
        this.rightColumn!.Add(rightVm);
        rightVm.Setup(english, italian, Language.English);
    }

    #endregion Populating the word options 

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
            _ => 8, // DEBUG !!! 
            // _ => 20, // Easy 
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