namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class ScoreViewModel : ViewModel<ScoreView>
{
#if DEBUG
    private const int delay = 600;
    private const int shortenedDelay = delay - 100;
#else
    private const int delay = 1_300;
    private const int shortenedDelay = delay - 100;
#endif

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
        "Maledetta!", "Scemo" ,  "Ottuso...", "Terribile", 
        "Errore" , "Che Sbaglio" , "Mancanza", "Fallo", "Pecca"        
    ];

    private readonly IRandomizer randomizer;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;

    [ObservableProperty]
    private bool nextVisible;

    [ObservableProperty]
    private bool visible;

    [ObservableProperty]
    private string? comment;

    [ObservableProperty]
    private Brush? commentColor;

    [ObservableProperty]
    private IBrush teamColor;

    private int scoreUpdate;

    public ScoreViewModel(IRandomizer randomizer)
    {
        this.randomizer = randomizer;
        this.beNice = new Chooser<string>(this.randomizer, ScoreViewModel.beingNice);
        this.beMean = new Chooser<string>(this.randomizer, ScoreViewModel.beingMean);
        this.Visible = true;
        this.TeamColor = ColorTheme.LeftForeground;
        this.NextVisible = true; // So that we'll have a property changed 
    }

    public void Show(
        Team team,
        PhraseDifficulty phraseDifficulty, EvaluationResult evaluationResult, TimeSpan translateTime, bool hasCalledFriend)
    {
        this.NextVisible = false; // So that we'll have a property changed 
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.Visible = true;
        if (evaluationResult == EvaluationResult.Fail)
        {
            this.PopMessage(this.beMean.Next(), ColorTheme.BeMean);
        }
        else
        {
            this.PopMessage(this.beNice.Next(), ColorTheme.BeNice);
        }

        int teamScore = team.Score;
        int score = ScoreViewModel.DifficultyToScore(phraseDifficulty);
        int malus = ScoreViewModel.Malus(phraseDifficulty, evaluationResult);
        int lifeline = ScoreViewModel.LifelineMalus(hasCalledFriend);
        int timeBonus = ScoreViewModel.TimeBonus(translateTime);
        this.scoreUpdate = score - malus - lifeline + timeBonus;
        if (this.scoreUpdate < 0)
        {
            this.scoreUpdate = 0;
        }

        // Baseline 
        Schedule.OnUiThread(
            delay,
            () =>
            {
                string message = ScoreViewModel.DifficultyToScoreText(phraseDifficulty);
                this.PopMessage(message, ColorTheme.Text);
                new ScoreUpdateMessage(teamScore + score).Publish();
            }, DispatcherPriority.Background);

        // Malus 
        Schedule.OnUiThread(
            2 * delay,
            () =>
            {
                string message = ScoreViewModel.MalusText(evaluationResult, malus);
                this.PopMessage(message, ColorTheme.Text);
                new ScoreUpdateMessage(teamScore + score - malus).Publish();
            }, DispatcherPriority.Background);

        // Called a Friend 
        Schedule.OnUiThread(
            3 * delay,
            () =>
            {
                string message = ScoreViewModel.LifelineText(hasCalledFriend);
                this.PopMessage(message, ColorTheme.Text);
                new ScoreUpdateMessage(teamScore + score - malus - lifeline).Publish();
            }, DispatcherPriority.Background);

        // Time Bonus
        Schedule.OnUiThread(
            4 * delay,
            () =>
            {
                string message = ScoreViewModel.TimeBonusText(translateTime);
                this.PopMessage(message, ColorTheme.Text);
                new ScoreUpdateMessage(teamScore + score - malus - lifeline + timeBonus).Publish();
            }, DispatcherPriority.Background);

        // Final 
        Schedule.OnUiThread(
            5 * delay,
            () =>
            {
                string message = "Punteggio non è cambiato.";
                if (this.scoreUpdate > 0)
                {
                    message = string.Format("Punteggio aumentato di: {0}" , this.scoreUpdate);
                }
                this.PopMessage(message, ColorTheme.Text, hide:false);

                team.Score = teamScore + this.scoreUpdate;
                this.NextVisible = true;
                new ScoreUpdateMessage(teamScore + score - malus - lifeline + timeBonus).Publish();
            }, DispatcherPriority.Background);
    }

    private void PopMessage(string text, Brush brush, bool hide = true)
    {
        this.Logger.Info(text); 
        this.Comment = text;
        this.CommentColor = brush;
        if (hide)
        {
            Schedule.OnUiThread(shortenedDelay, () => { this.Comment = string.Empty; }, DispatcherPriority.Background);
        } 
    }

    private static int LifelineMalus(bool hasCalledFriend) => hasCalledFriend ? 2 : 0;

    private static int TimeBonus(TimeSpan timeSpan)
    {
        int seconds = (int)timeSpan.TotalSeconds;
        if (seconds < 15)
        {
            return 2;
        }
        else if (seconds < 25)
        {
            return 1;
        }

        return 0;
    }

    private static int DifficultyToScore(PhraseDifficulty phraseDifficulty)
        => phraseDifficulty switch
        {
            PhraseDifficulty.Medium => 3,
            PhraseDifficulty.Hard => 5,
            PhraseDifficulty.Insane => 7,
            _ => 1,
        };

    private static int Malus(PhraseDifficulty phraseDifficulty, EvaluationResult evaluationResult)
    {
        if (evaluationResult == EvaluationResult.Fail)
        {
            return ScoreViewModel.DifficultyToScore(phraseDifficulty);
        }

        return evaluationResult == EvaluationResult.Perfect ? 0 : 2;
    }

    private static string LifelineText(bool hasCalledFriend)
        => hasCalledFriend ? "Hai Chiamato:   -2" : "Non Hai Chiamato";

    private static string MalusText(EvaluationResult evaluationResult, int malus)
    {
        string result = ScoreViewModel.ResultToScoreText(evaluationResult); 
        return malus == 0 ? result :  string.Format("{0}   -{1}", result , malus);
    }

    private static string DifficultyToScoreText(PhraseDifficulty phraseDifficulty)
        => phraseDifficulty switch
        {
            PhraseDifficulty.Medium => "Agevole   +3",
            PhraseDifficulty.Hard => "Stimulante   +5",
            PhraseDifficulty.Insane => "Pazzo!!!   +7",
            _ => "Facile   +1",
        };

    private static string ResultToScoreText(EvaluationResult evaluationResult)
        => evaluationResult switch
        {
            EvaluationResult.Perfect => "Perfetto!",
            EvaluationResult.Close => "Abbozzato",
            _ => "Fallimento",
        };

    private static string TimeBonusText(TimeSpan timeSpan)
    {
        int seconds = (int)timeSpan.TotalSeconds;
        if (seconds < 15)
        {
            return "Un Fulmine!  +2";
        }
        else if (seconds < 25)
        {
            return "Veloce   +1";
        }
        else if (seconds < 50)
        {
            return "Svelto";
        }
        else if (seconds < 70)
        {
            return "Adagio...";
        }

        return "Così Lento...";
    }

    [RelayCommand]
    public void OnNext() => new ScoringCompleteMessage(this.scoreUpdate).Publish();
}
