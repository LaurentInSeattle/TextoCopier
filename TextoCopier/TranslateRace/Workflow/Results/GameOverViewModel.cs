namespace Lyt.TranslateRace.Workflow.Results;

using static ViewActivationMessage;

public sealed partial class GameOverViewModel(TranslateRaceModel model) : ViewModel<GameOverView>
{
    private readonly TranslateRaceModel model = model;

    [ObservableProperty]
    private string? gameOver;

    [ObservableProperty]
    private Brush? gameOverColor;

    [ObservableProperty]
    private string? isWon;

    [ObservableProperty]
    private Brush? isWonColor;

    [ObservableProperty]
    private string? finalScore;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.Profiler.FullGcCollect();
        this.ShowResults();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.Profiler.FullGcCollect();
    }

    [RelayCommand]
    public void OnExit(object? _) => this.Messenger.Publish(ActivatedView.Exit);

    [RelayCommand]
    public void OnPlayAgain(object? _) => this.Messenger.Publish(ActivatedView.Setup);

    private void ShowResults()
    {
        var win = this.model.WinningTeam!;
        var lost = this.model.LosingTeam!;
        this.GameOver = "Fine del Gioco";
        this.IsWonColor = ColorTheme.ValidUiText;
        this.FinalScore = string.Format("Punteggio finale: {0} / {1}", win.Score, lost.Score);
        if (win.Score > lost.Score)
        {
            this.IsWon = "La " + win.Name + " ha vinto!";
        }
        else
        {
            this.IsWon = "Vincono entrambe le due squadre!"; 
        } 
    }
}
