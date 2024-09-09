namespace Lyt.TranslateRace.Workflow.Game; 

public sealed class OptionsViewModel : Bindable<OptionsView>
{
    private Player? player;

    public OptionsViewModel() => this.TeamColor = ColorTheme.LeftForeground;

    public void Update(Team team, Player player)
    {
        this.player = player;
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.SelectionGroup = this.View.SelectionGroup;
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnClick(object? _) { /* TODO */ }

    private void OnNext(object? _) { /* TODO */ }

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public ICommand ClickCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public SelectionGroup SelectionGroup { get => this.Get<SelectionGroup>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
