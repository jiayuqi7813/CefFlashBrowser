using CrossPlatformCefFlashBrowser.Models;

namespace CrossPlatformCefFlashBrowser.Services;

public interface ISettingsService
{
    Task<Settings> LoadSettingsAsync(CancellationToken cancellationToken = default);
    Task SaveSettingsAsync(Settings settings, CancellationToken cancellationToken = default);
}
