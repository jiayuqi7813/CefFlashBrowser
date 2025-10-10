using System;

namespace CrossPlatformCefFlashBrowser.Utils;

public class WebViewBridge : IWebViewBridge
{
    public Task<string> ResolveLocalFileUrlAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(filePath);
        return Task.FromResult($"app://local/{Uri.EscapeDataString(fileName)}");
    }

    public Task InjectFlashPlayerAsync(string sourceUrl, CancellationToken cancellationToken = default)
    {
        // Placeholder for Ruffle integration. Platform specific implementations will override this behaviour.
        return Task.CompletedTask;
    }
}
