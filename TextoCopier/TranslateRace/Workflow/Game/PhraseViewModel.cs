namespace Lyt.TranslateRace.Workflow.Game;

public sealed class PhraseViewModel : Bindable<PhraseView>
{
    private Team? team;
    private Phrase? phrase;
    private bool isRevealed;

    public PhraseViewModel()
    {
        this.TeamColor = ColorTheme.Text;
        this.Visible = true;
        this.Messenger.Subscribe<PlayerDropMessage>(this.OnPlayerDrop);
    }

    public void Update(Team team, Phrase phrase)
    {
        this.team = team;
        this.phrase = phrase;
        this.isRevealed = false;
        this.TeamColor = ColorTheme.Text;
        this.Italian = phrase.Italian;
        this.English = string.Empty;
        this.CallVisible = team.Players.Count > 1;
        this.NextVisible = true;
    }

    private void OnPlayerDrop(PlayerDropMessage message)
    {
        if (this.team is null)
        {
            return;
        }

        if (this.CallVisible && (this.team.Players.Count < 2))
        {
            this.CallVisible = false;
        } 
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnCall(object? _)
    {
        // Can call only once 
        this.CallVisible = false;
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
            if (this.phrase is null)
            {
                throw new Exception("No phrase!");
            }

            this.Messenger.Publish(new TranslateRevealedMessage());
            this.English = this.phrase.English;
            this.isRevealed = true;
            this.CallVisible = false;
        }
    }

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }

    public bool CallVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool NextVisible { get => this.Get<bool>(); set => this.Set(value); }

    public string Italian { get => this.Get<string>()!; set => this.Set(value); }

    public string English { get => this.Get<string>()!; set => this.Set(value); }

    public ICommand CallCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }
}
