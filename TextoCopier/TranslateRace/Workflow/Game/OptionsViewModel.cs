namespace Lyt.TranslateRace.Workflow.Game;

public sealed class OptionsViewModel : Bindable<OptionsView>
{
    private PhraseDifficulty difficulty;

    public OptionsViewModel() => this.TeamColor = ColorTheme.LeftForeground;

    public void Update(Team team)
    {
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.SelectionGroup = this.View.SelectionGroup;
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnClick(object? parameter)
    {
        if (parameter is string enumAsString)
        {
            if (Enum.TryParse(enumAsString, ignoreCase: true, out PhraseDifficulty difficulty))
            {
                this.difficulty = difficulty;
                Debug.WriteLine(this.difficulty.ToString());
            }
        }
    }

    private void OnNext(object? _)
    {
        this.Messenger.Publish(new DifficultyChoiceMessage(this.difficulty));
    } 

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand ClickCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public SelectionGroup SelectionGroup { get => this.Get<SelectionGroup>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
