namespace Lyt.Avalonia.Mvvm.Animations;

public sealed class AnimationService : IAnimationService
{
#pragma warning disable CA2211 // Non-constant fields should not be visible
    // Cannot be made const to use as a default parameter.
    public static IterationCount OneIteration = IterationCount.Parse("1");
#pragma warning restore CA2211 

    public void FadeIn(Control control, double durationSeconds = 1.5, Action? onAnimationCompleted = null)
        => this.StartAnimation(
            control, Control.OpacityProperty, 1.0, durationSeconds, 
            OneIteration, PlaybackDirection.Normal, FillMode.Forward, onAnimationCompleted);

    public void FadeOut(Control control, double durationSeconds = 1.5, Action? onAnimationCompleted = null)
        => this.StartAnimation(control, Control.OpacityProperty, 0.0, durationSeconds,
            OneIteration, PlaybackDirection.Normal, FillMode.Forward, onAnimationCompleted);

    public void CancelAnimation(CancellationTokenSource cancellationTokenSource)
        => cancellationTokenSource.Cancel();

    public CancellationTokenSource StartAnimation(
        Control control,
        AvaloniaProperty avaloniaProperty,
        double toValue,
        double animationDurationSeconds,
        IterationCount iterationCount,
        PlaybackDirection playbackDirection = PlaybackDirection.Normal,
        FillMode fillMode = FillMode.Forward,
        Action? onAnimationCompleted = null)
    {
        object? propertyValue = control.GetValue(avaloniaProperty);
        if (propertyValue is not double fromValue)
        {
            throw new NotSupportedException("Animate only double properties");
        }

        CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Animation animation = new()
        {
            Duration = TimeSpan.FromSeconds(animationDurationSeconds),
            IterationCount = iterationCount,
            PlaybackDirection = playbackDirection,
            FillMode = fillMode,
            Children =
                {
                    new KeyFrame
                    {
                        Cue= new Cue(0.0),
                        Setters = { new Setter(avaloniaProperty, fromValue) }
                    },
                    new KeyFrame
                    {
                        Cue= new Cue(1.0),
                        Setters = { new Setter(avaloniaProperty, toValue) }
                    }
                }
        };

        var animationTask = animation.RunAsync(control, cancellationToken);
        animationTask.ContinueWith(task =>
        {
            if (task.IsCompleted && onAnimationCompleted is not null)
            {
                // Invoke completion delegate if present,
                // Important => Most likely, this is NOT running on the UI thread, so: dispatch 
                Dispatch.OnUiThread(() => { onAnimationCompleted.Invoke(); }, DispatcherPriority.Normal);
            }

            // Do not leak the cancellation Token Source, always dispose it.
            cancellationTokenSource.Dispose();
        });

        return cancellationTokenSource;
    }
}
