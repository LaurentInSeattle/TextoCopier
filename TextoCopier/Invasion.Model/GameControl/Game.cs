namespace Lyt.Invasion.Model.GameControl;

public sealed class Game
{
    public const int MinPlayerCount = 2;
    public const int MaxPlayerCount = 4;

    public readonly GameOptions GameOptions;

    public readonly List<Player> Players;

    /// <summary> Random number generator used during creation of PixelMap and during the game  </summary>
    public readonly IRandomizer Randomizer;

    private Task? gameTask;
    private int recursionDepth;

#pragma warning disable CS8618 
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Map and Players gets created and are normally not null, unless we crash 
    public Game(GameOptions gameOptions, IMessenger messenger, ILogger logger, IRandomizer randomizer)
#pragma warning restore CS8618 
    {
        this.Messenger = messenger;
        this.Logger = logger;
        this.Randomizer = randomizer;

        this.GameOptions = gameOptions;
        this.blockingQueue = new BlockingCollection<GameSynchronizationResponse>(8);

        int playerCount = gameOptions.Players.Count;
        if ((playerCount < MinPlayerCount) || (playerCount > MaxPlayerCount))
        {
            throw new ArgumentException("Invalid player count: " + playerCount);
        }

        int retries = 5;
        bool constructed = false;
        while (!constructed)
        {
            try
            {
                this.Map = new Map(this, this.Messenger, this.Logger, randomizer);
                this.Players = this.CreatePlayers();
                this.recursionDepth = 0;
                this.AllocateInitialRegions();
                constructed = true;
            }
            catch (Exception ex)
            {
                this.Logger.Error("Failed to create map: " + ex);
                retries--;
                if (retries == 0)
                {
                    // Crash :( 
                    throw;
                }
            }
        }
    }

    public ILogger Logger { get; private set; }

    public IMessenger Messenger { get; private set; }

    public Map Map { get; private set; }

    public bool IsGameRunning => this.gameTask is not null;

    /// <summary> Indicates if the game is over. </summary>
    public bool IsGameOver { get; private set; }

    /// <summary> Indicates if the game is has been terminated. </summary>
    public bool IsTerminated { get; private set; }

    /// <summary> Indicates if the game should end abruptly. </summary>
    public bool ShouldAbort { get; private set; }

    public Player? Winner { get; private set; }

    public int PlayerIndex { get; private set; }

    public int Turn { get; private set; }

    public Phase CurrentPhase { get; private set; }

    public Player CurrentPlayer => this.Players[this.PlayerIndex];

    // Cancellation source and token for the game thread.
    private CancellationTokenSource? cancellationTokenSource;
    private CancellationToken cancellationToken;
    private BlockingCollection<GameSynchronizationResponse> blockingQueue;

    public async void Abort()
    {
        this.ShouldAbort = true;
        await Task.Delay(200);
        if (this.IsTerminated)
        {
            return;
        }

        this.OnGameSynchronizationResponse(new GameSynchronizationResponse(MessageKind.Abort));
        this.blockingQueue.CompleteAdding();
        await Task.Delay(200);
        if (this.IsTerminated)
        {
            return;
        }

        this.cancellationTokenSource?.Cancel();
        await Task.Delay(200);
        if (this.IsTerminated)
        {
            return;
        }

        // Uh Oh ...
        throw new Exception("Failed to terminate the game thread");
    }

    public void Start()
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = this.cancellationTokenSource.Token;
        this.blockingQueue = new BlockingCollection<GameSynchronizationResponse>(16);
        this.Messenger.Subscribe<GameSynchronizationResponse>(this.OnGameSynchronizationResponse);

