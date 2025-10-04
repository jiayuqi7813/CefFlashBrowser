using System;
using System.Runtime.InteropServices;

namespace CefFlashBrowser.Utils
{
    /// <summary>
    /// macOS-specific P/Invoke declarations and helpers
    /// Only available on macOS platform
    /// </summary>
    public static class MacOS
    {
        private const string LibSystem = "libSystem.dylib";
        private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
        private const string AppKit = "/System/Library/Frameworks/AppKit.framework/AppKit";

        /// <summary>
        /// Set library search path on macOS (equivalent to SetDllDirectory on Windows)
        /// </summary>
        public static bool SetLibraryPath(string path)
        {
            if (!PlatformHelper.IsMacOS)
                return false;

            try
            {
                // On macOS, we use DYLD_LIBRARY_PATH environment variable
                // or we rely on the OS to find libraries in standard locations
                if (!string.IsNullOrEmpty(path))
                {
                    var currentPath = Environment.GetEnvironmentVariable("DYLD_LIBRARY_PATH");
                    var newPath = string.IsNullOrEmpty(currentPath) ? path : $"{path}:{currentPath}";
                    Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", newPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load a dynamic library on macOS
        /// </summary>
        [DllImport(LibSystem, EntryPoint = "dlopen")]
        public static extern IntPtr DlOpen(string filename, int flags);

        /// <summary>
        /// Close a dynamic library handle
        /// </summary>
        [DllImport(LibSystem, EntryPoint = "dlclose")]
        public static extern int DlClose(IntPtr handle);

        /// <summary>
        /// Get a symbol from a dynamic library
        /// </summary>
        [DllImport(LibSystem, EntryPoint = "dlsym")]
        public static extern IntPtr DlSym(IntPtr handle, string symbol);

        /// <summary>
        /// Get error message from dynamic library loading
        /// </summary>
        [DllImport(LibSystem, EntryPoint = "dlerror")]
        public static extern IntPtr DlError();

        // dlopen flags
        public const int RTLD_LAZY = 0x0001;
        public const int RTLD_NOW = 0x0002;
        public const int RTLD_LOCAL = 0x0004;
        public const int RTLD_GLOBAL = 0x0008;

        /// <summary>
        /// Checks if running on macOS ARM64
        /// </summary>
        public static bool IsMacARM64()
        {
            return PlatformHelper.IsMacARM;
        }

        /// <summary>
        /// Gets the architecture string for the current macOS platform
        /// </summary>
        public static string GetArchitectureString()
        {
            if (!PlatformHelper.IsMacOS)
                return string.Empty;

            return PlatformHelper.IsARM64 ? "arm64" : "x86_64";
        }
    }
}
