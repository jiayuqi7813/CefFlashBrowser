using CrossPlatformCefFlashBrowser.Views;

namespace CrossPlatformCefFlashBrowser;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        InitializeComponent();

        MainPage = new NavigationPage(mainPage);
    }
}
