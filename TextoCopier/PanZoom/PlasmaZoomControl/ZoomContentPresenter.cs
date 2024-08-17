namespace Lyt.Avalonia.Zoom;

public class ContentSizeChangedEventArgs(Size size)
{
    public Size Size { get; } = size; 
}

public class ZoomContentPresenter : ContentControl
{
    public delegate void ContentSizeChangedEventHandler(object sender, ContentSizeChangedEventArgs e);

    public event ContentSizeChangedEventHandler? ContentSizeChanged;

    private Size contentSize;

    public Size ContentSize
    {
        get => this.contentSize;
        private set
        {
            if (value == this.contentSize)
            {
                return;
            }

            this.contentSize = value;
            this.ContentSizeChanged?.Invoke(this, new ContentSizeChangedEventArgs(this.contentSize));
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double max = 10_000.0;
        double x = double.IsInfinity(constraint.Width) ? max : constraint.Width;
        double y = double.IsInfinity(constraint.Height) ? max : constraint.Height;
        return new Size(x, y);
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var children = this.GetVisualChildren();
        int visualChildrenCount = children.Count();
        Control? child = visualChildrenCount > 0 ?  children.FirstOrDefault() as Control : null;
        if (child is null)
        {
            return arrangeBounds;
        }

        var grandChildren = child.GetVisualChildren();
        visualChildrenCount = grandChildren.Count();
        Control? grandChild = visualChildrenCount > 0 ? grandChildren.FirstOrDefault() as Control : null;
        if (grandChild is null)
        {
            return arrangeBounds;
        }

        // set the ContentSize
        this.ContentSize = grandChild.DesiredSize;
        grandChild.Arrange(new Rect(grandChild.DesiredSize));
        return arrangeBounds;
    }
}
