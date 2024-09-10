namespace Lyt.TranslateRace.Workflow.Results;

using static Lyt.TranslateRace.Messaging.ViewActivationMessage;

public sealed class GameOverViewModel : Bindable<GameOverView>
{
    private readonly TranslateRaceModel wordsModel;

    private GameResult? results;
    private Statistics? statistics;

    public GameOverViewModel(TranslateRaceModel wordsModel) => this.wordsModel = wordsModel;

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("SetupViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.Logger.Debug("SetupViewModel: OnViewLoaded complete");
    }

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

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    private void OnPlayAgain(object? _) => this.Messenger.Publish(ActivatedView.Setup);

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public string GameOver { get => this.Get<string>()!; set => this.Set(value); }

    public Brush GameOverColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public ICommand PlayAgainCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }


    public string Plays { get => this.Get<string>()!; set => this.Set(value); }

    public string WinsLosses { get => this.Get<string>()!; set => this.Set(value); }

    public string Percent { get => this.Get<string>()!; set => this.Set(value); }

    public string Streaks { get => this.Get<string>()!; set => this.Set(value); }


    public string IsWon { get => this.Get<string>()!; set => this.Set(value); }

    public Brush IsWonColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public string Duration { get => this.Get<string>()!; set => this.Set(value); }

    public string Matches { get => this.Get<string>()!; set => this.Set(value); }

    public string Missed { get => this.Get<string>()!; set => this.Set(value); }

    public string Clicks { get => this.Get<string>()!; set => this.Set(value); }

    public string TotalMatches { get => this.Get<string>()!; set => this.Set(value); }

    public string TotalMissed { get => this.Get<string>()!; set => this.Set(value); }

    public string TotalClicks { get => this.Get<string>()!; set => this.Set(value); }
}
