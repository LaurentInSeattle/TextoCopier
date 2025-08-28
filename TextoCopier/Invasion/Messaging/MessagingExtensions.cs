namespace Lyt.Invasion.Messaging; 

public static class AppMessagingExtensions
{
    public static void Publish(ViewActivationMessage.ActivatedView view, object? activationParameter = null)
        => new ViewActivationMessage(view, activationParameter).Publish();
}
