namespace Lyt.Invasion.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    // Border paths color components
    private const byte red = 0x10;
    private const byte blu = 0x20;
    private const byte gre = 0x10;

    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;
    private readonly GameOptions gameOptions;

    public ShellViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;

        this.gameOptions = new GameOptions
        {
            MapSize = MapSize.Huge,
            Players =
            [
                 new PlayerInfo { Name = "Laurent", IsHuman =true},
                 new PlayerInfo { Name = "Annalisa", IsHuman =true},
                 new PlayerInfo { Name = "Oksana"},
                 new PlayerInfo { Name = "Irina"},
            ],
        };
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Select default language 
        //this.localizer.DetectAvailableLanguages();
        //string preferredLanguage = this.templatesModel.Language;
        //this.Logger.Debug("Language: " + preferredLanguage);
        //this.localizer.SelectLanguage(preferredLanguage);

        this.Logger.Debug("OnViewLoaded language loaded");

        var canvas = this.View.Canvas;
        canvas.Width = this.gameOptions.PixelWidth;
        canvas.Height = this.gameOptions.PixelHeight;
        canvas.InvalidateVisual();

        Schedule.OnUiThread(500, this.SetupWorkflow, DispatcherPriority.Background);

        this.Logger.Debug("OnViewLoaded complete");
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    private void SetupWorkflow()
    {
        this.invasionModel.NewGame(this.gameOptions);
        Schedule.OnUiThread(500, this.UpdateUi, DispatcherPriority.Background);
    }

    private void UpdateUi()
    {
        this.GenerateMapImage();
        this.GeneratePaths();
        this.GenerateCenters();

        //static void CreateAndBind<TViewModel, TControl>()
        //     where TViewModel : Bindable<TControl>
        //     where TControl : Control, new()
        // {
        //     var vm = App.GetRequiredService<TViewModel>();
        //     vm.CreateViewAndBind();
        // }
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
            if (( region.Ecosystem == Ecosystem.Ocean) || (region.Ecosystem == Ecosystem.Mountain))
            {
                continue;
            }

            var center = region.AltCenter;
            var ellipse = new Ellipse
            {
                Width = 12,
                Height = 12,
                Stroke = Brushes.Firebrick,
                Fill = Brushes.Black,
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
        var strokeBrush = new SolidColorBrush(Color.FromRgb(red, blu, gre));
        foreach (var region in map.Regions)
        {
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
        int regionCount = game.GameOptions.RegionCount;

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
                var color = ShellViewModel.EcosystemToColor(ecosystem);
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
}
