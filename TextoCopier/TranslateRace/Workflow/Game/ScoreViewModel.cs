namespace Lyt.TranslateRace.Workflow.Game;

public sealed class ScoreViewModel : Bindable<ScoreView>
{
    private const int delay = 2_500;
    private const int shortenedDelay = delay - 100;

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
        "Maledetta!",
        "Errore" , "Che Sbaglio" , "Mancanza", "Fallo", "Pecca",
        "Scemo" ,  "Ottuso...",
        "Terribile"
    ];

    private readonly IRandomizer randomizer;
    private readonly Chooser<string> beNice;
    private readonly Chooser<string> beMean;

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
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.Visible = true;
        if (evaluationResult == EvaluationResult.Fail)
        {
            this.PopMessage(this.beMean.Next(), ColorTheme.UiText);
        }
        else
        {
            this.PopMessage(this.beNice.Next(), ColorTheme.BoxExact);
        }

        int teamScore = team.Score;
        int score = this.DifficultyToScore(phraseDifficulty);
        int malus = this.Malus(phraseDifficulty, evaluationResult);
        int lifeline = hasCalledFriend ? -2 : 0;

        // TODO
        int timeBonus = 2;

        Debugger.Break(); 

        // Baseline 
        Schedule.OnUiThread(
            delay,
            () =>
            {
                string message = this.DifficultyToScoreText(phraseDifficulty);
                this.PopMessage(message, ColorTheme.Text);
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score));
            }, DispatcherPriority.Background);

        // Malus 
        Schedule.OnUiThread(
            2 * delay,
            () =>
            {
                string message = this.MalusText(evaluationResult, malus);
                this.PopMessage(message, ColorTheme.Text);
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score - malus));
            }, DispatcherPriority.Background);

        // Called a Friend 
        Schedule.OnUiThread(
            3 * delay,
            () =>
            {
                string message = this.LifelineText(hasCalledFriend);
                this.PopMessage(message, ColorTheme.Text);
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score - malus - lifeline));
            }, DispatcherPriority.Background);

        // Time Bonus
        Schedule.OnUiThread(
            4 * delay,
            () =>
            {
                // TODO 
            }, DispatcherPriority.Background);

        // Final 
        Schedule.OnUiThread(
            5 * delay,
            () =>
            {
                int scoreUpdate = score - malus - lifeline + timeBonus;
                if (scoreUpdate < 0)
                {
                    scoreUpdate = 0;
                }

                // TODO 

                team.Score = teamScore + scoreUpdate;
                this.NextVisible = true;
            }, DispatcherPriority.Background);
    }

    private void PopMessage(string text, Brush brush)
    {
        this.Comment = text;
        this.CommentColor = brush;
        Schedule.OnUiThread(
            shortenedDelay,
            () =>
            {
                this.Comment = string.Empty;
            }, DispatcherPriority.Background);
    }

    private int Malus(PhraseDifficulty phraseDifficulty, EvaluationResult evaluationResult)
    {
        if (evaluationResult == EvaluationResult.Fail)
        {
            return this.DifficultyToScore(phraseDifficulty);
        }

        return evaluationResult == EvaluationResult.Perfect ? 0 : 2;
    }

    private string LifelineText(bool hasCalledFriend)
        => hasCalledFriend ? "Hai Chiamato:   -2" : "Non Hai Chiamato";

    private string MalusText(EvaluationResult evaluationResult, int malus)
        => string.Format("{0}   {1}", this.ResultToScoreText(evaluationResult), malus);

    private int DifficultyToScore(PhraseDifficulty phraseDifficulty)
        => phraseDifficulty switch
        {
            PhraseDifficulty.Medium => 3,
            PhraseDifficulty.Hard => 5,
            PhraseDifficulty.Insane => 7,
            _ => 1,
        };

    private string DifficultyToScoreText(PhraseDifficulty phraseDifficulty)
        => phraseDifficulty switch
        {
            PhraseDifficulty.Medium => "Agevole   +3",
            PhraseDifficulty.Hard => "Stimulante   +5",
            PhraseDifficulty.Insane => "Pazzo!!!   +7",
            _ => "Facile   +1",
        };

    private string ResultToScoreText(EvaluationResult evaluationResult)
        => evaluationResult switch
        {
            EvaluationResult.Perfect => "Perfetto",
            EvaluationResult.Close => "Abbozzato",
            _ => "Fallimento",
        };

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnNext(object? _) => this.Messenger.Publish(new ScoringCompleteMessage());

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public bool NextVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }

    public string Comment { get => this.Get<string>()!; set => this.Set(value); }

    public Brush CommentColor { get => this.Get<Brush>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
/*
10s come un fulmine  +2
20s abbastanza veloce +1
30s svelto
45s vivace
1:00 lento 
1:30 così lento -1

 */