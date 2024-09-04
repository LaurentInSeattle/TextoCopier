namespace Lyt.TranslateRace.Messaging;

public sealed class ViewActivationMessage(ViewActivationMessage.ActivatedView view, object? activationParameter = null)
{
    public enum ActivatedView
    {
        Setup,
        Game,
        GameOver,
        GoBack,
        Exit,
    }

    public ActivatedView View { get; private set; } = view;

    public object? ActivationParameter { get; private set; } = activationParameter;
}
