namespace Lyt.Invasion.Workflow.Gameplay;

using static ViewActivationMessage;

public sealed partial class GameViewModel : ViewModel<GameView>
{
    // Border paths color components
    private const byte red = 0x10;
    private const byte blu = 0x20;
    private const byte gre = 0x10;

    private readonly InvasionModel invasionModel;
    private readonly List<SolidColorBrush> playerBrushes;
    private readonly Dictionary<Region, List<Polyline>> regionsBorders;

    [ObservableProperty]
    private double zoomFactor;

    private Image? mapImage;
    private Region? hoveredRegion;
    private GameOptions gameOptions;

#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Some non-nullable fields and properties get assigned when the view model is activated 
    public GameViewModel(InvasionModel invasionModel)
#pragma warning restore CS8618 
    {
        this.invasionModel = invasionModel;
        this.playerBrushes = new (4);
        this.regionsBorders = new(512); 
        this.Messenger.Subscribe<ZoomRequestMessage>(this.OnZoomRequest); 
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Create and bind child views 
        var regionVm = App.GetRequiredService<RegionViewModel>();
        regionVm.Bind(this.View.RegionView);
        var playerVm = App.GetRequiredService<PlayerViewModel>();
        playerVm.Bind(this.View.PlayerView);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not GameOptions gameOptions)
        {
            throw new ArgumentNullException(nameof(activationParameters));
        }

        this.gameOptions = gameOptions;

        if (this.mapImage is not null)
        {
            this.mapImage.PointerMoved -= this.OnPointerMoved;
            this.mapImage.PointerPressed -= this.OnPointerPressed;
            this.mapImage.Source = null;
            this.mapImage = null;
        }

        // Resize and clear the canvas (needed when starting a second game)
        var canvas = this.View.Canvas;
        canvas.Children.Clear();
        canvas.Width = this.gameOptions.PixelWidth;
        canvas.Height = this.gameOptions.PixelHeight;
        canvas.InvalidateVisual();

