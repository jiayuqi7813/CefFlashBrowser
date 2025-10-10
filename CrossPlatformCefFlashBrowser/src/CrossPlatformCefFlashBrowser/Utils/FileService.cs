using System.IO;
using Microsoft.Maui.Storage;

namespace CrossPlatformCefFlashBrowser.Utils;

public class FileService : IFileService
{
    public Task<string> GetAppDataPathAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(FileSystem.AppDataDirectory);
    }

    public Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        return File.ReadAllBytesAsync(path, cancellationToken);
    }

    public Task WriteFileAsync(string path, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return File.WriteAllBytesAsync(path, data.ToArray(), cancellationToken);
    }

    public Task<string?> PickFileAsync(string[] extensions, CancellationToken cancellationToken = default)
    {
        // Placeholder: platform-specific implementation will be provided later.
        return Task.FromResult<string?>(null);
    }
}
