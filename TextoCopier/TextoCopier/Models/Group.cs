namespace Lyt.TextoCopier.Models; 

public sealed class Group
{
    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty; 

    public List<Template> Templates { get; set; } = [] ;
}
