namespace Lyt.Avalonia.Interfaces.UserInterface;

public interface IToaster
{
    object? Host { get; set; }

    void Show(string title, string message, int dismissDelay = 10, ToastLevel toastLevel = ToastLevel.Info);

    void Dismiss();
}
