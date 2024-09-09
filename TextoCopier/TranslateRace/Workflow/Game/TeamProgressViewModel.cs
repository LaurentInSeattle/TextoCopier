namespace Lyt.TranslateRace.Workflow.Game;

public sealed class TeamProgressViewModel : Bindable<TeamProgressView>
{
    public TeamProgressViewModel(string teamName, float total, IBrush background, IBrush foreground)
    {
        this.TeamName = teamName;
        this.Total = total;
        this.Background = background;
        this.Foreground = foreground;
    }

    public void Update(int score)
    {
        this.ScoreValue = (float) score;
        this.ScoreTextLeft = score.ToString();
        this.ScoreTextRight = (this.Total - score).ToString();
    }

    #region Bound properties 

    public string TeamName { get => this.Get<string>()!; set => this.Set(value); }

    public float Total { get => this.Get<float>(); set => this.Set(value); }

    public string ScoreTextLeft { get => this.Get<string>()!; set => this.Set(value); }

    public string ScoreTextRight { get => this.Get<string>()!; set => this.Set(value); }

    public float ScoreValue { get => this.Get<float>(); set => this.Set(value); }

    public IBrush Background { get => this.Get<IBrush>()!; set => this.Set(value); }

    public IBrush Foreground { get => this.Get<IBrush>()!; set => this.Set(value); }

    #endregion  Bound properties 
}
