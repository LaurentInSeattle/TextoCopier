namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class TeamProgressViewModel(
    string teamName, float total, IBrush background, IBrush foreground) 
    : ViewModel<TeamProgressView>
{
    [ObservableProperty]
    private string teamName = teamName;

    [ObservableProperty]
    private float total = total;

    [ObservableProperty]
    private string? scoreTextLeft;

    [ObservableProperty]
    private string? scoreTextRight;

    [ObservableProperty]
    private float scoreValue;

    [ObservableProperty]
    private IBrush background = background;

    [ObservableProperty]
    private IBrush foreground = foreground;

    public void Update(int score)
    {
        this.ScoreValue = (float)score;
        this.ScoreTextLeft = score.ToString();
        int remain = (int) this.Total - score; 
        this.ScoreTextRight = remain < 0 ? string.Empty : remain.ToString();
    }
}
