using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CrossPlatformCefFlashBrowser.Models;
using CrossPlatformCefFlashBrowser.Utils;

namespace CrossPlatformCefFlashBrowser.Services;

public class FavoritesService : IFavoritesService
{
    private readonly IFileService _fileService;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public FavoritesService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<IReadOnlyList<FavoriteEntry>> LoadFavoritesAsync(CancellationToken cancellationToken = default)
    {
        var path = await GetFavoritesPathAsync(cancellationToken).ConfigureAwait(false);
        if (!File.Exists(path))
        {
            return Array.Empty<FavoriteEntry>();
        }

        await using var stream = File.OpenRead(path);
        var favorites = await JsonSerializer.DeserializeAsync<List<FavoriteEntry>>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
        return favorites ?? Array.Empty<FavoriteEntry>();
    }

    public async Task SaveFavoritesAsync(IEnumerable<FavoriteEntry> favorites, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(favorites);

        var path = await GetFavoritesPathAsync(cancellationToken).ConfigureAwait(false);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, favorites, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string> GetFavoritesPathAsync(CancellationToken cancellationToken)
    {
        var appData = await _fileService.GetAppDataPathAsync(cancellationToken).ConfigureAwait(false);
        return Path.Combine(appData, "favorites.json");
    }
}
