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
    public List<PlayerInfo> Players { get; set; } = [];

    public MapSize MapSize { get; set; }

    public Ecosystem DominantEcosystem { get; set; }

    public GameDifficulty Difficulty { get; set; }

    public int PixelWidth =>    
        this.MapSize switch
        {
            MapSize.Tiny => 600,
            MapSize.Small => 800,
            MapSize.Medium => 1000,
            MapSize.Large => 1400,
            _ => 1800, // Huge
        };

    public int PixelHeight => this.PixelWidth * 9 / 16;

    public int RegionCount =>
        this.MapSize switch
        {
            MapSize.Tiny => 80,
            MapSize.Small => 160,
            MapSize.Medium => 240,
            MapSize.Large => 360,
            _ => 420, // Huge
        };
}
