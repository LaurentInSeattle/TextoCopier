namespace Lyt.TranslateRace.Workflow.Setup;

using System.Collections.ObjectModel;
using static Lyt.TranslateRace.Messaging.ViewActivationMessage;

public enum PlayerTeam
{
    Left,
    Middle,
    Right,
}

public sealed class SetupViewModel : Bindable<SetupView>
{
    private readonly IMessenger messenger;
    private readonly IToaster toaster;
    private readonly IDialogService dialogService;
    private readonly TranslateRaceModel translateRaceModel;

    public SetupViewModel(
        IMessenger messenger, IDialogService dialogService, IToaster toaster, TranslateRaceModel translateRaceModel)
    {
        this.messenger = messenger;
        this.toaster = toaster;
        this.dialogService = dialogService;
        this.translateRaceModel = translateRaceModel;
        this.messenger.Subscribe<PlayerAssignmentMessage>(this.OnPlayerAssignmentMessage);
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("SetupViewModel: OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.LoadParticipants();
        this.Logger.Debug("SetupViewModel: OnViewLoaded complete");
    }

    private void OnPlayerAssignmentMessage(PlayerAssignmentMessage message)
    {
        ObservableCollection<PlayerViewModel> fromCollection = 
            message.FromAssignment switch
            {
                Assignment.Right => this.RightTeam,
                Assignment.Absent => this.BottomTeam,
                Assignment.Participant => this.MiddleTeam,
                _ => this.LeftTeam,
            };
        fromCollection.Remove(message.PlayerViewModel);

        ObservableCollection<PlayerViewModel> toCollection =
            message.ToAssignment switch
            {
                Assignment.Right => this.RightTeam,
                Assignment.Absent => this.BottomTeam,
                Assignment.Participant => this.MiddleTeam,
                _ => this.LeftTeam,
            };
        toCollection.Add( new PlayerViewModel(message.PlayerViewModel.Participant, message.ToAssignment));
    }

    private void LoadParticipants()
    {
        List<PlayerViewModel> participants = new(this.translateRaceModel.Participants.Count);
        foreach (var participant in this.translateRaceModel.Participants)
        {
            var vm = new PlayerViewModel(participant, Assignment.Participant);
            participants.Add(vm);
        }

        this.LeftTeam = [];
        this.RightTeam = [];
        this.BottomTeam = [];
        this.MiddleTeam = new ObservableCollection<PlayerViewModel>( participants);
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnSplit(object? _)
    {

    }

    private void OnAdd(object? _)
    {

    }

    private void OnNext(object? _) => this.Messenger.Publish(ActivatedView.Game);

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    #region Bound properties 

    public ICommand SplitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AddCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> LeftTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> MiddleTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> BottomTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> RightTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    #endregion Bound properties 
}
