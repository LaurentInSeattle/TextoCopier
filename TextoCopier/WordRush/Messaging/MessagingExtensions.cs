namespace Lyt.WordRush.Messaging; 

public static class MessagingExtensions
{
    public static void Publish(
        this IMessenger messenger, ViewActivationMessage.ActivatedView view, object? activationParameter = null)
        => messenger.Publish(new ViewActivationMessage(view, activationParameter));
}