        // Launch the game thread 
        //     Creates and starts a task for the specified action delegate, state, cancellation
        //     token, creation options and the default task scheduler.
        this.gameTask = Task.Factory.StartNew(
            this.GameThread, new object(), this.cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private void OnGameSynchronizationResponse(GameSynchronizationResponse response)
    {
        if (this.IsGameRunning && !this.IsTerminated &&
            this.blockingQueue is not null && !this.blockingQueue.IsAddingCompleted)
        {
            this.blockingQueue.Add(response);
        }
    }

    private async void GameThread(object? _)
    {
        bool aborted = false;
        try
        {
            // Wait a bit so that the UI has time to load the map and the rest of the UI widgets
            await Task.Delay(250);
            await this.GameLoop(this.cancellationToken);
        }
        catch (Exception ex)
        {
            aborted = true;
            this.Logger.Warning(ex.Message);
            Debug.WriteLine(ex);
        }

        this.IsTerminated = true;
        if (this.blockingQueue is not null)
        {
            this.blockingQueue.CompleteAdding();
            this.blockingQueue.Dispose();
        }

        if (this.cancellationTokenSource is not null)
        {
            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null;
        }

        if (aborted)
        {
            this.Notify(MessageKind.Abort);
        }
    }

    private async Task GameLoop(CancellationToken cancellationToken)
    {
        this.Turn = 0;
        this.CurrentPhase = (Phase)0;
        while (!this.IsGameOver)
        {
            ++this.Turn;
            this.PlayerIndex = 0;
            foreach (Player player in this.Players)
            {
                bool abort = await this.CurrentPlayer.Turn(cancellationToken);
                if (abort)
                {
                    throw new Exception("Cancellation Requested");
                }

                ++this.PlayerIndex;
                this.IsGameOver = this.CheckGameOver();
                if (this.IsGameOver)
                {
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new Exception("Cancellation Requested");
                }

                if (this.ShouldAbort)
                {
                    throw new Exception("Cancellation Requested");
                }
            }
        }

        if (this.IsGameOver)
        {
            this.Notify(MessageKind.GameOver);
        }
    }

    public void Notify(MessageKind messageKind) => this.Messenger.Publish(new GameSynchronizationRequest(messageKind));

    public bool Synchronize(
        MessageKind request, out GameSynchronizationResponse? response, CancellationToken cancellationToken)
        => this.Synchronize(new GameSynchronizationRequest(request), out response, cancellationToken);

    public bool Synchronize(
        GameSynchronizationRequest request, out GameSynchronizationResponse? response, CancellationToken cancellationToken)
    {
        // 1 - Publish a request message 
        this.Messenger.Publish(request);

        // 2 - Wait for a response or a cancel request
        response = null;
        // A bunch of tricky exceptions here, that are not really exceptional...
        try
        {
            response = this.blockingQueue.Take(cancellationToken);
            if (response.Message == MessageKind.Abort)
            {
                return false;
            }

            return true;
        }
        catch (OperationCanceledException oce)
        {
            // The CancellationToken is canceled. => Abort the loop, this is a normal situation
            this.Logger.Info("Game Synchronization Queue: First chance exception: " + oce.Message);
            Debug.WriteLine(oce);
            return false;
        }
        catch (ObjectDisposedException ode)
        {
            // The BlockingCollection has been disposed. => Abort the loop, this is a normal situation
            this.Logger.Info("Game Synchronization Queue: First chance exception: " + ode.Message);
            Debug.WriteLine(ode);
            return false;
        }
        catch (InvalidOperationException ioc)
        {
            // The underlying collection was modified outside of this BlockingCollection<T> instance,
            // or the BlockingCollection<T> is empty and has been marked as complete with regards to additions.
            // =>> Swallow and it will eventually break the loop at the next iteration, this is a normal situation. 
            this.Logger.Info("Game Synchronization Queue: First chance exception: " + ioc.Message);
            Debug.WriteLine(ioc);
            return false;
        }
        catch (Exception e)
        {
            this.Logger.Info("Game Synchronization Queue: Unexpected exception: " + e.Message);
            Debug.WriteLine(e);
            throw;
        }
    }

    /// <summary> Checks if the Game is Over:  Either: There is a winner OR  All Humans have lost. </summary>
    private bool CheckGameOver()
    {
        bool hasWinner = false;
        bool allHumansHaveLost = true;
        foreach (Player player in this.Players)
        {
            hasWinner = player.Status == Player.StatusKind.Won;
            if (hasWinner)
            {
                this.Winner = player;
            }

            if (player.Status != Player.StatusKind.Lost)
            {
                allHumansHaveLost = false;
            }
        }

        return hasWinner || allHumansHaveLost;
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
        // Randomize the list of player info
        this.Randomizer.Shuffle<PlayerInfo>(this.GameOptions.Players);
        var list = new List<Player>();
        int index = 0;
        foreach (var playerInfo in this.GameOptions.Players)
        {
            list.Add(
                playerInfo.IsHuman ?
                    new HumanPlayer(index, playerInfo, this) :
                    new AiPlayer(index, playerInfo, this));
            ++index;
        }

        return list;
    }

    private void AllocateInitialRegions()
    {
        int addedTerritories = Game.MaxPlayerCount - this.Players.Count;
        List<Coordinate> initialPositions = this.GenerateInitialPositions(this.Players.Count);
        foreach (Player player in this.Players)
        {
            if (player.Color == "Red")
            {
                // if ( Debugger.IsAttached ) {  Debugger.Break(); }   
            }

            // Initial capture becomes the player capital 
            Coordinate initialPosition = initialPositions[player.Index];
            short regionId = this.Map.PixelMap.RegionIdsPerPixel[initialPosition.X, initialPosition.Y];
            Region region = this.Map.Regions[regionId];
            if (region.IsOwned || !region.CanBeOwned)
            {
                region = this.FindCapturableRegionNear(region);
            }

            region.CaptureBy(player);
            player.Capital = region;
            region.IsCapital = true;

            // Build territory around initial region, up to capture provided count 
            int requiredCount = this.GameOptions.InitialTerritory + addedTerritories - 1;
            int captured = 0;
            int maxLoops = 0;
            Region? lastNeighbour = null;
            while ((captured < requiredCount) && (maxLoops < 10))
            {
                int immediateNeighboursCount = region.NeighbourIds.Count;
                for (int k = 0; k < immediateNeighboursCount; ++k)
                {
                    short neighbourId = region.NeighbourIds[k];
                    Region neighbour = this.Map.Regions[neighbourId];
                    if (!neighbour.IsOwned && neighbour.CanBeOwned)
                    {
                        neighbour.CaptureBy(player);
                        lastNeighbour = neighbour;
                        ++captured;
                    }

                    if (captured >= requiredCount)
                    {
                        break;
                    }
                }

                if (captured < requiredCount)
                {
                    if (lastNeighbour is not null)
                    {
                        region = this.FindCapturableRegionNear(lastNeighbour);
                        region.CaptureBy(player);
                        ++captured;
                    }
                }

                ++maxLoops;
            }
        }
    }

    private Region FindCapturableRegionNear(Region region)
    {
        ++this.recursionDepth;
        Region? lastNeighbour = null;
        for (int i = 0; i < region.NeighbourIds.Count; ++i)
        {
            short neighbourId = region.NeighbourIds[i];
            Region neighbour = this.Map.Regions[neighbourId];
            if (!neighbour.IsOwned && neighbour.CanBeOwned)
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
            if (this.recursionDepth > 250)
            {
                throw new Exception("Cant figure out initial region: possible cycle");
            }

            return this.FindCapturableRegionNear(lastNeighbour);
        }
    }

    private List<Coordinate> GenerateInitialPositions(int playerCount)
    {
        List<Coordinate> initialPositions = new(playerCount);
        int width = this.GameOptions.PixelWidth;
        int height = this.GameOptions.PixelHeight;
        if (playerCount == 2)
        {
            int x1 = width / 4 + this.Randomizer.Next(-30, 10);
            int y1 = height / 2 + this.Randomizer.Next(-50, 50) - height / 6;
            initialPositions.Add(new Coordinate(x1, y1));

            int x2 = 3 * width / 4 + this.Randomizer.Next(-10, 30); ;
            int y2 = height / 2 + this.Randomizer.Next(-50, 50) + height / 6;
            initialPositions.Add(new Coordinate(x2, y2));
        }
        else if (playerCount == 3)
        {
            int x1 = width / 6 + this.Randomizer.Next(-30, 10);
            int y1 = height / 4 + this.Randomizer.Next(-20, 20);
            initialPositions.Add(new Coordinate(x1, y1));

            int x2 = 3 * width / 6 + this.Randomizer.Next(-10, 30); ;
            int y2 = 3 * height / 4 + this.Randomizer.Next(-20, 20);
            initialPositions.Add(new Coordinate(x2, y2));

            int x3 = 5 * width / 6 + this.Randomizer.Next(-10, 30); ;
            int y3 = height / 4 + this.Randomizer.Next(-20, 20);
            initialPositions.Add(new Coordinate(x3, y3));
        }
        else // playerCount == 4
        {
            int x1 = width / 4 + this.Randomizer.Next(-30, 10);
            int y1 = height / 4 + this.Randomizer.Next(-20, 20);
            initialPositions.Add(new Coordinate(x1, y1));

            int x2 = width / 4 + this.Randomizer.Next(-30, 10);
            int y2 = 3 * height / 4 + this.Randomizer.Next(-20, 20);
            initialPositions.Add(new Coordinate(x2, y2));

            int x3 = 3 * width / 4 + this.Randomizer.Next(-10, 30); ;
            int y3 = height / 4 + this.Randomizer.Next(-20, 20);
            initialPositions.Add(new Coordinate(x3, y3));

            int x4 = 3 * width / 4 + this.Randomizer.Next(-10, 30); ;
            int y4 = 3 * height / 4 + this.Randomizer.Next(-20, 20);
            initialPositions.Add(new Coordinate(x4, y4));
        }

        return initialPositions;
    }
}
