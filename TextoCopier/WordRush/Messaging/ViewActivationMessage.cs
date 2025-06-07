namespace Lyt.WordRush.Messaging;

public sealed record class ViewActivationMessage(
    ViewActivationMessage.ActivatedView View, object? ActivationParameter = null)
{
    public enum ActivatedView
    {
        Setup,
        Countdown,
        Game,
        GameOver,
        GoBack,
        Exit,
    }
}
