﻿namespace Lyt.Invasion.Workflow.Setup;

using static ViewActivationMessage;

//{
//    MapSize = MapSize.Large,
//    Difficulty = GameDifficulty.Fair,
//    Players =
//    [
//         new PlayerInfo { Name = "Laurent", IsHuman =true, Color = "Crimson"},
//         new PlayerInfo { Name = "Annalisa", IsHuman =true, Color = "DarkTurquoise"},
//         new PlayerInfo { Name = "Oksana", Color = "DarkOrange"},
//         new PlayerInfo { Name = "Irina", Color = "HotPink"},
//    ],
//};

public enum PlayerColor : int
{
    Crimson = 0,
    DarkTurquoise = 1,
    DarkOrange = 2,
    HotPink = 3,
}

public sealed class PlayerSetupViewModel : Bindable<PlayerSetupView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly LocalizerModel localizer;
    private readonly InvasionModel invasionModel;

    private GameOptions gameOptions;
    private List<PlayerInfo> humanPlayers;
    private PlayerInfo currentPlayer;
    private int currentPlayerIndex;

#pragma warning disable CS8618 
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Some non-nullable fields (ex: gameOptions) and properties get assigned when the view model is activated 
    public PlayerSetupViewModel(
        LocalizerModel localizer, InvasionModel invasionModel,
        IDialogService dialogService, IToaster toaster)
#pragma warning restore CS8618
    {
        this.localizer = localizer;
        this.invasionModel = invasionModel;
        this.dialogService = dialogService;
        this.toaster = toaster;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (activationParameters is not GameOptions gameOptions)
        {
            throw new ArgumentNullException(nameof(activationParameters));
        }

        this.gameOptions = gameOptions;
        this.humanPlayers = (from player in this.gameOptions.Players where player.IsHuman select player).ToList();
        this.currentPlayerIndex = 0;
        this.currentPlayer = this.humanPlayers[this.currentPlayerIndex];
        this.UpdateUi();
    }

    private void UpdateUi()
    {
        void UpdatePlayerColor()
        {
            if (Enum.TryParse<PlayerColor>(this.currentPlayer.Color, out PlayerColor playerColor))
            {
                this.PlayerColor = playerColor;
            }
            else
            {
                throw new Exception("Invalid player string color");
            }
        }

        void UpdateButton()
        {
            if (this.currentPlayerIndex == this.humanPlayers.Count - 1)
            {
                this.NextButtonText = "Start the Game!";
            }
            else
            {
                this.NextButtonText = "Next Player";
            }
        }

        UpdatePlayerColor();
        UpdateButton();
    }


    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnPlayerColorChanged (PlayerColor _ , PlayerColor playerColor)
    {
        this.currentPlayer.Color = playerColor.ToString();
    }

    private void OnBack(object? _)
    {
        if (this.currentPlayerIndex == 0)
        {
            this.Messenger.Publish(ActivatedView.GoBack);
        }
        else
        {
            // Previous player 
            -- this.currentPlayerIndex;
            this.currentPlayer = this.humanPlayers[this.currentPlayerIndex];
            this.UpdateUi();
        }
    }

    private void OnPlay(object? _)
    {
        if (this.currentPlayerIndex == this.humanPlayers.Count - 1 )
        {
            this.Messenger.Publish(ActivatedView.Game, this.gameOptions);
        }
        else
        {
            // Next player 
            ++this.currentPlayerIndex;
            this.currentPlayer = this.humanPlayers[this.currentPlayerIndex];
            this.UpdateUi();
        }
    }
#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public ICommand PlayCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand BackCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string? NextButtonText { get => this.Get<string?>(); set => this.Set(value); }

    public PlayerColor PlayerColor { get => this.Get<PlayerColor>(); set => this.Set(value); }
}