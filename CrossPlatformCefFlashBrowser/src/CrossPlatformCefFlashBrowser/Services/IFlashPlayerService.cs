namespace CrossPlatformCefFlashBrowser.Services;

public interface IFlashPlayerService
{
    Task<bool> CanPlayFlashAsync(string url, CancellationToken cancellationToken = default);
    Task PlayLocalSwfAsync(string filePath, CancellationToken cancellationToken = default);
}
