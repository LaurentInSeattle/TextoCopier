namespace Lyt.Invasion.Workflow.Gameplay.Players;

public sealed partial class PlayerViewModel : ViewModel<PlayerView>
{
    private readonly InvasionModel invasionModel;

    private Player player;

#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Some non-nullable fields and properties get assigned when the view model is activated 
    public PlayerViewModel(InvasionModel invasionModel) => this.invasionModel = invasionModel;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not Player player)
        {
            throw new ArgumentNullException(nameof(activationParameters));
        }

        this.player = player;
        Dispatch.OnUiThread(() => { this.UpdateUi(); });
    }

    private void UpdateUi()
    {
        // Things we want to display : 
        //public abstract bool IsHuman { get; }

        //public string Name { get; set; } = string.Empty;

        //public string EmpireName { get; set; } = string.Empty;

        //public string Avatar { get; set; } = string.Empty;

        //public string Color { get; set; } = string.Empty;

        //public Region Capital { get; internal set; }

        //public List<Region> Territory { get; private set; }

        //public Age Age { get; private set; }

        //public readonly Wealth Wealth;

        //public readonly Dictionary<ActorKind, int> Population;
    }
}
