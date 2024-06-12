namespace Lyt.TextoCopier.Messaging;

public sealed class ViewActivationMessage(ViewActivationMessage.StaticView view)
{
    public enum StaticView
    {
        Group,
        NewGroup,
        EditGroup,
        Help,
        Settings,
        NewTemplate,

        GoBack,
    }

    public StaticView View { get; private set; } = view;
}
