namespace Lyt.Invasion.Workflow.Gameplay;

using Lyt.Invasion.Model.GameControl;
using static ViewActivationMessage;

public sealed class GameViewModel : Bindable<GameView>
{
    // Border paths color components
    private const byte red = 0x10;
    private const byte blu = 0x20;
    private const byte gre = 0x10;

    private SolidColorBrush[] playerBrushes;
    private GameOptions gameOptions;

    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

    private Region? hoveredRegion;

#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Some non-nullable fields and properties get assigned when the view model is activated 
    public GameViewModel(
#pragma warning restore CS8618 
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster)
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;
    }

    protected override void OnViewLoaded()
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
        this.playerBrushes = new SolidColorBrush[8];
        var canvas = this.View.Canvas;
        canvas.Width = this.gameOptions.PixelWidth;
        canvas.Height = this.gameOptions.PixelHeight;
        canvas.InvalidateVisual();

        Dispatch.OnUiThread( () => { this.UpdateUi(); });
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);


#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    private void UpdateUi()
    {
        // TODO: Create the game model in a background thread 
        this.invasionModel.NewGame(this.gameOptions);

        this.GeneratePlayerBrushes();
        this.GenerateMapImage();
        this.GeneratePaths();
        this.GenerateCenters();

        this.Logger.Info("Ui generated");
        this.Profiler.MemorySnapshot("Ui generated");
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

        int count = 0;
        var canvas = this.View.Canvas;
        var map = game.Map;
        var unclaimedStrokeBrush = new SolidColorBrush(Color.FromRgb(red, blu, gre));
        foreach (var region in map.Regions)
        {
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
            }

            ++count;
            if (count > 1000)
            {
                break;
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

        this.MapImage = bitmap;
    }

    public Bitmap? MapImage { get => this.Get<Bitmap?>(); set => this.Set(value); }

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

        for (int i = 0; i < this.gameOptions.Players.Count; i++)
        {
            Color playerColor = PlayerToColor(game.Players[i]);
            this.playerBrushes[i] = new SolidColorBrush(playerColor);
        }
    }

    private static Color PlayerToColor(Player player) => player.Color.ColorNameToColor();

    public void OnPointerPressedOnMap(Point point, KeyModifiers keyModifiers)
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        var map = game.Map;
        int regionIndex = map.PixelMap.RegionAt((int)point.X, (int)point.Y);
        var region = map.Regions[regionIndex];
        this.Messenger.Publish(new RegionSelectMessage(region, PointerAction.Clicked, keyModifiers));
    }

    public void OnPointerMovedOnMap(Point point, KeyModifiers keyModifiers)
    {
        var game = this.invasionModel.Game;
        if (game is null)
        {
            return;
        }

        var map = game.Map;
        int regionIndex = map.PixelMap.RegionAt((int) point.X, (int)point.Y);
        var region = map.Regions[regionIndex];
        if ((this.hoveredRegion is null) || (this.hoveredRegion != region))
        {
            this.Messenger.Publish(new RegionSelectMessage(region, PointerAction.Hovered, keyModifiers));
            this.hoveredRegion = region;
        }
    }
}
