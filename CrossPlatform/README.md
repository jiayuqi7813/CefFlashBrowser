# Cross-Platform CefFlashBrowser

This is a cross-platform rewrite of the original Windows-only CefFlashBrowser application, supporting Windows, macOS, and Linux.

## Architecture

The application is built using Avalonia UI, a cross-platform XAML-based UI framework similar to WPF, which allows for a familiar development experience while enabling deployment across multiple platforms.

### Project Structure

```
CrossPlatform/
├── CrossPlatformCefFlashBrowser.sln          # Solution file
└── src/
    ├── CrossPlatformCefFlashBrowser/          # Main Avalonia UI application
    │   ├── Views/                             # UI views and windows
    │   ├── ViewModels/                        # MVVM view models
    │   └── Services/                          # Platform-specific services
    ├── CrossPlatformCefFlashBrowser.Core/     # Core business logic (platform-independent)
    │   ├── Models/                            # Data models
    │   └── Services/                          # Service interfaces
    ├── CrossPlatformCefFlashBrowser.Sol/      # SOL file processing library
    │   ├── SolFile.cs                         # Main SOL file handler
    │   ├── SolParser.cs                       # SOL file reader
    │   ├── SolWriter.cs                       # SOL file writer
    │   └── Models/                            # SOL data structures
    └── CrossPlatformCefFlashBrowser.Tests/    # Unit tests
```

## Key Features

### 1. Cross-Platform Compatibility
- **Windows**: Full support with native UI integration
- **macOS**: Native macOS application bundle
- **Linux**: GTK-based UI with native file dialogs

### 2. SOL File Processing (Flash Save Files)
The C++ SOL file processing library has been completely rewritten in C# for better cross-platform compatibility:

- **SolFile**: Main class for loading and saving SOL files
- **SolParser**: Reads SOL file format (AMF0 encoding)
- **SolWriter**: Writes SOL file format
- **Full compatibility**: Maintains 100% compatibility with original Flash SOL files

#### SOL File Usage Example

```csharp
using CrossPlatformCefFlashBrowser.Sol;

// Load a SOL file
var solFile = SolFile.Parse("/path/to/save.sol");

if (solFile.IsValid)
{
    // Access data
    foreach (var kvp in solFile.Data)
    {
        Console.WriteLine($"{kvp.Key}: {kvp.Value.Type}");
    }
    
    // Modify data
    solFile.Data["score"] = SolValue.Integer(1000);
    
    // Save changes
    solFile.Save();
}
```

### 3. Settings Management
Settings are stored in JSON format for easy editing and debugging:

```csharp
var settingsService = new SettingsService(fileService);
var settings = await settingsService.LoadSettingsAsync();

// Modify settings
settings.Theme = Theme.Dark;
settings.Language = "en-US";

// Save settings
await settingsService.SaveSettingsAsync(settings);
```

### 4. Favorites Management
Website favorites can be managed through the `IFavoritesService`:

```csharp
var favoritesService = new FavoritesService(fileService);
var favorites = await favoritesService.LoadFavoritesAsync();

// Add a favorite
await favoritesService.AddFavoriteAsync(new Website 
{ 
    Name = "Example", 
    Url = "https://example.com" 
});
```

## Technology Stack

### UI Framework
- **Avalonia UI 11.3.6**: Cross-platform XAML-based UI framework
  - Similar to WPF, making migration easier
  - Supports Windows, macOS, Linux, iOS, Android, and WebAssembly
  - MVVM pattern support

### Core Technologies
- **.NET 8.0**: Latest long-term support version
- **C# 12**: Modern C# features
- **System.Text.Json**: Fast, low-allocation JSON serialization

### Browser Engine
Current implementation uses Avalonia's built-in WebView, but can be extended with:
- **CefGlue**: Cross-platform CEF wrapper (recommended for Flash support)
- **WebView2** (Windows only): Microsoft's modern web control
- **Ruffle**: WebAssembly-based Flash emulator

## Building and Running

### Prerequisites
- .NET 8.0 SDK or later
- Platform-specific requirements:
  - **Windows**: Visual Studio 2022 or JetBrains Rider
  - **macOS**: Xcode command-line tools
  - **Linux**: GTK+ 3 development libraries

### Build Commands

```bash
# Navigate to the CrossPlatform directory
cd CrossPlatform

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project src/CrossPlatformCefFlashBrowser
```

### Platform-Specific Builds

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# macOS (Intel)
dotnet publish -c Release -r osx-x64 --self-contained

# macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained
```

## Migration from Original CefFlashBrowser

### Data Migration
User data from the original Windows application can be migrated:

1. **Settings**: Convert from binary/XML format to JSON
2. **Favorites**: Import from the original favorites file
3. **SOL Files**: 100% compatible, no conversion needed

### Feature Parity
Current implementation includes:
- ✅ SOL file reading and writing
- ✅ Settings management
- ✅ Favorites management
- ✅ Cross-platform file access
- ⏳ Browser with WebView (basic implementation)
- ⏳ SWF player (requires Flash emulator integration)
- ⏳ Advanced browser features (DevTools, etc.)

## Flash Support Options

Since Flash Player is no longer supported in modern browsers, we have several options:

### Option 1: Ruffle (Recommended)
Integrate the Ruffle WebAssembly Flash emulator:
```csharp
// Load Ruffle in WebView
webView.Navigate("ruffle.html?swf=" + swfPath);
```

### Option 2: Custom Flash Player
Bundle a standalone Flash player executable for local SWF playback.

### Option 3: CEF with Flash Plugin
Use CefGlue with the last Flash plugin version (requires careful licensing consideration).

## Testing

Run the test suite:

```bash
cd CrossPlatform
dotnet test
```

### Test Coverage
- SOL file parsing and writing
- Settings serialization
- Favorites management
- Cross-platform file operations

## Contributing

This is a cross-platform rewrite of the original CefFlashBrowser. Contributions are welcome!

### Development Guidelines
1. Keep platform-specific code in separate files with conditional compilation
2. Use dependency injection for platform-specific services
3. Maintain backward compatibility with SOL file format
4. Write unit tests for core business logic
5. Follow C# coding conventions

## Known Limitations

1. **Flash Support**: Modern browsers don't support Flash Plugin natively. Ruffle emulator recommended.
2. **Performance**: SOL file operations are slightly slower than native C++ (acceptable for typical use cases)
3. **Platform Differences**: Some UI elements may look different across platforms

## Roadmap

### Phase 1 (Current)
- [x] Project structure setup
- [x] SOL file library (C# rewrite)
- [x] Core business logic
- [x] Basic settings and favorites management

### Phase 2 (Next)
- [ ] Complete Avalonia UI implementation
- [ ] Browser component with WebView
- [ ] SWF player with Ruffle integration
- [ ] Platform-specific file services

### Phase 3 (Future)
- [ ] Advanced browser features
- [ ] Proxy support
- [ ] User agent customization
- [ ] Theme system
- [ ] Multi-language support

### Phase 4 (Enhancement)
- [ ] Performance optimizations
- [ ] Additional platforms (iOS, Android)
- [ ] Cloud sync for settings/favorites
- [ ] Plugin system

## License

This project maintains the same license as the original CefFlashBrowser.

## Acknowledgments

- Original CefFlashBrowser by Mzying2001
- Avalonia UI team for the excellent cross-platform framework
- Ruffle project for Flash preservation efforts
