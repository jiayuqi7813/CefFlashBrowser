using System;
using System.Runtime.InteropServices;

namespace CefFlashBrowser.Utils
{
    /// <summary>
    /// Platform detection and abstraction helper
    /// </summary>
    public static class PlatformHelper
    {
        public enum PlatformType
        {
            Windows,
            MacOS,
            Linux,
            Unknown
        }

        public enum ArchitectureType
        {
            X86,
            X64,
            ARM,
            ARM64,
            Unknown
        }

        private static PlatformType? _currentPlatform;
        private static ArchitectureType? _currentArchitecture;

        /// <summary>
        /// Gets the current platform
        /// </summary>
        public static PlatformType CurrentPlatform
        {
            get
            {
                if (_currentPlatform.HasValue)
                    return _currentPlatform.Value;

#if NETFRAMEWORK
                // .NET Framework runs only on Windows
                _currentPlatform = PlatformType.Windows;
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    _currentPlatform = PlatformType.Windows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    _currentPlatform = PlatformType.MacOS;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    _currentPlatform = PlatformType.Linux;
                else
                    _currentPlatform = PlatformType.Unknown;
#endif
                return _currentPlatform.Value;
            }
        }

        /// <summary>
        /// Gets the current architecture
        /// </summary>
        public static ArchitectureType CurrentArchitecture
        {
            get
            {
                if (_currentArchitecture.HasValue)
                    return _currentArchitecture.Value;

#if NETFRAMEWORK
                _currentArchitecture = Environment.Is64BitProcess ? ArchitectureType.X64 : ArchitectureType.X86;
#else
                var arch = RuntimeInformation.ProcessArchitecture;
                switch (arch)
                {
                    case Architecture.X86:
                        _currentArchitecture = ArchitectureType.X86;
                        break;
                    case Architecture.X64:
                        _currentArchitecture = ArchitectureType.X64;
                        break;
                    case Architecture.Arm:
                        _currentArchitecture = ArchitectureType.ARM;
                        break;
                    case Architecture.Arm64:
                        _currentArchitecture = ArchitectureType.ARM64;
                        break;
                    default:
                        _currentArchitecture = ArchitectureType.Unknown;
                        break;
                }
#endif
                return _currentArchitecture.Value;
            }
        }

        /// <summary>
        /// Checks if the current platform is Windows
        /// </summary>
        public static bool IsWindows => CurrentPlatform == PlatformType.Windows;

        /// <summary>
        /// Checks if the current platform is macOS
        /// </summary>
        public static bool IsMacOS => CurrentPlatform == PlatformType.MacOS;

        /// <summary>
        /// Checks if the current platform is Linux
        /// </summary>
        public static bool IsLinux => CurrentPlatform == PlatformType.Linux;

        /// <summary>
        /// Checks if the current architecture is ARM64
        /// </summary>
        public static bool IsARM64 => CurrentArchitecture == ArchitectureType.ARM64;

        /// <summary>
        /// Checks if the current platform is macOS ARM64
        /// </summary>
        public static bool IsMacARM => IsMacOS && IsARM64;

        /// <summary>
        /// Gets the platform-specific library extension
        /// </summary>
        public static string LibraryExtension
        {
            get
            {
                switch (CurrentPlatform)
                {
                    case PlatformType.Windows:
                        return ".dll";
                    case PlatformType.MacOS:
                        return ".dylib";
                    case PlatformType.Linux:
                        return ".so";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a platform-specific description string
        /// </summary>
        public static string GetPlatformDescription()
        {
            return $"{CurrentPlatform} {CurrentArchitecture}";
        }
    }
}
