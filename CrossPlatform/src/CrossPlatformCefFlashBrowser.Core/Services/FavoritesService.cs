using System.Text.Json;
using CrossPlatformCefFlashBrowser.Core.Models;

namespace CrossPlatformCefFlashBrowser.Core.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IFileService _fileService;
        private const string FavoritesFileName = "favorites.json";

        public FavoritesService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<List<Website>> LoadFavoritesAsync()
        {
            try
            {
                var appDataPath = await _fileService.GetAppDataPathAsync();
                var favoritesPath = Path.Combine(appDataPath, FavoritesFileName);

                if (!await _fileService.FileExistsAsync(favoritesPath))
                {
                    return new List<Website>();
                }

                var data = await _fileService.ReadFileAsync(favoritesPath);
                var json = System.Text.Encoding.UTF8.GetString(data);
                var favorites = JsonSerializer.Deserialize<List<Website>>(json);

                return favorites ?? new List<Website>();
            }
            catch
            {
                return new List<Website>();
            }
        }

        public async Task SaveFavoritesAsync(List<Website> favorites)
        {
            try
            {
                var appDataPath = await _fileService.GetAppDataPathAsync();
                var favoritesPath = Path.Combine(appDataPath, FavoritesFileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(favorites, options);
                var data = System.Text.Encoding.UTF8.GetBytes(json);

                await _fileService.WriteFileAsync(favoritesPath, data);
            }
            catch
            {
                // Log error in production
            }
        }

        public async Task<bool> AddFavoriteAsync(Website website)
        {
            try
            {
                var favorites = await LoadFavoritesAsync();
                
                if (favorites.Any(f => f.Url == website.Url))
                {
                    return false;
                }

                favorites.Add(website);
                await SaveFavoritesAsync(favorites);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFavoriteAsync(Website website)
        {
            try
            {
                var favorites = await LoadFavoritesAsync();
                var removed = favorites.RemoveAll(f => f.Url == website.Url) > 0;
                
                if (removed)
                {
                    await SaveFavoritesAsync(favorites);
                }

                return removed;
            }
            catch
            {
                return false;
            }
        }
    }
}
