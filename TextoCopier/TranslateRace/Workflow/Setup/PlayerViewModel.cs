namespace Lyt.TranslateRace.Workflow.Setup;

public sealed class PlayerViewModel : Bindable<PlayerView>
{
    public PlayerViewModel(Participant participant)
    {
        this.Name = participant.Name;
    }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }
}
