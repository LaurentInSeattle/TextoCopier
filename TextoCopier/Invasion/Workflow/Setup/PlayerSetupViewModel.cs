namespace Lyt.Invasion.Workflow.Setup;

using System.Collections.Generic;
using static ViewActivationMessage;

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

    private bool playerNameIsValid;
    private bool playerAvatarIsValid;
    private bool playerSkillSetIsValid;
    private List<PlayerColor> availablePlayerColors;

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
        this.availablePlayerColors = [.. Enum.GetValues<PlayerColor>()];
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

        this.playerNameIsValid = false;
        this.playerAvatarIsValid = true; // FOR NOW 
        this.playerSkillSetIsValid = true; // FOR NOW 

        this.InitializeUi();
    }

    private void InitializeUi()
    {
        if (this.ValidatePlayerName(this.currentPlayer.Name, out string validationMessage))
        {
            this.PlayerName = this.currentPlayer.Name.Trim();
            this.PlayerNameValidationMessage = string.Empty;
        }
        else
        {
            this.PlayerName = string.Empty;
            this.PlayerNameValidationMessage = validationMessage;
        }

        if (Enum.TryParse<PlayerColor>(this.currentPlayer.Color, out PlayerColor playerColor))
        {
            if (!this.availablePlayerColors.Contains(playerColor))
            {
                this.availablePlayerColors.Add(playerColor);
            }
        }
        else
        {
            playerColor = this.availablePlayerColors[0];
        }

        // Needs to be done AFTER the color is restored as available 
        this.IsCrimsonEnabled = this.availablePlayerColors.Contains(PlayerColor.Crimson);
        this.IsHotPinkEnabled = this.availablePlayerColors.Contains(PlayerColor.HotPink);
        this.IsDarkTurquoiseEnabled = this.availablePlayerColors.Contains(PlayerColor.DarkTurquoise);
        this.IsDarkOrangeEnabled = this.availablePlayerColors.Contains(PlayerColor.DarkOrange);

        this.PlayerColor = playerColor; 

        this.UpdateButton();
        this.IsValid = this.playerNameIsValid && this.playerAvatarIsValid && this.playerSkillSetIsValid;
    }

    private void UpdateButton()
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

    private void UpdateUi()
    {
        this.UpdateButton();
        this.IsValid = this.playerNameIsValid && this.playerSkillSetIsValid;
    }


    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    }

    private bool ValidatePlayerName(string? playerName, out string validationMessage)
    {
        this.playerNameIsValid = false;
        validationMessage = string.Empty;
        if (string.IsNullOrEmpty(playerName))
        {
            validationMessage = "Cannot be left empty.";
            return false;
        }

        playerName = playerName.Trim();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            validationMessage = "Cannot be left empty.";
            return false;
        }

        if (playerName.Length < 3)
        {
            validationMessage = "Too short.";
            return false;
        }

        if (playerName.Length > 32)
        {
            validationMessage = "Too long.";
            return false;
        }

        this.playerNameIsValid = true;
        return true;
    }

    private void CreateAiPlayers()
    {
        int playerRank = 1; 
        foreach (PlayerInfo playerInfo in this.gameOptions.Players)
        {
            if (playerInfo.IsHuman)
            {
                continue;
            }

            // LATER 
            // More fields: Avatar, Empire, Skills 
            playerInfo.Name = string.Format("Computer_{0}", playerRank);
            playerInfo.Color = this.availablePlayerColors[0].ToString();
            ++ playerRank;
            this.availablePlayerColors.RemoveAt(0);
        }
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnPlayerNameChanged(string? _, string playerName)
    {
        if (this.ValidatePlayerName(playerName, out string validationMessage))
        {
            this.currentPlayer.Name = playerName.Trim();
        }

        this.PlayerNameValidationMessage = validationMessage;
        this.UpdateUi();
    }

    private void OnPlayerColorChanged(PlayerColor _, PlayerColor playerColor)
    {
        this.currentPlayer.Color = playerColor.ToString();
    }

    private void OnBack(object? _)
    {
        if (this.currentPlayerIndex == 0)
        {
            // Restore all colours and go back 
            this.availablePlayerColors = [.. Enum.GetValues<PlayerColor>()];
            this.Messenger.Publish(ActivatedView.GoBack);
        }
        else
        {
            // Previous player 
            --this.currentPlayerIndex;
            this.currentPlayer = this.humanPlayers[this.currentPlayerIndex];
            this.InitializeUi();
        }
    }

    private void OnPlay(object? _)
    {
        if (this.currentPlayerIndex == this.humanPlayers.Count - 1)
        {
            // Finish up creating players, and restore all colours in case we play a second game
            this.CreateAiPlayers();
            this.availablePlayerColors = [.. Enum.GetValues<PlayerColor>()];

            this.Messenger.Publish(ActivatedView.Game, this.gameOptions);
        }
        else
        {
            // Next player 
            this.availablePlayerColors.Remove(Enum.Parse<PlayerColor>(this.currentPlayer.Color));
            ++this.currentPlayerIndex;
            this.currentPlayer = this.humanPlayers[this.currentPlayerIndex];
            this.InitializeUi();
        }
    }
#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public ICommand PlayCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand BackCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string? NextButtonText { get => this.Get<string?>(); set => this.Set(value); }

    public PlayerColor PlayerColor { get => this.Get<PlayerColor>(); set => this.Set(value); }

    public bool IsHotPinkEnabled { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsCrimsonEnabled { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsDarkTurquoiseEnabled { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsDarkOrangeEnabled { get => this.Get<bool>(); set => this.Set(value); }

    public string? PlayerName { get => this.Get<string?>(); set => this.Set(value); }

    public string? PlayerNameValidationMessage { get => this.Get<string?>(); set => this.Set(value); }

    public bool IsValid { get => this.Get<bool>(); set => this.Set(value); }
}
