namespace Lyt.TranslateRace.Workflow.Results;

using static Lyt.TranslateRace.Messaging.ViewActivationMessage;

public sealed partial class GameOverViewModel(TranslateRaceModel wordsModel) : ViewModel<GameOverView>
{
    private readonly TranslateRaceModel wordsModel = wordsModel;

    private GameResult? results;
    private Statistics? statistics;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not GameResult results)
        {
            throw new ArgumentException("Invalid activation parameters.");
        }

        this.Profiler.FullGcCollect();
        this.results = results;
        this.statistics = this.wordsModel.Statistics();
        this.ShowResults();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.Profiler.FullGcCollect();
    }

    private void ShowResults()
    {
        if ((this.results is null) || (this.statistics is null) || (App.Current is null))
        {
            throw new Exception("no results or stats");
        }

        this.GameOver = "Fine dei Giochi";
        App.Current.TryGetResource("LightAqua_1_100", out object? resource);
        this.GameOverColor = resource is Brush brush ? brush : (Brush)Brushes.DodgerBlue;
        this.Plays =
            string.Format(
                "Giocato {0} partite per {1} minuti",
                this.statistics.Wins + this.statistics.Losses,
                (int)(0.5 + this.statistics.Duration.TotalMinutes));
        this.WinsLosses =
            string.Format("Vittorie: {0}  ~  Perdite: {1}", this.statistics.Wins, this.statistics.Losses);
        int percent =
            (int)(0.5 + 100.0 * this.statistics.Wins / (this.statistics.Wins + this.statistics.Losses));
        this.Percent = string.Format("Percentuale di Vittorie: {0}%", percent);
        this.Streaks =
            string.Format(
                "Serie: Più Lunga: {0}, In Corso: {1}",
                this.statistics.BestStreak, this.statistics.CurrentStreak);

        this.IsWon = this.results.IsWon ? "Hai Vinto!" : "Perdi... :(";
        this.IsWonColor = this.results.IsWon ? ColorTheme.ValidUiText : ColorTheme.BoxBorder;
        this.Duration = string.Format("Questa Partita: {0} seconds", (int)(0.5 + this.results.GameDuration.TotalSeconds));

        this.TotalMatches = string.Format("Totale Parole Abbinate: {0}", this.statistics.MatchedWordsCount);
        this.TotalMissed = string.Format("Somma degli Errori: {0}", this.statistics.MissedWordsCount);
        this.TotalClicks = string.Format("Tutti i Clic del Mouse: {0}", this.statistics.ClicksCount);
    }

    [RelayCommand]
    public void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnPlayAgain(object? _) => this.Messenger.Publish(ActivatedView.Setup);

    [ObservableProperty]
    private string? gameOver;

    [ObservableProperty]
    private Brush? gameOverColor;

    [ObservableProperty]
    private string? plays;

    [ObservableProperty]
    private string? winsLosses;

    [ObservableProperty]
    private string? percent;

    [ObservableProperty]
    private string? streaks;

    [ObservableProperty]
    private string? isWon;

    [ObservableProperty]
    private Brush? isWonColor;

    [ObservableProperty]
    private string? duration;

    [ObservableProperty]
    private string? matches;

    [ObservableProperty]
    private string? missed;

    [ObservableProperty]
    private string? clicks;

    [ObservableProperty]
    private string? totalMatches;

    [ObservableProperty]
    private string? totalMissed;

    [ObservableProperty]
    private string? totalClicks;
}
