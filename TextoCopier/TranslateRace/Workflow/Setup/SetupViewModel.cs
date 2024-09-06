namespace Lyt.TranslateRace.Workflow.Setup;

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
        if ((message.PlayerViewModel is not PlayerViewModel playerViewModel) ||
            (playerViewModel.Participant is not Participant participant))
        {
            return;
        }

        ObservableCollection<PlayerViewModel> fromCollection =
            message.FromAssignment switch
            {
                Assignment.Right => this.RightTeam,
                Assignment.Absent => this.BottomTeam,
                Assignment.Participant => this.MiddleTeam,
                _ => this.LeftTeam,
            };
        fromCollection.Remove(playerViewModel);

        if (message.ToAssignment == Assignment.Delete)
        {
            // Delete participant 
            if (!this.translateRaceModel.DeleteParticipant(participant))
            {
                // Toast: Failed to delete 
                this.toaster.Show("Problema!", "Impossibile eliminare questo elemento dati!", 3_000, InformationLevel.Warning);
            }
        }
        else
        {
            ObservableCollection<PlayerViewModel> toCollection =
                message.ToAssignment switch
                {
                    Assignment.Right => this.RightTeam,
                    Assignment.Absent => this.BottomTeam,
                    Assignment.Participant => this.MiddleTeam,
                    _ => this.LeftTeam,
                };
            toCollection.Add(new PlayerViewModel(participant, message.ToAssignment));
        }

        this.ReorderAndUpdateTeamCounts();
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
        this.MiddleTeam = new ObservableCollection<PlayerViewModel>(participants);
        this.ReorderAndUpdateTeamCounts();
    }

    private void ReorderAndUpdateTeamCounts()
    {
        // Reorder
        this.LeftTeam = new ObservableCollection<PlayerViewModel>(this.LeftTeam.OrderBy(vm => vm.Name));
        this.RightTeam = new ObservableCollection<PlayerViewModel>(this.RightTeam.OrderBy(vm => vm.Name));
        this.BottomTeam = new ObservableCollection<PlayerViewModel>(this.BottomTeam.OrderBy(vm => vm.Name));
        this.MiddleTeam = new ObservableCollection<PlayerViewModel>(this.MiddleTeam.OrderBy(vm => vm.Name));

        // Update Team Counts
        this.LeftTeamPlayerCount = string.Format("({0})", this.LeftTeam.Count);
        this.RightTeamPlayerCount = string.Format("({0})", this.RightTeam.Count);
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnSplit(object? _)
    {
        if (this.MiddleTeam.Count == 0)
        {
            return;
        }

        bool addLeft = true;
        foreach (var playerViewModel in this.MiddleTeam)
        {
            if (addLeft)
            {
                this.LeftTeam.Add(new PlayerViewModel(playerViewModel.Participant, Assignment.Left));
            }
            else
            {
                this.RightTeam.Add(new PlayerViewModel(playerViewModel.Participant, Assignment.Right));
            }

            addLeft = !addLeft;
        }

        this.MiddleTeam.Clear();
        this.ReorderAndUpdateTeamCounts();
    }

    private void OnAdd(object? _)
    {

    }

    private void OnNext(object? _)
    {
        if ((this.LeftTeam.Count == 0) || (this.RightTeam.Count == 0))
        {
            // Toast: Not enough players
            this.toaster.Show("Problema!", "Non ci sono abbastanza giocatori!", 3_000, InformationLevel.Warning);
            return;
        }

        // Update players participations 
        foreach (var playerViewModel in this.LeftTeam)
        {
            ++playerViewModel.Participant.Participations;
        }

        foreach (var playerViewModel in this.RightTeam)
        {
            ++playerViewModel.Participant.Participations;
        }

        // Save Participants model 
        this.translateRaceModel.Save();

        // Create players and teams 
        Team leftTeam = new("Squadra Azzurra");
        foreach (var playerViewModel in this.LeftTeam)
        {
            leftTeam.Join(playerViewModel.Participant);
        }

        Team rightTeam = new("Scuderia Ferrari");
        foreach (var playerViewModel in this.RightTeam)
        {
            rightTeam.Join(playerViewModel.Participant);
        }

        // Create and populate game parameters 
        GameViewModel.Parameters parameters = new(leftTeam, rightTeam);

        // And... Rock and roll 
        this.Messenger.Publish(ActivatedView.Game, parameters);
    }

#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    #region Bound properties 

    public ICommand SplitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AddCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string LeftTeamPlayerCount { get => this.Get<string>()!; set => this.Set(value); }

    public string RightTeamPlayerCount { get => this.Get<string>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> LeftTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> MiddleTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> BottomTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    public ObservableCollection<PlayerViewModel> RightTeam { get => this.Get<ObservableCollection<PlayerViewModel>>()!; set => this.Set(value); }

    #endregion Bound properties 
}
