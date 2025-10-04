using System;
using System.IO;

namespace CefFlashBrowser.Utils
{
    /// <summary>
    /// Platform-specific initialization and configuration
    /// </summary>
    public static class PlatformInitializer
    {
        /// <summary>
        /// Initialize platform-specific library paths
        /// </summary>
        public static bool InitializeLibraryPaths(string libraryPath)
        {
            if (string.IsNullOrEmpty(libraryPath))
                return false;

            if (!Directory.Exists(libraryPath))
            {
                // Try to create the directory if it doesn't exist
                try
                {
                    Directory.CreateDirectory(libraryPath);
                }
                catch
                {
                    return false;
                }
            }

            switch (PlatformHelper.CurrentPlatform)
            {
                case PlatformHelper.PlatformType.Windows:
                    return Win32.SetDllDirectorySafe(libraryPath);

                case PlatformHelper.PlatformType.MacOS:
                    return MacOS.SetLibraryPath(libraryPath);

                case PlatformHelper.PlatformType.Linux:
                    // On Linux, we can set LD_LIBRARY_PATH
                    try
                    {
                        var currentPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
                        var newPath = string.IsNullOrEmpty(currentPath) ? libraryPath : $"{libraryPath}:{currentPath}";
                        Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", newPath);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }

                default:
                    return false;
            }
        }

        /// <summary>
        /// Get platform-specific subdirectory name for binaries
        /// </summary>
        public static string GetPlatformBinaryDirectory()
        {
            switch (PlatformHelper.CurrentPlatform)
            {
                case PlatformHelper.PlatformType.Windows:
                    return Environment.Is64BitProcess ? "win-x64" : "win-x86";

                case PlatformHelper.PlatformType.MacOS:
                    return PlatformHelper.IsARM64 ? "osx-arm64" : "osx-x64";

                case PlatformHelper.PlatformType.Linux:
                    return Environment.Is64BitProcess ? "linux-x64" : "linux-x86";

                default:
                    return "unknown";
            }
        }

        /// <summary>
        /// Get the platform-specific Flash player plugin path
        /// </summary>
        public static string GetFlashPluginPath(string basePluginsPath)
        {
            if (string.IsNullOrEmpty(basePluginsPath))
                return string.Empty;

            string pluginName;
            switch (PlatformHelper.CurrentPlatform)
            {
                case PlatformHelper.PlatformType.Windows:
                    pluginName = "pepflashplayer.dll";
                    break;

                case PlatformHelper.PlatformType.MacOS:
                    // On macOS, Flash plugin is typically a .plugin bundle
                    // For CEF, it might be looking for PepperFlashPlayer.plugin or a .dylib
                    pluginName = "PepperFlashPlayer.plugin";
                    var pluginPath = Path.Combine(basePluginsPath, pluginName);
                    if (Directory.Exists(pluginPath))
                        return pluginPath;
                    // Fallback to .dylib if .plugin doesn't exist
                    pluginName = "libpepflashplayer.dylib";
                    break;

                case PlatformHelper.PlatformType.Linux:
                    pluginName = "libpepflashplayer.so";
                    break;

                default:
                    return string.Empty;
            }

            return Path.Combine(basePluginsPath, pluginName);
        }

        /// <summary>
        /// Check if Flash is supported on the current platform
        /// </summary>
        public static bool IsFlashSupported()
        {
            // Flash support availability per platform
            // Note: Flash is officially discontinued, but legacy versions may work
            switch (PlatformHelper.CurrentPlatform)
            {
                case PlatformHelper.PlatformType.Windows:
                    return true; // Flash was widely available on Windows

                case PlatformHelper.PlatformType.MacOS:
                    // Flash was available on macOS, including ARM64 (Rosetta 2)
                    // However, native ARM64 Flash may not be available
                    return true;

                case PlatformHelper.PlatformType.Linux:
                    return true; // Flash was available on Linux

                default:
                    return false;
            }
        }

        /// <summary>
        /// Get platform-specific warning message about Flash support
        /// </summary>
        public static string GetFlashSupportWarning()
        {
            if (PlatformHelper.IsMacARM)
            {
                return "Note: Running on macOS ARM64. Flash may run through Rosetta 2 emulation if native ARM64 version is not available.";
            }

            if (!IsFlashSupported())
            {
                return $"Warning: Flash support may not be available on {PlatformHelper.CurrentPlatform}.";
            }

            return string.Empty;
        }
    }
}
