namespace Lyt.TextoCopier.Messaging;

public sealed record class ViewActivationMessage(
    ViewActivationMessage.ActivatedView View, object? ActivationParameter = null)
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
}
