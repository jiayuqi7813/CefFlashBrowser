using CrossPlatformCefFlashBrowser.Core.Models;

namespace CrossPlatformCefFlashBrowser.Core.Services
{
    public interface IFavoritesService
    {
        Task<List<Website>> LoadFavoritesAsync();
        Task SaveFavoritesAsync(List<Website> favorites);
        Task<bool> AddFavoriteAsync(Website website);
        Task<bool> RemoveFavoriteAsync(Website website);
    }
}
