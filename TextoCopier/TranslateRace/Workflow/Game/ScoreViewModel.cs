﻿namespace Lyt.TranslateRace.Workflow.Game;

public sealed class ScoreViewModel : Bindable<ScoreView>
{
    private const int delay = 2_700;
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
        this.NextVisible = false; // So that we'll have a property changed 
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
        int score = ScoreViewModel.DifficultyToScore(phraseDifficulty);
        int malus = ScoreViewModel.Malus(phraseDifficulty, evaluationResult);
        int lifeline = ScoreViewModel.Lifeline(hasCalledFriend);
        int timeBonus = ScoreViewModel.TimeBonus(translateTime);
        int scoreUpdate = score - malus - lifeline + timeBonus;
        if (scoreUpdate < 0)
        {
            scoreUpdate = 0;
        }

        // Baseline 
        Schedule.OnUiThread(
            delay,
            () =>
            {
                string message = ScoreViewModel.DifficultyToScoreText(phraseDifficulty);
                this.PopMessage(message, ColorTheme.Text);
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score));
            }, DispatcherPriority.Background);

        // Malus 
        Schedule.OnUiThread(
            2 * delay,
            () =>
            {
                string message = ScoreViewModel.MalusText(evaluationResult, malus);
                this.PopMessage(message, ColorTheme.Text);
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score - malus));
            }, DispatcherPriority.Background);

        // Called a Friend 
        Schedule.OnUiThread(
            3 * delay,
            () =>
            {
                string message = ScoreViewModel.LifelineText(hasCalledFriend);
                this.PopMessage(message, ColorTheme.Text);
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score - malus - lifeline));
            }, DispatcherPriority.Background);

        // Time Bonus
        Schedule.OnUiThread(
            4 * delay,
            () =>
            {
                string message = ScoreViewModel.TimeBonusText(translateTime);
                this.PopMessage(message, ColorTheme.Text);
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score - malus - lifeline + timeBonus));
            }, DispatcherPriority.Background);

        // Final 
        Schedule.OnUiThread(
            5 * delay,
            () =>
            {
                string message = "Punteggio non è cambiato.";
                if (scoreUpdate > 0)
                {
                    message = string.Format("Punteggio aumentato di: {0}" , scoreUpdate);
                }
                this.PopMessage(message, ColorTheme.Text, hide:false);

                team.Score = teamScore + scoreUpdate;
                this.NextVisible = true;
                this.Messenger.Publish(new ScoreUpdateMessage(teamScore + score - malus - lifeline + timeBonus));
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

    private static int Lifeline(bool hasCalledFriend) => hasCalledFriend ? -2 : 0;

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