namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class OptionsViewModel : ViewModel<OptionsView>
{
    private PhraseDifficulty difficulty;

    [ObservableProperty]
    private bool nextVisible;

    [ObservableProperty]
    private bool visible;

    [ObservableProperty]
    private SelectionGroup? selectionGroup;

    [ObservableProperty]
    private IBrush teamColor;

    public OptionsViewModel()
    {
        this.TeamColor = ColorTheme.LeftForeground;
        this.NextVisible = true; // So that we'll have a property changed 
    }

    public void Update(Team team)
    {
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.SelectionGroup = this.View.SelectionGroup;
        this.SelectionGroup.Clear();
        this.NextVisible = false;
    }

    [RelayCommand]
    public void OnClick(object? parameter)
    {
        if (parameter is string enumAsString)
        {
            if (Enum.TryParse(enumAsString, ignoreCase: true, out PhraseDifficulty difficulty))
            {
                this.difficulty = difficulty;
                Debug.WriteLine(this.difficulty.ToString());
                this.NextVisible = true;
            }
        }
    }

    [RelayCommand]
    public void OnNext()
        => this.Messenger.Publish(new DifficultyChoiceMessage(this.difficulty));
}
