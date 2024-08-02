namespace Lyt.Avalonia.PanZoom;

public partial class PanZoomControl  // Pointer events 
{
    /// <summary> The control for creating a zoom border </summary>
    private Border _partDragZoomBorder;

    /// <summary> The control for creating a zoom border --- ??? </summary>
    private Canvas _partDragZoomCanvas;

    /// <summary> Specifies the current state of the mouse handling logic. </summary>
    private MouseHandlingModeEnum _mouseHandlingMode = MouseHandlingModeEnum.None;

    /// <summary> The point that was clicked relative to the ZoomAndPanControl. </summary>
    private Point _origZoomAndPanControlMouseDownPoint;

    /// <summary> The point that was clicked relative to the content that is contained within the ZoomAndPanControl. </summary>
    private Point _origContentMouseDownPoint;

    /// <summary> Records which mouse button clicked during mouse dragging. </summary>
    private MouseButton _mouseButtonDown;

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        _content.Focus();

        PointerPoint pointerPointControl = args.GetCurrentPoint(this);
        _mouseButtonDown = pointerPointControl.MouseButtonFromPointerPoint();
        _origZoomAndPanControlMouseDownPoint = pointerPointControl.Position;

        PointerPoint pointerPointContent = args.GetCurrentPoint(this._content);
        _origContentMouseDownPoint = pointerPointContent.Position;

        if ((args.KeyModifiers & KeyModifiers.Shift) != 0 &&
            (_mouseButtonDown == MouseButton.Left || _mouseButtonDown == MouseButton.Right))
        {
            // Shift + left- or right-down initiates zooming mode.
            _mouseHandlingMode = MouseHandlingModeEnum.Zooming;
        }
        else if (_mouseButtonDown == MouseButton.Left)
        {
            // Just a plain old left-down initiates panning mode.
            _mouseHandlingMode = MouseHandlingModeEnum.Panning;
        }

