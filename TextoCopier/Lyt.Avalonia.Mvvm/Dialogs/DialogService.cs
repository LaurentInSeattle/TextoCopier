namespace Lyt.Avalonia.Mvvm.Dialogs;

public sealed class DialogService(ILogger logger) : IDialogService
{
    private readonly ILogger logger = logger;

    #region LATER: Run a Modal Window 

    //public static bool Run<TDialog, TDialogParameters>(Window window, TDialogParameters? dialogParameters = null)
    //    where TDialog : UserControl, IDialog<TDialogParameters>, new()
    //    where TDialogParameters : class
    //{
    //    var host = new ModalHostWindow()
    //    {
    //        Owner = window,
    //        Width = window.ActualWidth,
    //        Height = window.ActualHeight,
    //    };

    //    TDialog modal = new TDialog { Host = host };
    //    if (dialogParameters is not null)
    //    {
    //        modal.Initialize(dialogParameters);
    //        PropertyInfo? imageSourceProperty = dialogParameters.GetType().GetProperty("ImageSource");
    //        if (imageSourceProperty is not null)
    //        {
    //            object? imageSourcePropertyValue = imageSourceProperty.GetValue(dialogParameters, null);
    //            if (imageSourcePropertyValue is string imageSourceUri)
    //            {
    //                if (!string.IsNullOrEmpty(imageSourceUri))
    //                {
    //                    try
    //                    {
    //                        BitmapImage source = new BitmapImage();
    //                        source.BeginInit();
    //                        source.UriSource = new Uri("pack://application:,,," + imageSourceUri);
    //                        source.EndInit();
    //                        host.BackgroundImage.Source = source;
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        // Silently swallow the exception if the image fails to load
    //                        Debug.WriteLine(ex);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    host.ContentGrid.Children.Add(modal);
    //    bool? _ = host.ShowDialog();
    //    return modal.DialogResult;
    //}

    #endregion Run a Window 

    public void Run<TDialog, TDialogParameters>(
        object maybePanel, Action<bool> onClose, TDialogParameters dialogParameters)
        where TDialog : IDialog<TDialogParameters>, new()
        where TDialogParameters : class
    {
        try
        {
            if (maybePanel is not Panel panel)
            {
                this.logger.Error("Must provide a host panel");
                throw new InvalidOperationException("Must provide a host panel");
            }

            if (!typeof(TDialog).IsSubclassOf(typeof(UserControl)))
            {
                this.logger.Error("TDialog Must provide a type deriving from UserControl");
                throw new InvalidOperationException("TDialog Must provide a type deriving from UserControl");
            }

            this.RunInternal<TDialog, TDialogParameters>(panel, onClose, dialogParameters);
        }
        catch (Exception ex)
        {
            this.logger.Error("Failed to launch dialog, exception thrown: \n" + ex.ToString());
            throw;
        }
    }

    private void RunInternal<TDialog, TDialogParameters>(
        Panel panel, Action<bool> onClose, TDialogParameters? dialogParameters = null)
        where TDialog : IDialog<TDialogParameters>, new()
        where TDialogParameters : class
    {
        var host = new ModalHostControl(panel, onClose);
        var modal = new TDialog { Host = host };
        if (modal is not UserControl userControl)
        {
            this.logger.Error("Failed to cast to UserControl");
            throw new InvalidOperationException("Failed to cast to UserControl");
        }

        if (dialogParameters is not null)
        {
            modal.Initialize(dialogParameters);
        }

        panel.Children.Add(host);
        host.ContentGrid.Children.Add(userControl);
    }
}
