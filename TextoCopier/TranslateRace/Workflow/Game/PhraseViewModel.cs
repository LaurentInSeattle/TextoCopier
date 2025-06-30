namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class PhraseViewModel : ViewModel<PhraseView>
{
    [ObservableProperty]
    private bool visible;

    [ObservableProperty]
    private bool callVisible;

    [ObservableProperty]
    private bool nextVisible;

    [ObservableProperty]
    private string? italian;

    [ObservableProperty]
    private string? english;

    [ObservableProperty]
    private string? translated;

    [ObservableProperty]
    private IBrush teamColor;

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
        this.Translated = string.Empty;
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

    [RelayCommand]
    public void OnCall()
    {
        // Can call only once 
        this.CallVisible = false;
        this.Messenger.Publish(new PlayerLifelineMessage());
    }

    [RelayCommand]
    public void OnNext()
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
            this.Translated = this.phrase.Translated;
            this.isRevealed = true;
            this.CallVisible = false;
        }
    }
}
