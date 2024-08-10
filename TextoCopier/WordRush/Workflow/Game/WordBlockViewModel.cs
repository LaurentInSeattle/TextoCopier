namespace Lyt.WordRush.Workflow.Game;

public sealed class WordBlockViewModel : Bindable<WordBlockView>
{
    private Language language;

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

    public string OriginalWord { get; private set; }

    public bool IsAvailable { get; set; }

    public void OnClick()
    {
        if (this.language != Language.English)
        {
            return ;
        }

        this.Messenger.Publish(new WordClickMessage(this.OriginalWord, this.language));
        this.Logger.Debug("WordBlockViewModel: Click");
    }


    public void OnEnter()
    {
        // TODO: Adjust colors
        //this.Logger.Debug("WordBlockViewModel: Enter");
        if (this.language != Language.English)
        {
            this.ForegroundBrush = ColorTheme.TextAbsent;
        }
        else
        {
            this.ForegroundBrush = ColorTheme.UiText;
        } 
    }

    public void OnLeave()
    {
        // TODO: Adjust colors
        // this.Logger.Debug("WordBlockViewModel: Leave");
        this.ForegroundBrush = ColorTheme.Text;
    }

    public void Setup(string word, Language language)
    {
        this.IsAvailable = false;
        this.OriginalWord = word;
        this.Word = word.ToTitleCase();
        this.language = language;
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
