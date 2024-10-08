﻿namespace Lyt.TranslateRace.Messaging;

public sealed class ViewActivationMessage(ViewActivationMessage.ActivatedView view, object? activationParameter = null)
{
    public enum ActivatedView
    {
        Intro,
        Setup,
        NewParticipant,
        Game,
        GameOver,
        GoBack,
        Exit,
    }

    public ActivatedView View { get; private set; } = view;

    public object? ActivationParameter { get; private set; } = activationParameter;
}
