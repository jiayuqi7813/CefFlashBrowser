using System;
using System.IO;
using System.Threading.Tasks;
using CrossPlatformCefFlashBrowser.Core.Services;

namespace CrossPlatformCefFlashBrowser.Services.Platform
{
    public class FileService : IFileService
    {
        public async Task<string> GetAppDataPathAsync()
        {
            await Task.CompletedTask;
            
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "CrossPlatformCefFlashBrowser");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            
            return appFolder;
        }

        public async Task<byte[]> ReadFileAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        public async Task WriteFileAsync(string path, byte[] data)
        {
            await File.WriteAllBytesAsync(path, data);
        }

        public async Task<string?> PickFileAsync(string[] extensions)
        {
            await Task.CompletedTask;
            // TODO: Implement platform-specific file picker
            // For now, return null
            return null;
        }

        public async Task<string?> PickFolderAsync()
        {
            await Task.CompletedTask;
            // TODO: Implement platform-specific folder picker
            return null;
        }

        public async Task<bool> FileExistsAsync(string path)
        {
            await Task.CompletedTask;
            return File.Exists(path);
        }
    }
}
