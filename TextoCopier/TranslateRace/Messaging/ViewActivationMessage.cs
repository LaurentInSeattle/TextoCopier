namespace Lyt.TranslateRace.Messaging;

public sealed record class ViewActivationMessage(
    ViewActivationMessage.ActivatedView View, object? ActivationParameter = null)
{
    public enum ActivatedView
    {
        Intro,
        Setup,
        NewParticipant,
        Game,
        GameOver,
        GoBack,
        Exit,
    }
}
