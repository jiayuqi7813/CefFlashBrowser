using System.IO;
using System.Text.Json;
using CrossPlatformCefFlashBrowser.Models;
using CrossPlatformCefFlashBrowser.Utils;

namespace CrossPlatformCefFlashBrowser.Services;

public class SettingsService : ISettingsService
{
    private readonly IFileService _fileService;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public SettingsService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<Settings> LoadSettingsAsync(CancellationToken cancellationToken = default)
    {
        var path = await GetSettingsPathAsync(cancellationToken).ConfigureAwait(false);
        if (!File.Exists(path))
        {
            return Settings.CreateDefault();
        }

        await using var stream = File.OpenRead(path);
        var settings = await JsonSerializer.DeserializeAsync<Settings>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
        return settings ?? Settings.CreateDefault();
    }

    public async Task SaveSettingsAsync(Settings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var path = await GetSettingsPathAsync(cancellationToken).ConfigureAwait(false);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, settings, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string> GetSettingsPathAsync(CancellationToken cancellationToken)
    {
        var appData = await _fileService.GetAppDataPathAsync(cancellationToken).ConfigureAwait(false);
        return Path.Combine(appData, "settings.json");
    }
}
