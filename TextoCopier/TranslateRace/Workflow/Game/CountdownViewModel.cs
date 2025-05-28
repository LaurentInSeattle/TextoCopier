namespace Lyt.TranslateRace.Workflow.Game;

public sealed partial class CountdownViewModel : ViewModel<CountdownView> 
{
    private const int DurationMilliseconds = 120_000;

    [ObservableProperty]
    private bool visible;

    [ObservableProperty]
    private float countDownTotal;

    [ObservableProperty]
    private float countDownValue;

    [ObservableProperty]
    private string? timeLeftText;

    private DispatcherTimer? dispatcherTimer;
    private DateTime countdownStart;

    public CountdownViewModel()
    {
        this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120), IsEnabled = false };
        this.dispatcherTimer.Tick += this.OnDispatcherTimerTick;
        this.Visible = true;
    }

    ~CountdownViewModel()
    {
        this.dispatcherTimer = null;  
    }

    public void Start()
    {
        this.Visible = true;
        this.TimeUsed = TimeSpan.Zero;
        this.TimeLeftText = string.Empty;
        this.countdownStart = DateTime.Now;
        this.CountDownTotal = (float)CountdownViewModel.DurationMilliseconds;
        this.StartTimer(); 
    }

    public TimeSpan TimeUsed { get; private set; }

    public void Stop() =>  this.StopTimer();

    private void OnDispatcherTimerTick(object? sender, EventArgs e)
    {
        if (!this.Visible )
        {
            return;
        }

        this.Visible = true;
        this.TimeUsed = DateTime.Now - this.countdownStart; 
        int elapsed = (int)this.TimeUsed.TotalMilliseconds;
        int left = CountdownViewModel.DurationMilliseconds - elapsed;
        this.CountDownValue = (float)left;
        var timeLeft = TimeSpan.FromMilliseconds(left);
        this.TimeLeftText = string.Format("{0}:{1:D2}", timeLeft.Minutes, timeLeft.Seconds);

        if (((timeLeft.Minutes < 0) || (timeLeft.Seconds < 0)) ||
            ((timeLeft.Minutes == 0) && (timeLeft.Seconds == 0)))
        {
            this.StopTimer();
            return;
        }
    }

    private void StartTimer()
    {
        Schedule.OnUiThread(
            500,
            () =>
            {
                if (this.dispatcherTimer is not null)
                {
                    this.dispatcherTimer.IsEnabled = true;
                    this.dispatcherTimer.Start();
                }
            }, DispatcherPriority.Normal);
    }

    private void StopTimer()
    {
        if (this.dispatcherTimer is not null)
        {
            this.dispatcherTimer.Stop();
            this.dispatcherTimer.IsEnabled = false;
        }
    }
}
