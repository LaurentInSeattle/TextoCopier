namespace Lyt.TextoCopier.Utilities;

public static class WebUtilities
{
    public static bool OpenWebUrl(string webUrl, out string message)
    {
        message = string.Empty; 
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var proc = new Process { StartInfo = { UseShellExecute = true, FileName = webUrl } };
                proc.Start();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("x-www-browser", webUrl);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", webUrl);
            }
            else
            {
                throw new ArgumentException("Unsupported platform: " + RuntimeInformation.OSDescription);
            }

            return true; 
        } 
        catch ( Exception e )
        {
            message = "Failed to open provided URL, exception thrown: " + e .Message;
            return false;
        }
    }
}