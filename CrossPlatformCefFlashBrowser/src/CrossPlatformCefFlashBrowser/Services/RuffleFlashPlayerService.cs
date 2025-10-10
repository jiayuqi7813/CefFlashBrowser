using System;
using CrossPlatformCefFlashBrowser.Utils;

namespace CrossPlatformCefFlashBrowser.Services;

public class RuffleFlashPlayerService : IFlashPlayerService
{
    private readonly IWebViewBridge _webViewBridge;

    public RuffleFlashPlayerService(IWebViewBridge webViewBridge)
    {
        _webViewBridge = webViewBridge;
    }

    public Task<bool> CanPlayFlashAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(url.EndsWith(".swf", StringComparison.OrdinalIgnoreCase));
    }

    public async Task PlayLocalSwfAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var resolved = await _webViewBridge.ResolveLocalFileUrlAsync(filePath, cancellationToken).ConfigureAwait(false);
        await _webViewBridge.InjectFlashPlayerAsync(resolved, cancellationToken).ConfigureAwait(false);
    }
}
