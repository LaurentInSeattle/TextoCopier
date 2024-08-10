using Lyt.Avalonia.Mvvm.Extensions;

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
    private readonly LocalizerModel localizer;
    private readonly WordsModel wordsModel;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;

    private DispatcherTimer dispatcherTimer;

    private Grid? selectedGrid ;
    private Queue<Tuple<string,string>>? wordQueue ;
    private List<WordBlockViewModel>? leftColumn;
    private List<WordBlockViewModel>? rightColumn;

    public GameViewModel(
        WordsModel wordsModel, LocalizerModel localizer, 
        IDialogService dialogService, IToaster toaster, IRandomizer randomizer)
    {
        this.wordsModel = wordsModel;
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.randomizer = randomizer;
        this.beNice = new Chooser<string>(this.randomizer, GameViewModel.beingNice);
        this.beMean = new Chooser<string>(this.randomizer, GameViewModel.beingMean);
        this.Messenger.Subscribe<WordClickMessage>(this.OnWordClick);
        this.State = GameState.Idle;
        this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200), IsEnabled = false };
        this.dispatcherTimer.Tick += this.OnDispatcherTimerTick;
    }

    public GameState State { get; private set; }

    public GameDifficulty Difficulty { get; private set; }

    private void OnDispatcherTimerTick(object? sender, EventArgs e)
    {
        if (this.State != GameState.Running)
        {
            return;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
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

    public void GameOver() => this.Messenger.Publish(ViewActivationMessage.ActivatedView.Setup);

    private void OnWordClick(WordClickMessage message)
    {
    }

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

    private void PopulateGrid ()
    {
        int wordCount = this.WordCount;
        int rowCount = this.RowCount;
        this.selectedGrid = this.GameGrid;

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
                var vm = new WordBlockViewModel();
                vm.CreateViewAndBind();
                WordBlockView view = vm.View;
                this.selectedGrid.Children.Add(view);
                view.SetValue(Grid.ColumnProperty, col);
                view.SetValue(Grid.RowProperty, row);
                view.SetValue(Grid.MarginProperty, new Thickness(20, 12, 20, 12));
                return vm;
            }

            var leftVm = CreateWordBlock(0, i );
            this.leftColumn.Add(leftVm);
            string italian = pair.Item1; 
            leftVm.Setup(italian, Language.Italian);

            var rightVm = CreateWordBlock(1, rightColumnIndices [i]);
            this.rightColumn.Add(rightVm);
            string english = pair.Item2;
            rightVm.Setup(english, Language.English);
        }
    }

    private int BonusSeconds
        => this.Difficulty switch
        {
            GameDifficulty.Medium => 2,
            GameDifficulty.Hard => 1,
            _ => 5, // Easy 
        };

    private int MalusSeconds
        => this.Difficulty switch
        {
            GameDifficulty.Medium => -5,
            GameDifficulty.Hard => -7,
            _ => -3, // Easy 
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
}