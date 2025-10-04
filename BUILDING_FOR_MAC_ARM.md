# Building CefFlashBrowser for Mac ARM64

## Overview

This document provides instructions for building CefFlashBrowser to support macOS ARM64 (Apple Silicon) architecture.

## Current Limitations

CefFlashBrowser was originally designed for Windows only. The codebase has been updated with platform abstraction layers to support cross-platform compilation, but full macOS support requires additional steps:

### Platform Dependencies

1. **.NET Framework 4.6.2**: This is Windows-only. For macOS, you would need to migrate to .NET 6+ or later
2. **WPF (Windows Presentation Foundation)**: Windows-only UI framework. Consider using:
   - Avalonia UI (cross-platform XAML)
   - MAUI (cross-platform)
   - Native macOS UI with AppKit
3. **C++/CLI Projects**: The `.vcxproj` projects (`CefFlashBrowser.Sol`, `CefFlashBrowser.Singleton`) are Windows-specific

### Recent Changes for Mac ARM Support

The following changes have been implemented to support Mac ARM compilation:

1. **Platform Detection** (`Utils/PlatformHelper.cs`):
   - Runtime platform and architecture detection
   - Identifies macOS ARM64 specifically
   - Provides platform-specific library extensions

2. **Platform Abstraction** (`Utils/PlatformInitializer.cs`):
   - Cross-platform library path initialization
   - Platform-specific Flash plugin path resolution
   - Flash support warnings for different platforms

3. **macOS Native Interop** (`Utils/MacOS.cs`):
   - P/Invoke declarations for macOS native APIs
   - Dynamic library loading (dlopen/dlsym)
   - Library path configuration via DYLD_LIBRARY_PATH

4. **Windows API Wrappers** (`Utils/Win32.cs`):
   - Safe wrappers that work on non-Windows platforms
   - `SetDllDirectorySafe()` returns true on non-Windows (no-op)

5. **Build Configuration**:
   - Added ARM64 platform to solution configuration
   - Updated project files to support ARM64 target

## Building for macOS ARM64

### Prerequisites

1. **macOS with Apple Silicon (M1/M2/M3)** or Rosetta 2
2. **.NET 6 SDK or later** (for cross-platform support)
   ```bash
   brew install dotnet-sdk
   ```
3. **Visual Studio for Mac** or **VS Code** with C# extension
4. **Mono Framework** (for .NET Framework compatibility layer)

### Migration Steps

To fully support macOS ARM64, you need to migrate the project:

#### Option 1: Migrate to .NET 6+ (Recommended)

1. Update `TargetFramework` in `.csproj` files:
   ```xml
   <TargetFramework>net6.0-macos</TargetFramework>
   ```

2. Replace WPF with Avalonia:
   ```bash
   dotnet add package Avalonia
   dotnet add package Avalonia.Desktop
   ```

3. Convert C++/CLI projects to native libraries or P/Invoke wrappers

#### Option 2: Use Mono (Limited Support)

1. Install Mono:
   ```bash
   brew install mono
   ```

2. Build with Mono (limited WPF support):
   ```bash
   msbuild /p:Configuration=Release /p:Platform=ARM64
   ```

### Flash Plugin for macOS ARM64

**Important**: Adobe Flash Player was discontinued in December 2020. However:

1. **Intel-based Flash**: Available but runs via Rosetta 2 emulation on ARM
2. **Native ARM64 Flash**: Not officially available
3. **Alternative**: Consider using Ruffle (Flash emulator written in Rust) which has ARM64 support

To use Flash on macOS ARM:

1. Download the last macOS Flash Player PPAPI plugin
2. Place in `Assets/Plugins/` directory
3. The platform initializer will automatically detect the plugin

### Build Commands

#### Current Build (Windows ARM64)
```bash
# Build for ARM64 (on Windows ARM)
dotnet build -c Release -p:Platform=ARM64
```

#### Future macOS Build (after migration)
```bash
# Build for macOS ARM64
dotnet build -c Release -r osx-arm64
```

### CEF (Chromium Embedded Framework) for macOS

**Important Note on CefSharp Version**: The current version (84.4.10) does not support ARM64 platform. To enable ARM64 builds:

1. **Option A - Update CefSharp** (Recommended for actual Mac deployment):
   - Upgrade to CefSharp 100+ which has ARM64 support
   - Update package references in `.csproj` files:
     ```xml
     <PackageReference Include="CefSharp.WinForms" Version="126.2.180" />
     ```

2. **Option B - Use x64 with Rosetta 2** (Quick test):
   - Build for x64 instead of ARM64
   - Run on Mac ARM via Rosetta 2 emulation

CEF binaries for macOS ARM64:

1. Download from [CEF Builds](https://cef-builds.spotifycdn.com/index.html)
2. Select: `macosarm64` platform
3. Extract to `Assets/CefSharp/`
4. Update asset references in the project

### Runtime Configuration

The code now includes platform detection that:

1. Skips Windows-specific initialization on macOS
2. Uses DYLD_LIBRARY_PATH instead of SetDllDirectory
3. Logs platform information on startup
4. Warns when running on ARM64 with emulated Flash

### Known Issues

1. **WPF Dependency**: Main blocker for macOS support
2. **C++/CLI Projects**: Need replacement with native code
3. **CefSharp 84.4.10**: Does not support ARM64 platform (upgrade to 100+ required)
4. **Flash Availability**: No native ARM64 Flash Player
5. **User32/Kernel32 APIs**: Many Windows-specific calls throughout codebase

### Testing on macOS ARM

1. Build the project with ARM64 configuration
2. Copy necessary CEF binaries for macOS ARM64
3. Copy Flash plugin (if available)
4. Run and check logs for platform detection:
   ```
   Platform: MacOS ARM64
   Note: Running on macOS ARM64. Flash may run through Rosetta 2...
   ```

## Roadmap

- [ ] Migrate to .NET 6+
- [ ] Replace WPF with Avalonia or native macOS UI
- [ ] Port C++/CLI projects to native libraries
- [ ] Add proper macOS application bundle (.app)
- [ ] Integrate Ruffle as Flash alternative
- [ ] Add CI/CD for macOS builds
- [ ] Create macOS-specific installer (DMG)

## Contributing

If you're interested in helping with macOS ARM64 support:

1. Test the current changes on macOS ARM
2. Help with WPF to Avalonia migration
3. Port C++/CLI code to native implementations
4. Test CEF functionality on macOS ARM64

## References

- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML UI framework
- [CEF Builds](https://cef-builds.spotifycdn.com/index.html) - CEF binary downloads
- [Ruffle](https://ruffle.rs/) - Flash Player emulator
- [.NET Multi-platform App UI](https://docs.microsoft.com/en-us/dotnet/maui/) - Cross-platform framework
