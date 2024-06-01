using System.Runtime.InteropServices;

namespace Lyt.TextoCopier.Utilities;

public static class WebUtilities
{
    public static void OpenWebUrl(string webUrl)
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
    }
}