        if (_mouseHandlingMode != MouseHandlingModeEnum.None)
        {
            // Capture the mouse so that we eventually receive the mouse up event.
            args.Pointer.Capture(this);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        if (_mouseHandlingMode != MouseHandlingModeEnum.None)
        {
            if (_mouseHandlingMode == MouseHandlingModeEnum.Zooming)
            {
                if (_mouseButtonDown == MouseButton.Left)
                {
                    // Shift + left-click zooms IN on the content.
                    ZoomIn(_origContentMouseDownPoint);
                }
                else if (_mouseButtonDown == MouseButton.Right)
                {
                    // Shift + left-click zooms OUT from the content.
                    ZoomOut(_origContentMouseDownPoint);
                }
            }
            else if (_mouseHandlingMode == MouseHandlingModeEnum.DragZooming)
            {
                PointerPoint pointerPointContent = args.GetCurrentPoint(this._content);
                Point finalContentMousePoint = pointerPointContent.Position;
                
                // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                ApplyDragZoomRect(finalContentMousePoint);
            }

            args.Pointer.Capture(null);
            _mouseHandlingMode = MouseHandlingModeEnum.None;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs args)
    {
        var oldContentMousePoint = MousePosition;
        PointerPoint pointerPointContent = args.GetCurrentPoint(this._content);
        Point curContentMousePoint = pointerPointContent.Position;

        Rect bounds = this._content.Bounds;
        MousePosition = curContentMousePoint.FilterClamp(bounds.Width - 1.0, bounds.Height - 1.0);

        // TODO ??? 
        //
        //OnPropertyChanged(new DependencyPropertyChangedEventArgs(MousePositionProperty, oldContentMousePoint,
        //    curContentMousePoint));

        if (_mouseHandlingMode == MouseHandlingModeEnum.Panning)
        {
            // The user is left-dragging the mouse:  Pan the viewport by the appropriate amount.
            var dragOffset = curContentMousePoint - _origContentMouseDownPoint;
            this.ContentOffsetX -= dragOffset.X;
            this.ContentOffsetY -= dragOffset.Y;

            args.Handled = true;
        }
        else if (_mouseHandlingMode == MouseHandlingModeEnum.Zooming)
        {
            PointerPoint pointerPointControl = args.GetCurrentPoint(this);
            Point curZoomAndPanControlMousePoint = pointerPointControl.Position;

            var dragOffset = curZoomAndPanControlMousePoint - _origZoomAndPanControlMouseDownPoint;
            double dragThreshold = 10;
            if (_mouseButtonDown == MouseButton.Left && 
                (Math.Abs(dragOffset.X) > dragThreshold || Math.Abs(dragOffset.Y) > dragThreshold))
            {
                // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                // initiate drag zooming mode where the user can drag out a rectangle to select the area to zoom in on.
                _mouseHandlingMode = MouseHandlingModeEnum.DragZooming;
                InitDragZoomRect(_origContentMouseDownPoint, curContentMousePoint);
            }
        }
        else if (_mouseHandlingMode == MouseHandlingModeEnum.DragZooming)
        {
            // When in drag zooming mode continously update the position of the rectangle that the user is dragging out.
            
            // TODO - Needed ? Same as code at the top ?? 
            PointerPoint pointerPointControl = args.GetCurrentPoint(this);
            curContentMousePoint = pointerPointControl.Position;

            SetDragZoomRect(_origZoomAndPanControlMouseDownPoint, curContentMousePoint);
        }
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs args)
    {
        args.Handled = true;
        PointerPoint pointerPointContent = args.GetCurrentPoint(this._content);
        Point curContentMousePoint = pointerPointContent.Position;
        double delta = args.Delta.Y;
        if (delta > 0.000_1)
        {
            ZoomIn(curContentMousePoint);
        }
        else if (delta < -0.000_1)
        {
            ZoomOut(curContentMousePoint);
        } 
        // else ~= 0.0: do nothing 
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs args)
    {
        Point curContentMousePoint = args.GetPosition(this._content);
        if ((args.KeyModifiers & KeyModifiers.Shift) == 0)
        {
            this.AnimatedSnapTo(curContentMousePoint);
            args.Handled = true;
        }
    }

    /// <summary> Zoom the viewport out, centering on the specified point (in content coordinates). </summary>
    private void ZoomOut(Point contentZoomCenter)
        => this.ZoomAboutPoint(this.InternalViewportZoom * 0.90909090909, contentZoomCenter);

    /// <summary> Zoom the viewport in, centering on the specified point (in content coordinates). </summary>
    private void ZoomIn(Point contentZoomCenter)
        => this.ZoomAboutPoint(this.InternalViewportZoom * 1.1, contentZoomCenter);


    /// <summary> Initialise the rectangle that the use is dragging out. </summary>
    private void InitDragZoomRect(Point pt1, Point pt2)
    {
        _partDragZoomCanvas.IsVisible = true;
        _partDragZoomBorder.Opacity = 1;
        SetDragZoomRect(pt1, pt2);
    }

    /// <summary> Update the position and size of the rectangle that user is dragging out. </summary>
    private void SetDragZoomRect(Point pt1, Point pt2)
    {
        // Update the coordinates of the rectangle that is being dragged out by the user.
        // ???? offset and rescale to convert from content coordinates.
        Rect bounds = _partDragZoomCanvas.Bounds;
        var rect = ViewportHelpers.Clip(pt1, pt2, new Point(0, 0), new Point(bounds.Width, bounds.Height));
        ViewportHelpers.PositionBorderOnCanvas(_partDragZoomBorder, rect);
    }

    /// <summary> When the user has finished dragging out the rectangle the zoom operation is applied. </summary>
    private void ApplyDragZoomRect(Point finalContentMousePoint)
    {
        Rect bounds = _partDragZoomCanvas.Bounds;
        var rect = 
            ViewportHelpers.Clip(
                finalContentMousePoint, _origContentMouseDownPoint, 
                new Point(0, 0), new Point(bounds.Width, bounds.Height));
        this.AnimatedZoomTo(rect);
        FadeOutDragZoomRect();
    }

    // Fade out the drag zoom rectangle.
    private void FadeOutDragZoomRect()
    {
        AnimationHelper.StartAnimation(_partDragZoomBorder, OpacityProperty, 0.0, 0.1,
            delegate { _partDragZoomCanvas.IsVisible = false; }, UseAnimations);
    }

    #region Commands -- Commented out 

    ///// <summary>
    /////     Command to implement the zoom to fill 
    ///// </summary>
    //public ICommand FillCommand => _fillCommand ?? (_fillCommand = new RelayCommand(() =>
    //{
    //    SaveZoom();
    //    AnimatedZoomToCentered(FillZoomValue);
    //    RaiseCanExecuteChanged();
    //}, () => !InternalViewportZoom.IsWithinOnePercent(FillZoomValue) && FillZoomValue >= MinimumZoomClamped));

    //private RelayCommand _fillCommand;

    ///// <summary>
    /////     Command to implement the zoom to fit 
    ///// </summary>
    //public ICommand FitCommand => _fitCommand ?? (_fitCommand = new RelayCommand(() =>
    //{
    //    SaveZoom();
    //    AnimatedZoomTo(FitZoomValue);
    //    RaiseCanExecuteChanged();
    //}, () => !InternalViewportZoom.IsWithinOnePercent(FitZoomValue) && FitZoomValue >= MinimumZoomClamped));

    //private RelayCommand _fitCommand;

    ///// <summary>
    /////     Command to implement the zoom to a percentage where 100 (100%) is the default and 
    /////     shows the image at a zoom where 1 pixel is 1 pixel. Other percentages specified
    /////     with the command parameter. 50 (i.e. 50%) would display 4 times as much of the image
    ///// </summary>
    //public ICommand ZoomPercentCommand
    //    => _zoomPercentCommand ?? (_zoomPercentCommand = new RelayCommand<double>(value =>
    //    {
    //        SaveZoom();
    //        var adjustedValue = value == 0 ? 1 : value / 100;
    //        AnimatedZoomTo(adjustedValue);
    //        RaiseCanExecuteChanged();
    //    }, value =>
    //    {
    //        var adjustedValue = value == 0 ? 1 : value / 100;
    //        return !InternalViewportZoom.IsWithinOnePercent(adjustedValue) && adjustedValue >= MinimumZoomClamped;
    //    }));


    //// Math.Abs(InternalViewportZoom - ((value == 0) ? 1.0 : value / 100)) > .01 * InternalViewportZoom 

    //private RelayCommand<double> _zoomPercentCommand;

    ///// <summary>
    /////     Command to implement the zoom ratio where 1 is is the the specified minimum. 2 make the image twices the size,
    /////     and is the default. Other values are specified with the CommandParameter. 
    ///// </summary>
    //public ICommand ZoomRatioFromMinimumCommand
    //    => _zoomRatioFromMinimumCommand ?? (_zoomRatioFromMinimumCommand = new RelayCommand<double>(value =>
    //    {
    //        SaveZoom();
    //        var adjustedValue = (value == 0 ? 2 : value) * MinimumZoomClamped;
    //        AnimatedZoomTo(adjustedValue);
    //        RaiseCanExecuteChanged();
    //    }, value =>
    //    {
    //        var adjustedValue = (value == 0 ? 2 : value) * MinimumZoomClamped;
    //        return !InternalViewportZoom.IsWithinOnePercent(adjustedValue) && adjustedValue >= MinimumZoomClamped;
    //    }));

    //private RelayCommand<double> _zoomRatioFromMinimumCommand;


    ///// <summary>
    /////     Command to implement the zoom out by 110% 
    ///// </summary>
    //public ICommand ZoomOutCommand => _zoomOutCommand ?? (_zoomOutCommand = new RelayCommand(() =>
    //{
    //    DelayedSaveZoom1500Miliseconds();
    //    ZoomOut(new Point(ContentZoomFocusX, ContentZoomFocusY));
    //}, () => InternalViewportZoom > MinimumZoomClamped));
    //private RelayCommand _zoomOutCommand;

    ///// <summary>
    /////     Command to implement the zoom in by 91% 
    ///// </summary>
    //public ICommand ZoomInCommand => _zoomInCommand ?? (_zoomInCommand = new RelayCommand(() =>
    //{
    //    DelayedSaveZoom1500Miliseconds();
    //    ZoomIn(new Point(ContentZoomFocusX, ContentZoomFocusY));
    //}, () => InternalViewportZoom < MaximumZoom));
    //private RelayCommand _zoomInCommand;

    //private void RaiseCanExecuteChanged()
    //{
    //    _zoomPercentCommand?.RaiseCanExecuteChanged();
    //    _zoomOutCommand?.RaiseCanExecuteChanged();
    //    _zoomInCommand?.RaiseCanExecuteChanged();
    //    _fitCommand?.RaiseCanExecuteChanged();
    //    _fillCommand?.RaiseCanExecuteChanged();
    //}

    #endregion
}
