namespace Lyt.WordRush.Workflow.Game;

public sealed class WordBlockViewModel : Bindable<WordBlockView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;

    private Language language; 

    public WordBlockViewModel( LocalizerModel localizer, IDialogService dialogService, IToaster toaster)
    {
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("WordBlockViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.Word = " ? ? ? ";
        this.ForegroundBrush = ColorTheme.Text;
        this.BackgroundBrush = ColorTheme.BoxAbsent;
        this.BorderBrush = ColorTheme.BoxBorder;

        this.Logger.Debug("WordBlockViewModel: OnViewLoaded complete");
    }

    public void OnClick ( ) => this.Messenger.Publish(new WordClickMessage(this.Word, this.language));
    

    public void OnEnter()
    {
        // TODO: Adjust colors
    }

    public void OnLeave()
    {
        // TODO: Adjust colors
    }

    public void Setup(string word, Language language)
    {
        this.Word = word;
        this.language = language;
    }

    public void Show ( bool show) => this.View.IsVisible = show; 

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
