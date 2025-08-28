namespace Lyt.TranslateRace.Messaging;

public static class AppMessagingExtensions
{
    private static readonly IDialogService dialogService;

    static AppMessagingExtensions() 
        => AppMessagingExtensions.dialogService = App.GetRequiredService<IDialogService>();

    public static void Select(ActivatedView activatedView, object? parameter = null)
        => ViewSelector<ActivatedView>.Select(activatedView, parameter);

    public static void Exit () => ShellViewModel.OnExit (null) ;
}
