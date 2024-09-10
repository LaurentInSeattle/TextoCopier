namespace Lyt.TranslateRace.Workflow.Game;

public sealed class EvaluationViewModel : Bindable<EvaluationView>
{
    private EvaluationResult result;

    public EvaluationViewModel()
    {
        this.TeamColor = ColorTheme.LeftForeground;
        this.NextVisible = true; // So that we'll have a property changed 
    }

    public void Update(Team team, PhraseDifficulty phraseDifficulty)
    {
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.GlyphSource = this.GlyphSourceFromPhraseDifficulty(phraseDifficulty);
        this.SelectionGroup = this.View.SelectionGroup;
        this.SelectionGroup.Clear();
        this.NextVisible = false;
    }

    private string GlyphSourceFromPhraseDifficulty(PhraseDifficulty phraseDifficulty)
        => phraseDifficulty switch
        {
            PhraseDifficulty.Medium => "emoji_smile_slight",
            PhraseDifficulty.Hard => "emoji_surprise",
            PhraseDifficulty.Insane => "emoji_meh",
            _ => "emoji_laugh",
        };

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnClick(object? parameter)
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

    private void OnNext(object? _) => this.Messenger.Publish(new EvaluationResultMessage(this.result));

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public string GlyphSource { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand ClickCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public bool NextVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }

    public SelectionGroup SelectionGroup { get => this.Get<SelectionGroup>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
