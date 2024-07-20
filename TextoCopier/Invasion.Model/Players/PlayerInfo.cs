namespace Lyt.Invasion.Model.Players;

/// <summary> Information collected in the UI so that the model can create actual players.</summary>
public sealed class PlayerInfo
{
    public string Name { get; set; }  = string.Empty;
    public bool IsHuman { get; set; }
}
