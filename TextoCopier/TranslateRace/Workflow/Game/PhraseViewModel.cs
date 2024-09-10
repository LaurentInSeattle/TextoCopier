namespace Lyt.TranslateRace.Workflow.Game;

public sealed class PhraseViewModel : Bindable<PhraseView>
{
    private Phrase? phrase;
    private bool isRevealed;

    public PhraseViewModel() => this.TeamColor = ColorTheme.LeftForeground;

    public void Update(Team team, Phrase phrase)
    {
        this.phrase = phrase;
        this.isRevealed = false;
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.Italian = phrase.Italian;
        this.English = string.Empty;
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnCall(object? _)
    {
        this.Messenger.Publish(new PlayerLifelineMessage());
    }

    private void OnNext(object? _)
    {
        if (this.isRevealed)
        {
            this.Messenger.Publish(new NextMessage());
        }
        else
        {
            if( this.phrase is null)
            {
                throw new Exception("No phrase!");
            }

            this.English = this.phrase.English;
            this.isRevealed = true;
        }
    }

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public string Italian { get => this.Get<string>()!; set => this.Set(value); }

    public string English { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand CallCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
