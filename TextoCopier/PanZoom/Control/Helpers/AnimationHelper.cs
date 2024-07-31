namespace Lyt.Avalonia.PanZoom;

/// <summary> A helper class to simplify animations. </summary>
public static class AnimationHelper
{
    /// <summary>
    /// Starts an animation to a particular value on the specified dependency property.
    /// You can pass in an event handler to call when the animation has completed.
    /// </summary>
    public static void StartAnimation(
        Control animatableElement, AvaloniaProperty dependencyProperty, 
        double toValue, double animationDurationSeconds, 
        EventHandler? completedEvent = null , bool useAnimations = false)
    {
        //new Animation()
        //{
        //    Duration = TimeSpan.FromSeconds(5),
        //    IterationCount = IterationCount.Infinite,
        //    PlaybackDirection = PlaybackDirection.Alternate, // Auto-reverse
        //    Children =
        //        {
        //            new KeyFrame
        //            {
        //                Cue = default,
        //                Setters =
        //                {
        //                    new Setter(Button.WidthProperty, 75.0)
        //                }
        //            },
        //            new KeyFrame
        //            {
        //                Cue = new Cue(1),
        //                Setters =
        //                {
        //                    new Setter(Button.WidthProperty, 300.0)
        //                }
        //            }
        //        }
        //}.RunAsync(aButton, cancellationToken);

        if (useAnimations)
        {
            //var fromValue = (double)animatableElement.GetValue(dependencyProperty);

            //var animation = new DoubleAnimation
            //{
            //    From = fromValue,
            //    To = toValue,
            //    Duration = TimeSpan.FromSeconds(animationDurationSeconds)
            //};

            //animation.Completed += delegate (object sender, EventArgs e)
            //{
            //    // When the animation has completed bake final value of the animation into the property.
            //    animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));
            //    CancelAnimation(animatableElement, dependencyProperty);
            //    completedEvent?.Invoke(sender, e);
            //};
            //animation.Freeze();
            //animatableElement.BeginAnimation(dependencyProperty, animation);
        }
        else
        {
            animatableElement.SetValue(dependencyProperty, toValue);
            completedEvent?.Invoke(null, new EventArgs());
        }
    }

    /// <summary> Cancel any animations that are running on the specified property. </summary>
    public static void CancelAnimation(Control animatableElement, AvaloniaProperty dependencyProperty)
    {

        //if (useAnimations)
        //{
        //    // animatableElement.BeginAnimation(dependencyProperty, null);
        //} 
    }
}
