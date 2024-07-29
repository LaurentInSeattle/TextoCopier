namespace Lyt.Invasion.Model.Players;

public sealed class AiPlayer : Player
{
    public AiPlayer(int index, PlayerInfo playerInfo, Game game) : base(index, playerInfo, game)
    {
    }

    public override bool IsHuman => false;

    public override async Task<bool> Turn(CancellationToken cancellationToken)
    {
        Debug.WriteLine("Ai Player: " + this.Index.ToString());
        await Task.Delay(500, cancellationToken);
        return cancellationToken.IsCancellationRequested;
    }

    public override void Destroy()
    {
    }
}
