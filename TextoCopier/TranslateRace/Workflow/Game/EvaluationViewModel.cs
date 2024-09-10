namespace Lyt.TranslateRace.Workflow.Game;

public sealed class EvaluationViewModel : Bindable<EvaluationView>
{
    private EvaluationResult result;

    public EvaluationViewModel() => this.TeamColor = ColorTheme.LeftForeground;

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
            if (Enum.TryParse(enumAsString, ignoreCase: true, out EvaluationResult result))
            {
                this.result= result;
                Debug.WriteLine(this.result.ToString());
            }
        }
    }

    private void OnNext(object? _) => this.Messenger.Publish(new EvaluationResultMessage(this.result));

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public ICommand ClickCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }

    public SelectionGroup SelectionGroup { get => this.Get<SelectionGroup>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
