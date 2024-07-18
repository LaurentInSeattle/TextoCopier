namespace Lyt.Invasion.Model.GameControl; 

public enum MapSize
{
    Tiny , 
    Small , 
    Medium, 
    Large, 
    Huge,
}

public enum GameDifficulty
{
    Easy,
    Fair,
    Challenging,
    Hard,
    Insane,
}

public sealed class GameOptions
{
    public List<HumanPlayer> HumanPlayers { get; set; } = [];

    public List<AiPlayer> AiPlayers { get; set; } = [];

    public MapSize MapSize { get; set; }

    public Ecosystem DominantEcosystem { get; set; }

    public GameDifficulty Difficulty { get; set; }
}
