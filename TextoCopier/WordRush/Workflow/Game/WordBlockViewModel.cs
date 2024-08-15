namespace Lyt.WordRush.Workflow.Game;

public sealed class WordBlockViewModel : Bindable<WordBlockView>
{
    private readonly IAnimationService animationService;
    private readonly IRandomizer randomizer;

    public WordBlockViewModel(IAnimationService animationService, IRandomizer randomizer)
    {
        this.animationService = animationService;
        this.randomizer = randomizer;
        this.DisablePropertyChangedLogging = true;
        this.OriginalWord = string.Empty;
        this.Word = string.Empty;
        this.MatchWord = string.Empty;
        this.IsAvailable = true;
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("WordBlockViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.View.Opacity = 0.0;
        this.ForegroundBrush = ColorTheme.Text;
        this.BackgroundBrush = ColorTheme.BoxAbsent;
        this.BorderBrush = ColorTheme.BoxBorder;

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
            this.Messenger.Publish(new WordClickMessage(this, this.OriginalWord, this.Language));
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

    public string Word { get => this.Get<string>()!; set => this.Set(value); }

    public Brush ForegroundBrush { get => this.Get<Brush>()!; set => this.Set(value); }

    public Brush BorderBrush { get => this.Get<Brush>()!; set => this.Set(value); }

    public Brush BackgroundBrush { get => this.Get<Brush>()!; set => this.Set(value); }
}
