namespace CrossPlatformCefFlashBrowser.Utils;

public interface IFileService
{
    Task<string> GetAppDataPathAsync(CancellationToken cancellationToken = default);
    Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default);
    Task WriteFileAsync(string path, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
    Task<string?> PickFileAsync(string[] extensions, CancellationToken cancellationToken = default);
}
