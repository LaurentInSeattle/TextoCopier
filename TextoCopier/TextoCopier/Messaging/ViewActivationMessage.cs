namespace Lyt.TextoCopier.Messaging;

public sealed class ViewActivationMessage(ViewActivationMessage.ActivatedView view, object? activationParameter = null)
{
    public enum ActivatedView
    {
        Group,
        NewGroup,
        EditGroup,
        Help,
        Settings,
        NewTemplate,
        EditTemplate,

        GoBack,
    }

    public ActivatedView View { get; private set; } = view;

    public object? ActivationParameter { get; private set; } = activationParameter;
}
