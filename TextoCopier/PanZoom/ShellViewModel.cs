namespace PanZoom;

public class ShellViewModel : Bindable<ShellView>
{
    PanZoomControl panZoom; 

    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;

    public ShellViewModel(
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;
    }

    protected override void OnViewLoaded()
    { 
        base.OnViewLoaded();
        this.panZoom = this.View.PanAndZoom; 
    }

    private void OnZoomIn(object? _)
    {
        this.panZoom.ViewportZoom = 0.5;
    }

    private void OnZoomOut(object? _)
    {
    }

    public ICommand ZoomInCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ZoomOutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

}
