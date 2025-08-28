namespace Lyt.TranslateRace.Workflow.Game;

using CommunityToolkit.Mvvm.Messaging;

public sealed partial class PhraseViewModel : ViewModel<PhraseView>, IRecipient<PlayerDropMessage>
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
        this.Subscribe<PlayerDropMessage>();
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

    public void Receive(PlayerDropMessage _)
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
        new PlayerLifelineMessage().Publish();
    }

    [RelayCommand]
    public void OnNext()
    {
        if (this.isRevealed)
        {
            new TranslateCompleteMessage().Publish();
            this.NextVisible = false;
        }
        else
        {
            if (this.phrase is null)
            {
                throw new Exception("No phrase!");
            }

            new TranslateRevealedMessage().Publish();
            this.English = this.phrase.English;
            this.Translated = this.phrase.Translated;
            this.isRevealed = true;
            this.CallVisible = false;
        }
    }
}
