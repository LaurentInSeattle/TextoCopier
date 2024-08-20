namespace Lyt.Invasion.Behaviors;

public class AppearsOnMouseOverBehavior : Behavior<Control>
{
    private IAnimationService? animationService;
    private bool leaving;

    protected override void OnAttached()
    {
        if (this.AssociatedObject is null)
        {
            return;
        }

        this.animationService = App.GetRequiredService<IAnimationService>();
        this.AssociatedObject.PointerEntered += this.OnPointerEnter;
        this.AssociatedObject.PointerExited += this.OnPointerLeave;
    }

    protected override void OnDetaching()
    {
        if (this.AssociatedObject is null)
        {
            return;
        }

        this.AssociatedObject.PointerEntered -= this.OnPointerEnter;
        this.AssociatedObject.PointerExited -= this.OnPointerLeave;
    }

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        this.leaving = false;
        if ((this.AssociatedObject is null) || (this.animationService is null))
        {
            return;
        }

        if (this.AssociatedObject.Opacity < 1.0)
        {
            this.animationService.FadeIn(this.AssociatedObject, 0.3);
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        if ((this.AssociatedObject is null) || (this.animationService is null))
        {
            return;
        }

        this.leaving = true;
        Schedule.OnUiThread(5000, () =>
        {
            if ((this.AssociatedObject is null) || (this.animationService is null))
            {
                return;
            }

            if (this.leaving && (this.AssociatedObject.Opacity == 1.0))
            {
                this.animationService.FadeOut(this.AssociatedObject, 0.3);
            }
        }, DispatcherPriority.Background);
    }
}
