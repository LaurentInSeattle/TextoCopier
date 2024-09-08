namespace Lyt.TranslateRace.Workflow.Game;

public sealed class TurnViewModel : Bindable<TurnView>
{
    private Player player; 

    public TurnViewModel(Team team, Player player, Team nextTeam, Player nextPlayer)
    {
        this.player = player;
        this.Update(team, player, nextTeam, nextPlayer);
    }

    public void Update(Team team, Player player, Team nextTeam, Player nextPlayer)
    {
        this.player = player;
        this.TeamName = team.Name;
        this.TeamColor = team.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
        this.PlayerName = player.Participant.Name;
        this.NextTeamName = nextTeam.Name;
        this.NextPlayerName = nextPlayer.Participant.Name;
        this.NextTeamColor = nextTeam.IsLeft ? ColorTheme.LeftForeground : ColorTheme.RightForeground;
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnHasDropped(object? _) => this.Messenger.Publish<PlayerDropMessage>(new PlayerDropMessage(this.player));

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members


    #region Bound properties 

    public string TeamName { get => this.Get<string>()!; set => this.Set(value); }

    public string PlayerName { get => this.Get<string>()!; set => this.Set(value); }

    public IBrush TeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }

    public string NextTeamName { get => this.Get<string>()!; set => this.Set(value); }

    public string NextPlayerName { get => this.Get<string>()!; set => this.Set(value); }

    public IBrush NextTeamColor { get => this.Get<IBrush>()!; set => this.Set(value); }

    public ICommand HasDroppedCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    #endregion  Bound properties 
}
