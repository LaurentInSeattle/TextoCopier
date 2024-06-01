namespace Lyt.Avalonia.Controls.Toasting;

public sealed class ToastViewModel(IToaster toaster) : Bindable<ToastView>
{
    private const int NoDelay = 0;
    private const int MinDelay = 1000;
    private const int MaxDelay = 60_000;

    private readonly IToaster toaster = toaster;

    private DispatcherTimer? dismissTimer;

    public void Show(string title, string message, int dismissDelay, ToastLevel toastLevel)
    {
        this.Title = title;
        this.Message = message;
        SolidColorBrush? brush;
        switch (toastLevel)
        {
            default:
            case ToastLevel.Info:
                this.IconName = "info";
                Utilities.TryFindResource<SolidColorBrush>("LightAqua_0_120", out brush);
                break;

            case ToastLevel.Warning:
                this.IconName = "warning";
                Utilities.TryFindResource<SolidColorBrush>("OrangePeel_0_100", out brush);
                break;

            case ToastLevel.Error:
                this.IconName = "error_circle";
                Utilities.TryFindResource<SolidColorBrush>("PastelOrchid_1_100", out brush);
                break;
        }

        if (brush is not null)
        {
            this.ColorLevel = brush;
        }

        // We should not need to do that !!!
        this.View!.Icon.Source = this.IconName;

        this.DismissCommand = new Command(this.Dismiss);

        if (dismissDelay == ToastViewModel.NoDelay)
        {
            // dismiss on click or explicit request 
        }
        else
        {
            // auto dismiss after delay
            if (dismissDelay < ToastViewModel.MinDelay)
            {
                dismissDelay = ToastViewModel.MinDelay;
            }
            else if (dismissDelay > ToastViewModel.MaxDelay)
            {
                dismissDelay = ToastViewModel.MaxDelay;
            }

            this.dismissTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(dismissDelay),
                IsEnabled = true,
            };
            this.dismissTimer.Tick += this.DismissTimerTick;
        }
    }

    private void DismissTimerTick(object? _, EventArgs e) => this.Dismiss();

    private void Dismiss(object? _) => this.Dismiss();

    public void Dismiss()
    {
        this.StopTimer();
        this.toaster.Dismiss();
    }

    private void StopTimer()
    {
        if (this.dismissTimer != null)
        {
            this.dismissTimer.Stop();
            this.dismissTimer = null;
        }
    }

    public ICommand DismissCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    /// <summary> Gets or sets the ColorLevel bound property.</summary>
    public SolidColorBrush ColorLevel { get => this.Get<SolidColorBrush>()!; set => _ = this.Set(value); }

    /// <summary> Gets or sets the IconName bound property.</summary>
    public string IconName { get => this.Get<string>()!; set => _ = this.Set(value); }

    /// <summary> Gets or sets the Title bound property.</summary>
    public string? Title { get => this.Get<string>(); set => _ = this.Set(value); }

    /// <summary> Gets or sets the Message bound property.</summary>
    public string? Message { get => this.Get<string>(); set => _ = this.Set(value); }
}
