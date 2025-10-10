using CrossPlatformCefFlashBrowser.Core.Models;

namespace CrossPlatformCefFlashBrowser.Core.Services
{
    public interface ISettingsService
    {
        Task<Settings> LoadSettingsAsync();
        Task SaveSettingsAsync(Settings settings);
    }
}
