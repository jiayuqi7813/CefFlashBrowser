# Migration Guide: Windows CefFlashBrowser to Cross-Platform

This guide helps users and developers migrate from the original Windows-only CefFlashBrowser to the new cross-platform version.

## For End Users

### System Requirements

#### Minimum Requirements
- **Windows**: Windows 10 version 1809 or later
- **macOS**: macOS 10.15 (Catalina) or later
- **Linux**: Any modern distribution with GTK+ 3.24 or later

#### Runtime Dependencies
- .NET 8.0 Runtime (will be bundled with self-contained builds)

### Data Migration

#### 1. Settings Migration

The new cross-platform version uses JSON format for settings instead of binary format.

**Windows - Original Location:**
```
%APPDATA%\CefFlashBrowser\settings.dat
```

**Cross-Platform - New Location:**
```
Windows: %APPDATA%\CrossPlatformCefFlashBrowser\settings.json
macOS:   ~/Library/Application Support/CrossPlatformCefFlashBrowser/settings.json
Linux:   ~/.config/CrossPlatformCefFlashBrowser/settings.json
```

**Manual Migration Steps:**
1. Open the original CefFlashBrowser and note your settings
2. Open the cross-platform version
3. Configure the same settings through the UI
4. Settings will be automatically saved in JSON format

#### 2. Favorites Migration

Favorites can be exported from the original version and imported to the new version.

**Export from Original (Windows):**
1. Open CefFlashBrowser
2. Go to Favorites Manager
3. Export favorites to a file

**Import to Cross-Platform:**
1. Open Cross-Platform CefFlashBrowser
2. Go to Favorites Manager
3. Import the exported file

#### 3. SOL Save Files

✅ **Full Compatibility** - SOL files are 100% compatible between versions!

SOL save files work identically in both versions:
- No conversion needed
- Same file format (AMF0)
- All operations (read/write/edit) work the same way

**Location:**
SOL files are typically stored by Flash in:
```
Windows: %APPDATA%\Macromedia\Flash Player\#SharedObjects\[RANDOM_ID]\
```

The cross-platform version can read and write SOL files from any location.

### Feature Comparison

| Feature | Original (Windows) | Cross-Platform |
|---------|-------------------|----------------|
| Flash Browser | ✅ CefSharp with Flash Plugin | ⏳ WebView (Ruffle planned) |
| SWF Player | ✅ Native Flash | ⏳ Ruffle emulator |
| SOL File Manager | ✅ C++ implementation | ✅ C# implementation |
| Settings | ✅ Binary format | ✅ JSON format |
| Favorites | ✅ Supported | ✅ Supported |
| Multi-window | ✅ Supported | ⏳ Planned |
| DevTools | ✅ Integrated | ⏳ Planned |
| Themes | ✅ Light/Dark | ✅ Light/Dark |
| Languages | ✅ Multiple | ⏳ Planned |
| Proxy Support | ✅ Supported | ⏳ Planned |

✅ = Fully implemented
⏳ = Planned/In progress
❌ = Not planned

## For Developers

### Architecture Changes

#### Original Architecture
```
CefFlashBrowser (Windows Only)
├── WPF UI Layer
├── CefSharp Browser Engine (Windows-specific)
├── C++ SOL Processing (Native DLL)
├── SimpleMvvm Framework
└── HandyControl UI Library
```

#### New Architecture
```
CrossPlatformCefFlashBrowser
├── Avalonia UI Layer (Cross-platform)
├── WebView/CefGlue (Cross-platform)
├── C# SOL Processing (Pure managed code)
├── CommunityToolkit.Mvvm
└── Built-in Avalonia Controls
```

### Key Technical Changes

#### 1. UI Framework: WPF → Avalonia

**XAML Changes:**
```xml
<!-- WPF -->
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Button Content="Click Me" Click="Button_Click"/>
</Window>

<!-- Avalonia -->
<Window xmlns="https://github.com/avaloniaui">
  <Button Content="Click Me" Command="{Binding ClickCommand}"/>
</Window>
```

**Key Differences:**
- Namespace changed from WPF to Avalonia
- Some controls have different names or properties
- MVVM pattern is more strongly encouraged

#### 2. Browser Engine Options

**Original:** CefSharp (Windows-only)
```csharp
var browser = new ChromiumWebBrowser();
browser.Address = "https://example.com";
```

**New Options:**

**Option A: Built-in WebView**
```csharp
var webView = new WebView();
// Platform-specific implementation
```

**Option B: CefGlue (Recommended)**
```csharp
// Cross-platform CEF wrapper
// Requires additional setup
```

#### 3. SOL File Processing: C++ → C#

**Original (C++):**
```cpp
#include "sol.h"

sol::SolFile file;
file.path = "save.sol";
sol::ReadSolFile(file);
```

