using Avalonia.Media.Imaging;
using System.Linq;

namespace Lyt.Invasion.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

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

        this.SetupWorkflow();
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
        var gameOptions = new GameOptions
        {
            MapSize = MapSize.Medium,
            Players =
            [
                 new PlayerInfo { Name = "Laurent", IsHuman =true},
                 new PlayerInfo { Name = "Annalisa", IsHuman =true},
                 new PlayerInfo { Name = "Oksana"},
                 new PlayerInfo { Name = "Irina"},
            ],
        };

        this.invasionModel.NewGame(gameOptions);
        this.GenerateMapImage();
        //static void CreateAndBind<TViewModel, TControl>()
        //     where TViewModel : Bindable<TControl>
        //     where TControl : Control, new()
        // {
        //     var vm = App.GetRequiredService<TViewModel>();
        //     vm.CreateViewAndBind();
        // }
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
                if (isBorder)
                {
                    bgraPixelData[byteIndex++] = 255;
                    bgraPixelData[byteIndex++] = 255;
                    bgraPixelData[byteIndex++] = 255;
                }
                else
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
