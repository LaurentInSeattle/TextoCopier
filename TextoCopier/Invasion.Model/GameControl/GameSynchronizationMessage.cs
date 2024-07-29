namespace Lyt.Invasion.Model.GameControl;

public enum MessageKind
{
    None = 0,
    Abort,
    GameOver, 

    Test,
}

public sealed class GameSynchronizationRequest(MessageKind message, object? gameData = null)
{
    public MessageKind Message { get; set; } = message;

    public object? GameData { get; set; } = gameData;
}

public sealed class GameSynchronizationResponse(MessageKind message, object? gameData = null)
{
    public MessageKind Message { get; set; } = message;

    public object? GameData { get; set; } = gameData;
}
