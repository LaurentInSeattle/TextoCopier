namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class TurnViewModel : ViewModel<TurnView>
{
    private readonly Player player;
    private readonly Team team;

    [ObservableProperty]
    private string teamName;

    [ObservableProperty]
    private string playerName;

    [ObservableProperty]
    private IBrush teamColor;

    [ObservableProperty]
    private string nextTeamName;

    [ObservableProperty]
    private string nextPlayerName;

    [ObservableProperty]
    private IBrush nextTeamColor;

    [ObservableProperty]
    private bool hasDroppedVisible;

    public TurnViewModel(Team team, Player player, Team nextTeam, Player nextPlayer)
    {
        this.team = team;
        this.player = player;
        this.HasDroppedVisible = true;
        this.team = team;
        this.player = player;
        this.TeamName = team.Name;
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.PlayerName = player.Participant.Name;
        this.NextTeamName = nextTeam.Name;
        this.NextPlayerName = nextPlayer.Participant.Name;
        this.NextTeamColor = nextTeam.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        if (this.team.Players.Count < 2)
        {
            this.HasDroppedVisible = false;
        }
    }

    [RelayCommand]
    public void OnHasDropped()
    {
        if (this.team.Players.Count < 2)
        {
            return;
        }

        new PlayerDropMessage(this.player).Publish<PlayerDropMessage>();
    }
}
