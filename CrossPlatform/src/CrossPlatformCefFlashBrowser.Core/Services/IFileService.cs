namespace CrossPlatformCefFlashBrowser.Core.Services
{
    public interface IFileService
    {
        Task<string> GetAppDataPathAsync();
        Task<byte[]> ReadFileAsync(string path);
        Task WriteFileAsync(string path, byte[] data);
        Task<string?> PickFileAsync(string[] extensions);
        Task<string?> PickFolderAsync();
        Task<bool> FileExistsAsync(string path);
    }
}