**New (C#):**
```csharp
using CrossPlatformCefFlashBrowser.Sol;

var file = SolFile.Parse("save.sol");
if (file.IsValid)
{
    // Process file
    file.Data["key"] = SolValue.Integer(42);
    file.Save();
}
```

**Benefits:**
- Pure managed code (no P/Invoke)
- Better cross-platform compatibility
- Easier to maintain and extend
- Same functionality and compatibility

#### 4. MVVM Framework: SimpleMvvm → CommunityToolkit.Mvvm

**Original:**
```csharp
using SimpleMvvm;

public class MyViewModel : ViewModelBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}
```

**New:**
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;
    
    // Property is auto-generated
}
```

**Benefits:**
- Source generators (less boilerplate)
- Better performance
- Industry-standard toolkit

#### 5. Dependency Injection

**Original:** Simple IoC container
```csharp
SimpleIoc.Global.Register<MyService>();
var service = SimpleIoc.Global.GetInstance<MyService>();
```

**New:** Microsoft.Extensions.DependencyInjection
```csharp
services.AddSingleton<IMyService, MyService>();
var service = serviceProvider.GetRequiredService<IMyService>();
```

### Platform-Specific Code

Use conditional compilation for platform-specific features:

```csharp
#if WINDOWS
    // Windows-specific code
    using Windows.Storage.Pickers;
#elif OSX
    // macOS-specific code
    using AppKit;
#elif LINUX
    // Linux-specific code
#endif
```

Or use platform abstractions:

```csharp
public interface IFileService
{
    Task<string?> PickFileAsync();
}

// Platform-specific implementations
public class WindowsFileService : IFileService { }
public class MacOSFileService : IFileService { }
public class LinuxFileService : IFileService { }
```

### Building for Multiple Platforms

```bash
# Development build (current platform)
dotnet build

# Release build for Windows
dotnet publish -c Release -r win-x64 --self-contained

# Release build for macOS (Intel)
dotnet publish -c Release -r osx-x64 --self-contained

# Release build for macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained

# Release build for Linux
dotnet publish -c Release -r linux-x64 --self-contained
```

### Testing Strategy

```csharp
// Unit tests work across all platforms
[TestClass]
public class SolFileTests
{
    [TestMethod]
    public void SolFile_Parse_ValidFile_Success()
    {
        var file = SolFile.Parse("test.sol");
        Assert.IsTrue(file.IsValid);
    }
}

// Run tests
dotnet test
```

## Flash Support in Modern Era

### The Flash Problem

Adobe Flash Player was discontinued on December 31, 2020. Modern browsers no longer support it.

### Solutions

#### 1. Ruffle (Recommended)

Ruffle is a Flash Player emulator written in Rust, compiled to WebAssembly.

**Advantages:**
- Works in modern browsers
- No security concerns
- Active development
- Good compatibility

**Integration:**
```html
<script src="ruffle.js"></script>
<script>
    window.RufflePlayer.config = {
        "autoplay": "auto",
        "unmuteOverlay": "hidden"
    };
</script>
```

#### 2. Standalone Flash Player

Bundle the last official Flash Player for offline SWF playback.

**Considerations:**
- Security risks (no updates)
- Windows-only
- Licensing concerns

#### 3. SWF to HTML5 Conversion

Convert SWF files to modern web technologies.

**Tools:**
- Google Swiffy (discontinued)
- Adobe Animate (commercial)
- Custom conversion scripts

## Troubleshooting

### Common Issues

#### Issue: Settings not migrating
**Solution:** Manually configure settings in the new version. The format change from binary to JSON prevents automatic migration.

#### Issue: SOL files not recognized
**Solution:** Ensure file permissions are correct. The C# parser has the same requirements as the C++ version.

#### Issue: Application won't start on macOS
**Solution:** Right-click and select "Open" to bypass Gatekeeper on first run.

#### Issue: Application won't start on Linux
**Solution:** Ensure GTK+ 3 dependencies are installed:
```bash
sudo apt-get install libgtk-3-0  # Debian/Ubuntu
sudo dnf install gtk3           # Fedora
```

## Future Plans

### Roadmap

**Phase 1: Core Functionality** (Current)
- ✅ SOL file processing
- ✅ Settings management
- ✅ Basic UI framework

**Phase 2: Browser Features**
- ⏳ WebView integration
- ⏳ Ruffle Flash emulator
- ⏳ DevTools support

**Phase 3: Advanced Features**
- ⏳ Multi-window support
- ⏳ Proxy configuration
- ⏳ User agent customization

**Phase 4: Platform Expansion**
- ⏳ Mobile support (iOS/Android)
- ⏳ Web browser version (WebAssembly)

## Getting Help

### Resources

- **Documentation**: See README.md in the CrossPlatform folder
- **Source Code**: Review the well-documented codebase
- **Tests**: Check unit tests for usage examples

### Reporting Issues

When reporting issues, please include:
1. Platform (Windows/macOS/Linux)
2. .NET version (`dotnet --version`)
3. Application version
4. Steps to reproduce
5. Expected vs actual behavior

## Contributing

Contributions are welcome! Areas where help is needed:
1. Platform-specific file dialogs
2. Ruffle Flash emulator integration
3. Advanced browser features
4. UI/UX improvements
5. Additional language translations

See the main repository for contribution guidelines.
