namespace Lyt.Avalonia.Controls;

public static class Utilities
{
    public static bool TryFindResource<T>(string resourceName, out T? resource)
    {
        resource = default;
        try
        {
            if (Application.Current is null)
            {
                return false;
            }

            bool found = Application.Current.TryFindResource(resourceName, out object? resourceObject);
            if (found && resourceObject is T resourceTypeT)
            {
                resource = resourceTypeT;
                return true;
            }
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(ex);
        }

        return false;
    }

    public static void ApplyControlTheme(this Control control, ControlTheme theme)
    {
        if (theme.Setters is null)
        {
            return;
        }

        foreach (var item in theme.Setters)
        {
            var setter = item as Setter;
            if ((setter is not null) && (setter.Property is not null))
            {
                control.SetCurrentValue(setter.Property, setter.Value);
            }
        }
    }
}
