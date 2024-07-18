namespace Lyt.Invasion.Model.GameControl; 

public sealed class Game
{
    public readonly GameOptions GameOptions; 
        
    public readonly List<Player> Players = [];

    public readonly Map Map = new();

    /// <summary> Indicates if the game is over. </summary>
    public bool IsGameOver { get; private set; }

    public Player? Winner { get; private set; }

    public Game (GameOptions gameOptions)
    {
        this.GameOptions = gameOptions;
    }

    public void Start()
    {
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
