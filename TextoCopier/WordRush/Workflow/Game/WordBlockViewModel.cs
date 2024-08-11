namespace Lyt.WordRush.Workflow.Game;

public sealed class WordBlockViewModel : Bindable<WordBlockView>
{
    public WordBlockViewModel()
    {
        this.DisablePropertyChangedLogging = true;
        this.OriginalWord = string.Empty;
        this.Word = string.Empty;
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

        this.ForegroundBrush = ColorTheme.Text;
        this.BackgroundBrush = ColorTheme.BoxAbsent;
        this.BorderBrush = ColorTheme.BoxBorder;

        this.Logger.Debug("WordBlockViewModel: OnViewLoaded complete");
    }

    public Language Language { get; private set; }

    public string OriginalWord { get; private set; }

    public string MatchWord { get; private set; }

    public bool IsAvailable { get; set; }

    public void Select(bool select = true)
        => this.BackgroundBrush = select ? ColorTheme.BoxPresent : ColorTheme.BoxAbsent;

    public void OnClick()
    {
        this.Messenger.Publish(new WordClickMessage(this, this.OriginalWord, this.Language));
        this.Logger.Debug("WordBlockViewModel: Click");
    }

    public void OnEnter()
    {
        this.ForegroundBrush = ColorTheme.UiText;
    }

    public void OnLeave()
    {
        this.ForegroundBrush = ColorTheme.Text;
    }

    public void Setup(string word, string matchWord, Language language)
    {
        this.IsAvailable = false;
        this.OriginalWord = word;
        this.MatchWord = matchWord;
        this.Word = word.ToTitleCase();
        this.Language = language;
    }

    public void Show(bool show) => this.View.IsVisible = show;

    public void FadeIn()
    {

    }

    public void Fadeout()
    {

    }

    public string Word { get => this.Get<string>()!; set => this.Set(value); }

    public Brush ForegroundBrush { get => this.Get<Brush>()!; set => this.Set(value); }

    public Brush BorderBrush { get => this.Get<Brush>()!; set => this.Set(value); }

    public Brush BackgroundBrush { get => this.Get<Brush>()!; set => this.Set(value); }
}
