namespace CrossPlatformCefFlashBrowser.Services;

public interface INavigationService
{
    bool CanGoBack { get; }
    bool CanGoForward { get; }
    string? GoBack();
    string? GoForward();
    void RecordHistory(string url);
}
