namespace Lyt.WordRush.Workflow.Game;

public sealed class GameViewModel : Bindable<GameView>
{
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard,
    }

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

    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IRandomizer randomizer;
    private readonly IAnimationService animationService; 
    private readonly LocalizerModel localizer;
    private readonly WordsModel wordsModel;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;

    private DispatcherTimer dispatcherTimer;
    private DateTime gameStart;
    private int bonusMilliseconds;
    private int malusMilliseconds;

    private Grid? selectedGrid;
    private Queue<Tuple<string, string>>? wordQueue;
    private List<WordBlockViewModel>? leftColumn;
    private List<WordBlockViewModel>? rightColumn;
    private WordBlockViewModel? selectedWord;
    private int wordsDiscovered; 

    public GameViewModel(
        WordsModel wordsModel, LocalizerModel localizer,
        IDialogService dialogService, IToaster toaster, IRandomizer randomizer, IAnimationService animationService)
    {
        this.wordsModel = wordsModel;
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
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

    private void OnDispatcherTimerTick(object? sender, EventArgs e)
    {
        if (this.State != GameState.Running)
        {
            return;
        }

        this.UpdateTimeLeft();
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

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not Parameters parameters)
        {
            throw new ArgumentException("Invalid activation parameters.");
        }

        this.Difficulty = parameters.Difficulty;
        this.Start();
    }

    public void Start()
    {
        this.gameStart = DateTime.Now;
        this.bonusMilliseconds = 0;
        this.malusMilliseconds = 0;
        this.wordsDiscovered = 0;
        this.wordQueue = new(this.WordCount);
        var words = this.wordsModel.RandomPicks(this.WordCount, []);
        foreach (var word in words)
        {
            string translated = this.wordsModel.TranslateToEnglish(word);
            translated = translated.Trim();
            string trimmedWord = word.Trim();
            Debug.WriteLine(trimmedWord + "  -  " + translated);
            this.wordQueue.Enqueue(new Tuple<string, string>(trimmedWord, translated));
        }

        this.PopulateGrid();
        this.dispatcherTimer.IsEnabled = true;
        this.dispatcherTimer.Start();
        this.State = GameState.Running;
    }

    public void GameOver()
    {
        this.dispatcherTimer.Stop();
        this.dispatcherTimer.IsEnabled = false;
        this.CountDownValue = 0.0f;
        this.View.CountDownBarControl.AdjustForegroundSize();
        this.TimeLeft = "Fine dei Giochi";

        // this.Messenger.Publish(ViewActivationMessage.ActivatedView.Setup);
    }

    private void OnWordClick(WordClickMessage message)
    {
        if (this.State != GameState.Running)
        {
            return;
        }

        if (this.HasSelection)
        {
            // We already have a selection 
            if (this.selectedWord!.Language == message.Language)
            {
                // Same languages: just change selection 
                this.selectedWord.Select(select:false);
                this.selectedWord = message.WordBlockViewModel;
                this.selectedWord.Select();
            }
            else
            {
                // Distinct languages: do we have a match ? 
                var clickedVm = message.WordBlockViewModel;
                if ( this.selectedWord.MatchWord == clickedVm.OriginalWord)
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

    private void Score(WordBlockViewModel wordBlockViewModel,  bool isGood )
    {
        if (!this.HasSelection)
        {
            return;
        }

        // Adjust colors 
        this.selectedWord!.BackgroundBrush = isGood ? ColorTheme.BoxExact : ColorTheme.UiText;
        wordBlockViewModel.BackgroundBrush = isGood ? ColorTheme.BoxExact : ColorTheme.UiText;
        this.selectedWord!.ForegroundBrush = ColorTheme.Text;
        wordBlockViewModel.ForegroundBrush = ColorTheme.Text;

        // Fade 
        this.selectedWord!.Fadeout();
        wordBlockViewModel.Fadeout();

        bool popMessage = this.randomizer.NextBool();
        if (isGood)
        {
            ++this.wordsDiscovered;
            this.WordsDiscovered = string.Format("{0}/{1}", this.wordsDiscovered, this.WordCount);
            this.bonusMilliseconds += this.BonusMilliseconds;
            if (popMessage)
            {
                this.Comment = this.beNice.Next();
                this.CommentColor = ColorTheme.BoxExact;
            }
        }
        else
        {
            this.malusMilliseconds += this.MalusMilliseconds;
            if (popMessage)
            {
                this.Comment = this.beMean.Next();
                this.CommentColor = ColorTheme.UiText;
            }
        }

        // Both words unselected 
        this.selectedWord = null;

        // Todo: Fade out
        Schedule.OnUiThread( 1500, ()=> { this.Comment = string.Empty;  }, DispatcherPriority.Background );
    }

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

    private void UpdateTimeLeft()
    {
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
        }
    }

    private void PopulateGrid()
    {
        int rowCount = this.RowCount;
        this.selectedGrid = this.GameGrid;
        this.View.EasyGrid.IsVisible = this.Difficulty == GameDifficulty.Easy;
        this.View.MediumGrid.IsVisible = this.Difficulty == GameDifficulty.Medium;
        this.View.DifficultGrid.IsVisible = this.Difficulty == GameDifficulty.Hard;

        if ((this.wordQueue is null) || (this.selectedGrid is null))
        {
            this.Logger.Fatal("No words...");
            throw new Exception("No words...");
        }

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
            var pair = this.wordQueue.Dequeue();
            WordBlockViewModel CreateWordBlock(int col, int row)
            {
                var vm = new WordBlockViewModel(this.animationService);
                vm.CreateViewAndBind();
                WordBlockView view = vm.View;
                this.selectedGrid.Children.Add(view);
                view.SetValue(Grid.ColumnProperty, col);
                view.SetValue(Grid.RowProperty, row);
                view.SetValue(Grid.MarginProperty, new Thickness(20, 12, 20, 12));
                return vm;
            }

            string italian = pair.Item1;
            string english = pair.Item2;

            var leftVm = CreateWordBlock(0, i);
            this.leftColumn.Add(leftVm);
            leftVm.Setup(italian, english, Language.Italian);

            var rightVm = CreateWordBlock(1, rightColumnIndices[i]);
            this.rightColumn.Add(rightVm);
            rightVm.Setup(english, italian, Language.English);
        }
    }

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
            _ => 20, // Easy 
        };

    private int DurationMilliseconds
        => this.Difficulty switch
        {
            GameDifficulty.Medium => 150_000, // 2 minutes 30 sec
            GameDifficulty.Hard => 180_000, // 3 minutes 
            _ => 120_000, // Easy : 2 minutes 
        };

    public string TimeLeft { get => this.Get<string>()!; [DoNotLog] set => this.Set(value); }

    public string WordsDiscovered { get => this.Get<string>()!; set => this.Set(value); }

    public string Bonus { get => this.Get<string>()!; set => this.Set(value); }

    public Brush BonusColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public string Comment { get => this.Get<string>()!; set => this.Set(value); }

    public Brush CommentColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public float CountDownTotal { get => this.Get<float>(); set => this.Set(value); }

    public float CountDownValue { get => this.Get<float>(); [DoNotLog] set => this.Set(value); }
}