namespace Lyt.Invasion.Workflow.Gameplay.Regions;

public sealed partial class RegionViewModel : ViewModel<RegionView>
{
    private readonly InvasionModel invasionModel;

    private Region region; 

#pragma warning disable CS8618 
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Some non-nullable fields and properties get assigned when the view model is activated 
    public RegionViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster)
    {
#pragma warning restore CS8618 
        this.invasionModel = invasionModel;
        this.Messenger.Subscribe<RegionSelectMessage>(this.OnRegionSelect); 
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not Region region)
        {
            throw new ArgumentNullException(nameof(activationParameters));
        }

        this.region = region;
        Dispatch.OnUiThread(() => { this.UpdateUi(); });
    }

    private void OnRegionSelect(RegionSelectMessage message)
    {
        this.region = message.Region;
        this.UpdateUi();
    }

    private void UpdateUi()
    {
        // Things we want to display : 
        //    /// <summary> Resources available in the region. </summary>
        //public Resources Resources { get; internal set; }

        ///// <summary> Ecosystem of the region. (or region kind)</summary>
        //public Ecosystem Ecosystem { get; internal set; }

        ///// <summary> True if this region is a capital </summary>
        //public bool IsCapital { get; internal set; }

        ///// <summary> Player who currently owns the region, or null </summary>
        //public Player? Owner { get; private set; }
    }
}
