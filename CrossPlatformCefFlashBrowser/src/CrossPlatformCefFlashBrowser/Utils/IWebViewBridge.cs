namespace CrossPlatformCefFlashBrowser.Utils;

public interface IWebViewBridge
{
    Task<string> ResolveLocalFileUrlAsync(string filePath, CancellationToken cancellationToken = default);
    Task InjectFlashPlayerAsync(string sourceUrl, CancellationToken cancellationToken = default);
}
