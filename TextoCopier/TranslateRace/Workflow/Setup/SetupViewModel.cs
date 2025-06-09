namespace Lyt.TranslateRace.Workflow.Setup;

using static ViewActivationMessage;

public enum PlayerTeam
{
    Left,
    Middle,
    Right,
}

public sealed partial class SetupViewModel : ViewModel<SetupView>
{
    private readonly TranslateRaceModel translateRaceModel;

    [ObservableProperty]
    private string? leftTeamName;

    [ObservableProperty]
    private string? rightTeamName;

    [ObservableProperty]
    private ObservableCollection<PlayerViewModel> leftTeam;

    [ObservableProperty]
    private ObservableCollection<PlayerViewModel> middleTeam;

    [ObservableProperty]
    private ObservableCollection<PlayerViewModel> bottomTeam;

    [ObservableProperty]
    private ObservableCollection<PlayerViewModel> rightTeam;

    public SetupViewModel(TranslateRaceModel translateRaceModel)
    {
        this.translateRaceModel = translateRaceModel;
        this.Messenger.Subscribe<PlayerAssignmentMessage>(this.OnPlayerAssignmentMessage);
        this.LeftTeam = [];
        this.RightTeam = [];
        this.BottomTeam = [];
        this.MiddleTeam = [];
        this.UpdateTeamCounts();
    }

    public override void Activate(object? _)
    {
        this.LeftTeam.Clear();
        this.RightTeam.Clear();
        this.BottomTeam.Clear();
        this.MiddleTeam.Clear();
        this.LoadParticipants();
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
                this.Logger.Warning("Problema! Impossibile eliminare questo elemento dati!");
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
        this.UpdateTeamCounts(); 
    }

    // Update Team Counts
    private void UpdateTeamCounts()
    {
        this.LeftTeamName = string.Format("{0}  ({1})", Team.LeftName, this.LeftTeam.Count);
        this.RightTeamName = string.Format("{0}  ({1})", Team.RightName, this.RightTeam.Count);
    }

    [RelayCommand]
    public void OnSplit()
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

    [RelayCommand]
    public void OnAdd(object? _) => this.Messenger.Publish(ActivatedView.NewParticipant);

    [RelayCommand]
    public void OnNext(object? _)
    {
        if ((this.LeftTeam.Count == 0) || (this.RightTeam.Count == 0))
        {
            this.Logger.Warning("Problema! Not enough players");
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
        int playerIndex = 0;
        Team leftTeam = new(Team.LeftName, isLeft:true);
        foreach (var playerViewModel in this.LeftTeam)
        {
            leftTeam.Join(playerIndex, playerViewModel.Participant);
            ++playerIndex;
        }

        playerIndex = 0;
        Team rightTeam = new(Team.RightName, isLeft: false);
        foreach (var playerViewModel in this.RightTeam)
        {
            rightTeam.Join(playerIndex, playerViewModel.Participant);
            ++playerIndex;
        }

        // Create and populate game parameters 
        GameViewModel.Parameters parameters = new(leftTeam, rightTeam);

        // And... Rock and roll 
        this.Messenger.Publish(ActivatedView.Game, parameters);
    }
}
