using CrossPlatformCefFlashBrowser.Models;

namespace CrossPlatformCefFlashBrowser.Services;

public interface IFavoritesService
{
    Task<IReadOnlyList<FavoriteEntry>> LoadFavoritesAsync(CancellationToken cancellationToken = default);
    Task SaveFavoritesAsync(IEnumerable<FavoriteEntry> favorites, CancellationToken cancellationToken = default);
}
