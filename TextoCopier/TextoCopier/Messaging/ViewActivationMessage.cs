namespace Lyt.TextoCopier.Messaging;

public sealed class ViewActivationMessage(ViewActivationMessage.StaticView view, object? parameter = null)
{
    public enum StaticView
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

    public StaticView View { get; private set; } = view;

    public object?  Parameter{ get; private set; } = parameter;
}
