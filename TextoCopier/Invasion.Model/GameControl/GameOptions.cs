namespace Lyt.Invasion.Model.GameControl; 

public enum MapSize
{
    Debug, 
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
    Debug,
}

public sealed class GameOptions
{
    public List<PlayerInfo> Players { get; set; } = [];

    public MapSize MapSize { get; set; }

    public Ecosystem DominantEcosystem { get; set; }

    public GameDifficulty Difficulty { get; set; }

    public int InitialSkills // Out of 10 currently 
        => this.Difficulty switch
        {
            GameDifficulty.Easy => 10,
            GameDifficulty.Fair => 9,
            GameDifficulty.Challenging => 8,
            GameDifficulty.Hard => 7,
            GameDifficulty.Insane => 6,
            _ => 6, // Easy 
        };

    public int InitialTerritory
        => this.Difficulty switch
        {
            GameDifficulty.Easy => 6,
            GameDifficulty.Fair => 6,
            GameDifficulty.Challenging => 5,
            GameDifficulty.Hard => 5,
            GameDifficulty.Insane => 5,
            _ => 6, // Easy 
        };

    public int PixelWidth =>    
        this.MapSize switch
        {
            MapSize.Debug => 512,
            MapSize.Tiny => 600,
            MapSize.Small => 800,
            MapSize.Medium => 1000,
            MapSize.Large => 1400,
            MapSize.Huge => 1800,
            _ => 1800, // Huge
        };

    public int PixelHeight => this.PixelWidth * 9 / 16;

    public int RegionCount =>
        this.MapSize switch
        {
            MapSize.Debug => 50,
            MapSize.Tiny => 90,
            MapSize.Small => 130,
            MapSize.Medium => 170,
            MapSize.Large => 210,
            MapSize.Huge => 260,
            _ => 260, // Huge
        };
}
