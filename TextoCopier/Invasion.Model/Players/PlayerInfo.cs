namespace Lyt.Invasion.Model.Players;

/// <summary> Information collected in the UI so that the model can create actual players.</summary>
public sealed class PlayerInfo
{
    public bool IsHuman { get; set; }

    public string Name { get; set; }  = string.Empty;

    public string EmpireName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;
}
