

namespace Lyt.Invasion.Model.GameControl;

public sealed class Game
{
    public readonly GameOptions GameOptions;

    public readonly List<Player> Players;

    // Random number generator used during creation of PixelMap
    public readonly Random Random;

    public Game(GameOptions gameOptions, IMessenger messenger, ILogger logger)
    {
        this.Messenger = messenger;
        this.Logger = logger;
        this.GameOptions = gameOptions;
        int playerCount = gameOptions.Players.Count;
        if ((playerCount < 2) || (playerCount > 4) ) 
        {
            throw new ArgumentException("Invalid player count.");
        }

        this.Random = new Random(666);
        // this.Random = new Random(Environment.TickCount);
        this.Map = new Map(this, this.Messenger, this.Logger);
        this.Players = this.CreatePlayers();
        this.AllocateInitialRegions(); 
    }

    public ILogger Logger { get; private set; }

    public IMessenger Messenger { get; private set; }

    public Map Map { get; private set; }

    /// <summary> Indicates if the game is over. </summary>
    public bool IsGameOver { get; private set; }

    public Player? Winner { get; private set; }

    public int PlayerIndex { get; private set; }

    public Phase CurrentPhase { get; private set; }

    public Player CurrentPlayer => this.Players[this.PlayerIndex];

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

    private List<Player> CreatePlayers()
    {
        var list = new List<Player>();
        int index = 0;
        // TODO: Randomize the list 
        foreach (var playerInfo in this.GameOptions.Players)
        {
            list.Add(playerInfo.IsHuman ? new HumanPlayer(index, playerInfo) : new AiPlayer(index, playerInfo));
            ++index; 
        }

        return list;
    }

    private void AllocateInitialRegions()
    {
        List<Coordinate> initialPositions = this.GenerateInitialPositions(this.Players.Count);
        foreach (Player player in this.Players)
        {
            // Initial capture 
            Coordinate initialPosition = initialPositions[player.Index];
            short regionId = this.Map.PixelMap.RegionIdsPerPixel[initialPosition.X, initialPosition.Y];
            Region region = this.Map.Regions[regionId];
            if (region.CanBeOwned)
            {
                region.CaptureBy(player);
            }
            else
            {
                region = this.FindCapturableRegionNear(region);
            } 

            // Build territory around initial region, up to capture provided count 
            int requiredCount = this.GameOptions.InitialTerritory;
            int immediateNeighboursCount = region.NeighbourIds.Count; 
            int minCount = Math.Min(requiredCount, immediateNeighboursCount);
            Region lastNeighbour;
            int captured = 0;
            int i = 0;
            while( (i < minCount) && ( i < region.NeighbourIds.Count))
            {
                short neighbourId = region.NeighbourIds[i];
                Region neighbour = this.Map.Regions[neighbourId];
                if (neighbour.CanBeOwned)
                {
                    neighbour.CaptureBy(player);
                    lastNeighbour = neighbour;
                    ++captured; 
                }

                ++i;
            }

            //int needMore = requiredCount - minCount;
            //while (needMore > 0)
            //{
            //    needMore = requiredCount - minCount;
            //}
        } 
    }

    private Region FindCapturableRegionNear( Region region )
    {
        Region? lastNeighbour = null;
        for (int i = 0; i < region.NeighbourIds.Count; ++i)
        {
            short neighbourId = region.NeighbourIds[i];
            Region neighbour = this.Map.Regions[neighbourId];
            if (neighbour.CanBeOwned)
            {
                return neighbour;
            }

            lastNeighbour = neighbour;
        }

        if (lastNeighbour is null)
        {
            throw new Exception("Cant figure out initial region.");
        } 
        else
        {
            return this.FindCapturableRegionNear(lastNeighbour);
        }
    }

    private List<Coordinate> GenerateInitialPositions(int playerCount)
    {
        List<Coordinate> initialPositions = new (playerCount);
        int width = this.GameOptions.PixelWidth;
        int height = this.GameOptions.PixelHeight;
        if (playerCount == 2) 
        {
            int x1 = width / 4 + this.Random.Next(-30, 10);
            int y1 = height / 2 + this.Random.Next(-50, 50);
            initialPositions.Add(new Coordinate(x1, y1));

            int x2 = 3 * width / 4 + this.Random.Next(-10, 30); ;
            int y2 = height / 2 + this.Random.Next(-50, 50);
            initialPositions.Add(new Coordinate(x2, y2));
        }
        else if (playerCount == 3)
        {
            int x1 = width / 6 + this.Random.Next(-30, 10);
            int y1 = height / 4 + this.Random.Next(-20, 20);
            initialPositions.Add(new Coordinate(x1, y1));

            int x2 = 3 * width / 6 + this.Random.Next(-10, 30); ;
            int y2 = 3 * height / 4 + this.Random.Next(-20, 20);
            initialPositions.Add(new Coordinate(x2, y2));

            int x3 = 5 * width / 6 + this.Random.Next(-10, 30); ;
            int y3 = height / 4 + this.Random.Next(-20, 20);
            initialPositions.Add(new Coordinate(x3, y3));
        }
        else // playerCount == 4
        {
            int x1 = width / 4 + this.Random.Next(-30, 10);
            int y1 = height / 4 + this.Random.Next(-20, 20);
            initialPositions.Add(new Coordinate(x1, y1));

            int x2 = width / 4 + this.Random.Next(-30, 10);
            int y2 = 3 * height / 4 + this.Random.Next(-20, 20);
            initialPositions.Add(new Coordinate(x2, y2));

            int x3 = 3 * width / 4 + this.Random.Next(-10, 30); ;
            int y3 = height / 4 + this.Random.Next(-20, 20);
            initialPositions.Add(new Coordinate(x3, y3));

            int x4 = 3 * width / 4 + this.Random.Next(-10, 30); ;
            int y4 = 3 * height / 4 + this.Random.Next(-20, 20);
            initialPositions.Add(new Coordinate(x4, y4));
        }

        return initialPositions;
    }
}
