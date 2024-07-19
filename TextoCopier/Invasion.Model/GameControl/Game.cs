namespace Lyt.Invasion.Model.GameControl;

public sealed class Game
{
    public readonly GameOptions GameOptions;

    public readonly List<Player> Players;

    public readonly Map Map;

    /// <summary> Indicates if the game is over. </summary>
    public bool IsGameOver { get; private set; }

    public Player? Winner { get; private set; }

    public int PlayerIndex { get; private set; }

    public Phase CurrentPhase { get; private set; }

    public Player CurrentPlayer => this.Players[this.PlayerIndex];

    public Game(GameOptions gameOptions)
    {
        this.GameOptions = gameOptions;
        this.Map = new Map(gameOptions);
        this.Players = new(8);
    }

    public void Start()
    {
        this.PlayerIndex = 0;
        this.CurrentPhase = (Phase)0;
    }

    public void Next()
    {
    }

    public void AiPlayerPhase()
    {
        switch (this.CurrentPhase)
        {
            case Phase.Collect:
                this.CurrentPlayer.DoCollect();
                break;
            case Phase.Deploy:
                this.CurrentPlayer.DoDeploy();
                break;
            case Phase.Destroy:
                this.CurrentPlayer.DoDestroy();
                break;
            case Phase.Build:
                this.CurrentPlayer.DoBuild();
                break;
            case Phase.Attack:
                this.CurrentPlayer.DoAttack();
                break;
            case Phase.Colonize:
                this.CurrentPlayer.DoColonize();
                break;
            case Phase.Move:
                this.CurrentPlayer.DoMove();
                break;
            default:
                break;
        }

        this.NextPhase();
    }

    private void NextPhase()
    {
        if (this.CurrentPhase == Phase.Move)
        {
            ++this.PlayerIndex;
            if (this.PlayerIndex == this.Players.Count)
            {
                this.PlayerIndex = 0;
                this.CurrentPhase = Phase.Collect;
            }
        }
        else
        {
            this.CurrentPhase = (Phase)(1 + (int)this.CurrentPhase);
        }
    }

    public void Destroy()
    {
        this.Map.Destroy();
        foreach (var player in this.Players)
        {
            player.Destroy();
        }
    }
}
