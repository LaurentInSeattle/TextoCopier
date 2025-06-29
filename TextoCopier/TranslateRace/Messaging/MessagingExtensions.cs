namespace Lyt.TranslateRace.Messaging;

public static class MessagingExtensions
{
    private static readonly IMessenger messenger;
    private static readonly IDialogService dialogService;

    static MessagingExtensions()
    {
        MessagingExtensions.messenger = App.GetRequiredService<IMessenger>();
        MessagingExtensions.dialogService = App.GetRequiredService<IDialogService>();
    }

    public static void Select(ActivatedView activatedView, object? parameter = null)
        => ViewSelector<ActivatedView>.Select(
            MessagingExtensions.messenger, activatedView, parameter);

    public static void Exit () => ShellViewModel.OnExit (null) ;
}
