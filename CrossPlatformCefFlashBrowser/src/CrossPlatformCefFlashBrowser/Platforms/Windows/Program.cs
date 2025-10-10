using Microsoft.UI.Xaml;

namespace CrossPlatformCefFlashBrowser;

public static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        MauiWinUIApplication.Main(args, typeof(App));
    }
}
