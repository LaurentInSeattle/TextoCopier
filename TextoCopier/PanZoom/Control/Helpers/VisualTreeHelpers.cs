namespace Lyt.Avalonia.PanZoom;

public static class VisualTreeHelpers
{
    /// <summary> Find first child of type T in VisualTree. </summary>
    /// 
    // Apparently never used 
    //
    //public static T FindChildControl<T>(this DependencyObject control) where T : DependencyObject
    //{
    //    int childNumber = VisualTreeHelper.GetChildrenCount(control);
    //    for (var i = 0; i < childNumber; i++)
    //    {
    //        DependencyObject child = VisualTreeHelper.GetChild(control, i);
    //        return (child is T)
    //            ? (T)child : FindChildControl<T>(child);
    //    }
    //    return null;
    //}
}
