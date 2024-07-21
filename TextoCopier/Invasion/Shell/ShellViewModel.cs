using Lyt.Invasion.Model.GameControl;

namespace Lyt.Invasion.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private const byte red = 0x10;
    private const byte blu = 0x10;
    private const byte gre = 0x10;

    //private const byte red = 0xA0;
    //private const byte blu = 0xC0;
    //private const byte gre = 0xD0;

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
            MapSize = MapSize.Tiny,
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
        canvas.Width =  this.gameOptions.PixelWidth;
        canvas.Height = this.gameOptions.PixelHeight;
        canvas.InvalidateVisual();

        Schedule.OnUiThread( 500, this.SetupWorkflow, DispatcherPriority.Background);

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
            var center = region.Center;
            //Debug.WriteLine(center);
            var ellipse = new Ellipse
            {
                Width=10, Height=10,
                Stroke = Brushes.Red,
                Fill = Brushes.Transparent,
                StrokeThickness = 3.0,
            };
            ellipse.SetValue(Canvas.TopProperty, center.Y);
            ellipse.SetValue(Canvas.LeftProperty, center.X);
            canvas.Children.Add(ellipse);
            ++count;
            if (count > 1000)
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
            var path = region.SimplifiedPaths [0];
            var points = ( from v in path select new Point(v.X, v.Y) ).ToList();
            var polygon = new Polygon
            {
                Stroke = strokeBrush,
                Fill = Brushes.Transparent,
                Points = points ,
                StrokeThickness = 3.0,
            };

            polygon.SetValue(Canvas.TopProperty, 0.0);
            polygon.SetValue(Canvas.LeftProperty, 0.0);
            canvas.Children.Add(polygon);   
            ++count;
            if(count > 1000)
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

        var pixelMap = game.Map.PixelMap;
        int regionCount = game.GameOptions.RegionCount;
        var colors = new Color[regionCount];
        byte a = 255;
        for (int i = 0; i < regionCount; i++)
        {
            byte r = (byte)pixelMap.Random.Next(10, 200);
            byte g = (byte)pixelMap.Random.Next(10, 200);
            byte b = (byte)pixelMap.Random.Next(10, 200);
            colors[i] = new Color(a, r, g, b);
        }

        int width = game.GameOptions.PixelWidth;
        int height = game.GameOptions.PixelHeight;
        byte[] bgraPixelData = new byte[width * height*4];
        int byteIndex = 0;
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                int regionIndex = pixelMap.RegionAt(w, h);
                bool isBorder = pixelMap.IsBorderPixel[w, h];
                //if (isBorder)
                //{
                //    bgraPixelData[byteIndex++] = blu;
                //    bgraPixelData[byteIndex++] = red;
                //    bgraPixelData[byteIndex++] = gre;
                //}
                //else
                {
                    bgraPixelData[byteIndex++] = colors[regionIndex].B;
                    bgraPixelData[byteIndex++] = colors[regionIndex].G;
                    bgraPixelData[byteIndex++] = colors[regionIndex].R;
                }

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
}