        this.Messenger.Subscribe<GameSynchronizationRequest>(this.OnGameSynchronizationRequest, withUiDispatch: true);
        Dispatch.OnUiThread(() => { this.UpdateUi(); });
    }

    public override void Deactivate()
    {
    }

    private void OnZoomRequest(ZoomRequestMessage message)
        => this.ZoomFactor = message.ZoomFactor; 

    private void OnGameSynchronizationRequest(GameSynchronizationRequest request)
    {
        if (request.Message == MessageKind.GameOver)
        {
            this.Messenger.Publish(ActivatedView.GameOver);
        }
        else if (request.Message == MessageKind.Abort)
        {
            this.Messenger.Publish(ActivatedView.GameOver);
        }
        else if (request.Message == MessageKind.Test)
        {
            // Not used for now
        }
    }

    [RelayCommand]
    public void OnExit() => this.Messenger.Publish(ActivatedView.Exit);

    private void UpdateUi()
    {
        // CONSIDER: Create the game model in a background thread 
        this.invasionModel.NewGame(this.gameOptions);
        if (this.invasionModel.Game is null)
        {
            throw new Exception("Failed to create a new game");
        }

        this.GeneratePlayerBrushes();
        this.GenerateMapImage();
        this.GeneratePaths();
        this.GenerateCenters();

        this.Logger.Info("Ui generated");
        this.Profiler.MemorySnapshot("Ui generated");

        this.invasionModel.Game.Start();
    }

    private void GenerateCenters()
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        int count = 0;
        var canvas = this.View.Canvas;
        var map = game.Map;
        foreach (var region in map.Regions)
        {
            if ((region.Ecosystem == Ecosystem.Ocean) || (region.Ecosystem == Ecosystem.Mountain))
            {
                continue;
            }

            var fillBrush = region.IsCapital ? Brushes.Wheat : Brushes.Black;
            fillBrush = region.IsOwned ? fillBrush : Brushes.Gray;
            IBrush strokeBrush = region.IsOwned ? this.playerBrushes[region.Owner!.Index] : Brushes.Gray;
            int size = region.IsOwned ? 14 : 6;
            size = region.IsCapital ? 16 : size;
            var center = region.AltCenter;
            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Stroke = strokeBrush,
                Fill = fillBrush,
                StrokeThickness = 2.0,
            };
            canvas.Children.Add(ellipse);
            ellipse.SetValue(Canvas.TopProperty, center.Y);
            ellipse.SetValue(Canvas.LeftProperty, center.X);

            ++count;
            if (count > 1_000)
            {
                break;
            }
        }
    }

    private void GeneratePaths()
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        this.regionsBorders.Clear();
        var canvas = this.View.Canvas;
        var map = game.Map;
        // new SolidColorBrush(Color.FromRgb(red, blu, gre));
        var unclaimedStrokeBrush = new SolidColorBrush(Color.FromArgb(140,0,0,0));
        foreach (var region in map.Regions)
        {
            List<Polyline> paths = new (16);
            this.regionsBorders.Add(region, paths);
            SolidColorBrush strokeBrush;
            int zindex;
            if (!region.CanBeOwned)
            {
                // No borders for regions that cannot be conquered
                // Use the ecosystem color to avoid pixel debris
                var color = GameViewModel.EcosystemToColor(region.Ecosystem);
                strokeBrush = new SolidColorBrush(color);
                zindex = 0;
            }
            else
            {
                if (region.IsOwned)
                {
                    strokeBrush = this.playerBrushes[region.Owner!.Index];
                    zindex = 200;
                }
                else
                {
                    strokeBrush = unclaimedStrokeBrush;
                    zindex = 100;
                }
            }

            foreach (var path in region.SimplifiedPaths)
            {
                var points = (from v in path select new Point(v.X, v.Y)).ToList();
                var polyline = new Polyline
                {
                    Stroke = strokeBrush,
                    Fill = Brushes.Transparent,
                    Points = points,
                    StrokeThickness = 3.0,
                    StrokeJoin = PenLineJoin.Round,
                    StrokeLineCap = PenLineCap.Round,
                    ZIndex = zindex,
                };

                polyline.SetValue(Canvas.TopProperty, 0.0);
                polyline.SetValue(Canvas.LeftProperty, 0.0);
                canvas.Children.Add(polyline);
                paths.Add(polyline);
            }
        }
    }

    private void GenerateMapImage()
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        var canvas = this.View.Canvas;
        var image = new Image { Stretch = Stretch.Uniform };
        RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.LowQuality);
        canvas.Children.Add(image);

        var map = game.Map;
        var pixelMap = map.PixelMap;
        int width = game.GameOptions.PixelWidth;
        int height = game.GameOptions.PixelHeight;
        byte[] bgraPixelData = new byte[width * height * 4];
        int byteIndex = 0;
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                int regionIndex = pixelMap.RegionAt(w, h);
                var ecosystem = map.Regions[regionIndex].Ecosystem;
                var color = GameViewModel.EcosystemToColor(ecosystem);
                bgraPixelData[byteIndex++] = color.B;
                bgraPixelData[byteIndex++] = color.G;
                bgraPixelData[byteIndex++] = color.R;
                bgraPixelData[byteIndex++] = 255;
            }
        }

        var dpi = new Vector(96, 96);
        var bitmap = new WriteableBitmap(new PixelSize(width, height), dpi, PixelFormat.Bgra8888, AlphaFormat.Premul);
        using (var frameBuffer = bitmap.Lock())
        {
            Marshal.Copy(bgraPixelData, 0, frameBuffer.Address, bgraPixelData.Length);
        }

        image.Source = bitmap;
        this.mapImage = image;
        this.mapImage.PointerMoved += this.OnPointerMoved;
        this.mapImage.PointerPressed += this.OnPointerPressed;
    }

    private static Color EcosystemToColor(Ecosystem ecosystem)
        => ecosystem switch
        {
            Ecosystem.Desert => Colors.Cornsilk,
            Ecosystem.Grassland => Colors.GreenYellow,
            Ecosystem.Forest => Colors.ForestGreen,
            Ecosystem.Mountain => Colors.SaddleBrown,
            Ecosystem.Hills => Colors.Peru,
            Ecosystem.Ocean => Colors.DarkBlue,
            Ecosystem.Wetland => Colors.LightSeaGreen,
            Ecosystem.Coast => Colors.PaleTurquoise,
            _ => Colors.LightGray,
        };

    private void GeneratePlayerBrushes()
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        this.playerBrushes.Clear();
        for (int i = 0; i < this.gameOptions.Players.Count; i++)
        {
            Color playerColor = PlayerToColor(game.Players[i]);
            this.playerBrushes.Add(new SolidColorBrush(playerColor));
        }
    }

    private static Color PlayerToColor(Player player) => player.Color.ColorNameToColor();

    #region Pointer Events Handlers 

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Image)
        {
            return;
        }

        // Send pixel position on the map to view model
        this.OnPointerPressedOnMap(e.GetPosition(this.mapImage), e.KeyModifiers);
        // e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Image)
        {
            return;
        }

        // Send pixel position on the map to view model
        this.OnPointerMovedOnMap(e.GetPosition(this.mapImage), e.KeyModifiers);
        // e.Handled = true;
    }

    private void OnPointerPressedOnMap(Point point, KeyModifiers keyModifiers)
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        var map = game.Map;
        if (!map.PixelMap.IsValidCoordinate((int)point.X, (int)point.Y))
        {
            return;
        }

        int regionIndex = map.PixelMap.RegionAt((int)point.X, (int)point.Y);
        var region = map.Regions[regionIndex];
        this.Messenger.Publish(new RegionSelectMessage(region, PointerAction.Clicked, keyModifiers));

        //if (this.sendTestResponseOnMapClick)
        //{
        //    this.sendTestResponseOnMapClick = false;
        //    if (keyModifiers == KeyModifiers.Shift)
        //    {
        //        this.Messenger.Publish(new GameSynchronizationResponse(MessageKind.Abort));
        //    }
        //    else
        //    {
        //        this.Messenger.Publish(new GameSynchronizationResponse(MessageKind.Test));
        //    }
        //}
    }

    private void OnPointerMovedOnMap(Point point, KeyModifiers keyModifiers)
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        var map = game.Map;
        if ( ! map.PixelMap.IsValidCoordinate((int)point.X, (int)point.Y))
        {
            return; 
        }
    
        int regionIndex = map.PixelMap.RegionAt((int)point.X, (int)point.Y);
        var region = map.Regions[regionIndex];
        if ((this.hoveredRegion is null) || (this.hoveredRegion != region))
        {
            this.Messenger.Publish(new RegionSelectMessage(region, PointerAction.Hovered, keyModifiers));
            this.hoveredRegion = region;
        }
    }
    
    #endregion Pointer Events Handlers 
}
