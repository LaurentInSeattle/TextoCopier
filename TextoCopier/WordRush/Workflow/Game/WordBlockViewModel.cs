namespace Lyt.WordRush.Workflow.Game;

public sealed partial class WordBlockViewModel : ViewModel<WordBlockView>
{
    private readonly IAnimationService animationService;
    private readonly IRandomizer randomizer;

    [ObservableProperty]
    private string word;

    [ObservableProperty]
    private Brush foregroundBrush;

    [ObservableProperty]
    private Brush borderBrush;

    [ObservableProperty]
    private Brush backgroundBrush;

    public WordBlockViewModel(IAnimationService animationService, IRandomizer randomizer)
    {
        this.animationService = animationService;
        this.randomizer = randomizer;
        this.OriginalWord = string.Empty;
        this.MatchWord = string.Empty;
        this.IsAvailable = true;
        this.Word = string.Empty;
        this.ForegroundBrush = ColorTheme.Text;
        this.BackgroundBrush = ColorTheme.BoxAbsent;
        this.BorderBrush = ColorTheme.BoxBorder;
    }

    public override void OnViewLoaded()
    {
        this.Logger.Debug("WordBlockViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.View.Opacity = 0.0;
        this.Logger.Debug("WordBlockViewModel: OnViewLoaded complete");
    }

    public Language Language { get; private set; }

    public string OriginalWord { get; private set; }

    public string MatchWord { get; private set; }

    public bool IsAvailable { get; private set; }

    public bool IsClickable { get; private set; }

    public void Select(bool select = true)
        => this.BackgroundBrush = select ? ColorTheme.BoxPresent : ColorTheme.BoxAbsent;

    public void OnClick()
    {
        if (this.IsClickable)
        {
            new WordClickMessage(this, this.OriginalWord, this.Language).Publish();
            this.Logger.Debug("WordBlockViewModel: Click");
        }
    }

    public void OnEnter() => this.ForegroundBrush = ColorTheme.UiText;

    public void OnLeave() => this.ForegroundBrush = ColorTheme.Text;

    public void Setup(string word, string matchWord, Language language)
    {
        this.View.Opacity = 0.0;
        this.IsAvailable = false;
        this.OriginalWord = word;
        this.MatchWord = matchWord;
        this.Word = word.ToTitleCase();
        this.Language = language;
        this.BackgroundBrush = ColorTheme.BoxAbsent;
        this.FadeIn();
    }

    public void Show(bool show) => this.View.IsVisible = show;

    public void FadeIn()
    {
        Schedule.OnUiThread(
            300,
            () =>
            {
                this.animationService.FadeIn(this.View, 0.9);
            }, DispatcherPriority.Normal);
        Schedule.OnUiThread(
            1200,
            () =>
            {
                this.IsClickable = true;
            }, DispatcherPriority.Normal);
    }

    public void Fadeout()
    {
        this.IsClickable = false;
        double duration = this.randomizer.NextDouble(1.0, 1.5);
        this.animationService.FadeOut(this.View, duration);
        Schedule.OnUiThread(
            1500, 
            () => 
            {
                this.View.Opacity = 0.0;
                this.IsAvailable = true; 
            }, DispatcherPriority.Normal);
    }
}
