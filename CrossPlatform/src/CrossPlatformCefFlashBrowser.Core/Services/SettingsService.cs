using System.Text.Json;
using CrossPlatformCefFlashBrowser.Core.Models;

namespace CrossPlatformCefFlashBrowser.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IFileService _fileService;
        private const string SettingsFileName = "settings.json";

        public SettingsService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Settings> LoadSettingsAsync()
        {
            try
            {
                var appDataPath = await _fileService.GetAppDataPathAsync();
                var settingsPath = Path.Combine(appDataPath, SettingsFileName);

                if (!await _fileService.FileExistsAsync(settingsPath))
                {
                    return Settings.Default;
                }

                var data = await _fileService.ReadFileAsync(settingsPath);
                var json = System.Text.Encoding.UTF8.GetString(data);
                var settings = JsonSerializer.Deserialize<Settings>(json);
                
                if (settings == null)
                {
                    return Settings.Default;
                }

                settings.SetNullPropertiesToDefault();
                return settings;
            }
            catch
            {
                return Settings.Default;
            }
        }

        public async Task SaveSettingsAsync(Settings settings)
        {
            try
            {
                var appDataPath = await _fileService.GetAppDataPathAsync();
                var settingsPath = Path.Combine(appDataPath, SettingsFileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(settings, options);
                var data = System.Text.Encoding.UTF8.GetBytes(json);

                await _fileService.WriteFileAsync(settingsPath, data);
            }
            catch
            {
                // Log error in production
            }
        }
    }
}
