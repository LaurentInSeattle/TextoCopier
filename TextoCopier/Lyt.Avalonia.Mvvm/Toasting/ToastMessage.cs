namespace Lyt.Avalonia.Controls.Toasting;

public sealed class ToastMessage
{
    public sealed class Show
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Delay { get; set; } = 10_000; 
        public ToastLevel Level { get; set; } = ToastLevel.Info;
    }

    public sealed class Dismiss { }

    public sealed class OnDismiss { }
}
