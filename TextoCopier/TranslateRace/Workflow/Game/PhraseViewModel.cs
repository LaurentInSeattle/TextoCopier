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
        this.CallVisible = true;
        this.NextVisible = true;
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
            this.Messenger.Publish(new TranslateCompleteMessage());
            this.NextVisible = false;
        }
        else
        {
            if( this.phrase is null)
            {
                throw new Exception("No phrase!");
            }

            this.English = this.phrase.English;
            this.isRevealed = true;
            this.CallVisible = false; 
        }
    }

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public bool CallVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool NextVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }

    public string Italian { get => this.Get<string>()!; set => this.Set(value); }

    public string English { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand CallCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
