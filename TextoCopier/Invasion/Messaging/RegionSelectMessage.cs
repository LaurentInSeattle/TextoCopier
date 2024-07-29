namespace Lyt.Invasion.Messaging; 

public enum PointerAction
{
    Hovered, 
    Clicked,
}

public sealed class RegionSelectMessage (Region region, PointerAction pointerAction, KeyModifiers keyModifiers)
{
    public Region Region { get; set; } = region;

    public PointerAction PointerAction { get; set; } = pointerAction;

    public KeyModifiers KeyModifiers { get; set; } = keyModifiers;
}
