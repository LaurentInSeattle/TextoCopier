namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class EvaluationViewModel : ViewModel<EvaluationView>
{
    private EvaluationResult result;

    public EvaluationViewModel()
    {
        this.GlyphSource = GlyphSourceFromPhraseDifficulty(PhraseDifficulty.Easy); 
        this.TeamColor = ColorTheme.LeftForeground;
        this.NextVisible = true; // So that we'll have a property changed 
    }

    public void Update(Team team, PhraseDifficulty phraseDifficulty)
    {
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.GlyphSource = GlyphSourceFromPhraseDifficulty(phraseDifficulty);
        this.SelectionGroup = this.View.SelectionGroup;
        this.SelectionGroup.Clear();
        this.NextVisible = false;
    }

    private static string GlyphSourceFromPhraseDifficulty(PhraseDifficulty phraseDifficulty)
        => phraseDifficulty switch
        {
            PhraseDifficulty.Medium => "emoji_smile_slight",
            PhraseDifficulty.Hard => "emoji_surprise",
            PhraseDifficulty.Insane => "emoji_meh",
            _ => "emoji_laugh",
        };

    [RelayCommand]
    public void OnClick(object? parameter)
    {
        if (parameter is string enumAsString)
        {
            if (Enum.TryParse(enumAsString, ignoreCase: true, out EvaluationResult result))
            {
                this.result= result;
                Debug.WriteLine(this.result.ToString());
                this.NextVisible = true;
            }
        }
    }

    [RelayCommand]
    public void OnNext() => this.Messenger.Publish(new EvaluationResultMessage(this.result));

    [ObservableProperty]
    private string glyphSource ;

    [ObservableProperty]
    private bool nextVisible;

    [ObservableProperty]
    private bool visible;

    [ObservableProperty]
    private SelectionGroup? selectionGroup;

    [ObservableProperty]
    private IBrush teamColor;    
}
