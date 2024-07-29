namespace Lyt.Invasion.Model.Players;

public sealed class HumanPlayer : Player
{
    public HumanPlayer(int index, PlayerInfo playerInfo, Game game) : base(index, playerInfo, game)
    {
    }

    public override bool IsHuman => true;

    public override async Task<bool> Turn(CancellationToken cancellationToken)
    {
        bool abort = false; 
        Debug.WriteLine("Human Player: " + this.Name);
        await Task.Delay(500, cancellationToken);
        bool sync = this.Game.Synchronize(MessageKind.Test, out GameSynchronizationResponse? response, cancellationToken);
        if (sync && (response is not null)) 
        {
            Debug.WriteLine(response.ToString() + " " + response.Message.ToString());
            abort = response.Message == MessageKind.Abort;
        }
        else
        {
            Debug.WriteLine("Abort");
            abort = true;   
        }

        return abort || cancellationToken.IsCancellationRequested;
    }

    public override void Destroy()
    {
    }
}
