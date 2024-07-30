namespace ISC.Melody.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System;

    public interface IContentSizeProvider
    {
        double OffsetX { get; }
        double OffsetY { get; }
        double Width { get; }
        double Height { get; }
        double ViewportWidth { get; }
        double ViewportHeight { get; }
    }

    /// <summary>
    /// Interaction logic for ZoomAndPanOverview.xaml
    /// </summary>
    public partial class ZoomAndPanOverview : UserControl
    {
        private UIElement overviewedContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomAndPanOverview"/> class.
        /// </summary>
        public ZoomAndPanOverview()
        {
            InitializeComponent();
        }

        public UIElement OverviewedContent
        {
            get { return overviewedContent; }

            set
            {
                if (value != overviewedContent)
                {
                    if (null != overviewedContent)
                    {
                        Grid.Children.Remove(overviewedContent);
                    }

                    overviewedContent = value;
                    overviewedContent.SetValue(Panel.ZIndexProperty, 1); 
                    Grid.Children.Add(overviewedContent);
                    InvalidateVisual();
                }
            }
        }

        /// <summary>
        /// Event raised when the size of the ZoomAndPanControl changes.
        /// </summary>
        private void OnOverviewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update the scale so that the entire content fits in the window.
            Overview.ScaleToFit();
        }

        /// <summary>
        /// Event raised when the user drags the overview zoom rect.
        /// </summary>
        private void OnOverviewZoomRectThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            IContentSizeProvider content = DataContext as IContentSizeProvider;
            if (null == content)
            {
                return; 
            }

            // Update the position of the overview rect as the user drags it around.
            double newContentOffsetX = Math.Min(Math.Max(0.0, Canvas.GetLeft(overviewZoomRectThumb) + e.HorizontalChange), content.Width - content.ViewportWidth);
            Canvas.SetLeft(overviewZoomRectThumb, newContentOffsetX);
            double newContentOffsetY = Math.Min(Math.Max(0.0, Canvas.GetTop(overviewZoomRectThumb) + e.VerticalChange), content.Height - content.ViewportHeight);
            Canvas.SetTop(overviewZoomRectThumb, newContentOffsetY);
        }

        /// <summary>
        /// Event raised on mouse down.
        /// </summary>
        private void OnOverviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Update the scale so that the entire content fits in the window.
            Overview.ScaleToFit();
            
            // Update the position of the overview rect to the point that was clicked.
            Point clickedPoint = e.GetPosition(Grid); // content 
            double newX = clickedPoint.X - (overviewZoomRectThumb.Width / 2);
            double newY = clickedPoint.Y - (overviewZoomRectThumb.Height / 2);
            Canvas.SetLeft(overviewZoomRectThumb, newX);
            Canvas.SetTop(overviewZoomRectThumb, newY);
        }
    }
}
