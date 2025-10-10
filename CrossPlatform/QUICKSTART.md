# Quick Start Guide: Cross-Platform CefFlashBrowser

## For End Users

### Installation

#### Windows

1. Download the Windows release package
2. Extract to a folder (e.g., `C:\Program Files\CrossPlatformCefFlashBrowser`)
3. Run `CrossPlatformCefFlashBrowser.exe`

**Requirements**: Windows 10 version 1809 or later

#### macOS

1. Download the macOS release package (.dmg)
2. Open the DMG file
3. Drag the app to Applications folder
4. Right-click and select "Open" on first launch (to bypass Gatekeeper)

**Requirements**: macOS 10.15 (Catalina) or later

#### Linux

**Ubuntu/Debian**:
```bash
# Install dependencies
sudo apt install libgtk-3-0

# Download and extract
wget https://github.com/[...]/cefflashbrowser-linux-x64.tar.gz
tar -xzf cefflashbrowser-linux-x64.tar.gz
cd CrossPlatformCefFlashBrowser
./CrossPlatformCefFlashBrowser
```

**Fedora/RHEL**:
```bash
# Install dependencies
sudo dnf install gtk3

# Download and run
```

### Basic Usage

#### Managing SOL Save Files

**Open SOL File**:
1. Launch the application
2. Go to File → Open SOL File
3. Select your .sol file
4. Edit the values
5. Save changes

**Example - Editing Game Save**:
```
File: game.sol
├── playerName: "Alice"      → Change to: "Bob"
├── score: 1000              → Change to: 9999
├── level: 5                 → Change to: 10
└── inventory
    ├── gold: 500            → Change to: 10000
    └── potions: 3           → Change to: 99
```

**Export/Import**:
- Export: File → Export SOL → Choose location
- Import: File → Import SOL → Select file

#### Settings

**Access Settings**: Menu → Settings (or Ctrl/Cmd+,)

**Common Settings**:
- **Theme**: Light or Dark mode
- **Language**: Select preferred language
- **Proxy**: Configure proxy server
- **User Agent**: Customize browser identification

**Settings Location**:
- Windows: `%APPDATA%\CrossPlatformCefFlashBrowser\settings.json`
- macOS: `~/Library/Application Support/CrossPlatformCefFlashBrowser/settings.json`
- Linux: `~/.config/CrossPlatformCefFlashBrowser/settings.json`

#### Favorites

**Add Favorite**:
1. Navigate to a website
2. Click the star icon (or Ctrl/Cmd+D)
3. Enter name and URL
4. Click Save

**Manage Favorites**:
- View: Menu → Favorites → Manage
- Edit: Click edit icon
- Delete: Click delete icon
- Import/Export: Use the import/export buttons

### Troubleshooting

#### Application Won't Start

**Windows**:
- Ensure .NET 8.0 Runtime is installed
- Check antivirus isn't blocking the app
- Run as Administrator if needed

**macOS**:
- Right-click → Open (first time only)
- Check System Preferences → Security
- Ensure app is from trusted source

**Linux**:
- Install GTK+ 3 dependencies
- Check file permissions: `chmod +x CrossPlatformCefFlashBrowser`
- Run from terminal to see error messages

#### SOL Files Not Opening

- Ensure file has .sol extension
- Check file isn't corrupted
- Verify file permissions
- Try opening in a text editor to verify it's a valid SOL file

#### Settings Not Saving

- Check write permissions for settings directory
- Ensure disk isn't full
- Verify settings.json isn't read-only

### Tips & Tricks

**Keyboard Shortcuts**:
- `Ctrl/Cmd + O`: Open SOL file
- `Ctrl/Cmd + S`: Save SOL file
- `Ctrl/Cmd + ,`: Settings
- `Ctrl/Cmd + D`: Add favorite
- `Ctrl/Cmd + Q`: Quit application

**Performance**:
- Close unused windows
- Clear browser cache regularly
- Limit number of open tabs

**Backup**:
- Regularly backup SOL files: File → Export SOL
- Export favorites: Favorites → Export
- Settings are automatically backed up in JSON format

## For Developers

### Development Setup

#### Prerequisites

```bash
# Check .NET version
dotnet --version
# Should be 8.0.x or later

# If not installed, download from:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

#### Clone and Build

```bash
# Clone the repository
git clone https://github.com/jiayuqi7813/CefFlashBrowser.git
cd CefFlashBrowser/CrossPlatform

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the application
dotnet run --project src/CrossPlatformCefFlashBrowser
```

### Project Structure

```
src/
├── CrossPlatformCefFlashBrowser/          # Main application
│   ├── Views/                             # UI views (XAML)
│   ├── ViewModels/                        # View models
│   ├── Services/                          # App services
│   └── App.axaml.cs                       # Entry point
│
├── CrossPlatformCefFlashBrowser.Core/     # Business logic
│   ├── Models/                            # Data models
│   └── Services/                          # Service interfaces
│
├── CrossPlatformCefFlashBrowser.Sol/      # SOL file library
│   ├── SolFile.cs                         # Main API
│   ├── SolParser.cs                       # Reader
│   └── SolWriter.cs                       # Writer
│
└── CrossPlatformCefFlashBrowser.Tests/    # Unit tests
```

### Common Development Tasks

#### Running the Application

```bash
# Debug mode (with hot reload)
dotnet watch run --project src/CrossPlatformCefFlashBrowser

# Release mode
dotnet run --project src/CrossPlatformCefFlashBrowser -c Release
```

#### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter ClassName=SolValueTests

# Run with code coverage
dotnet test /p:CollectCoverage=true

# Run in watch mode (re-run on changes)
dotnet watch test
```

#### Building for Distribution

**Windows**:
```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
# Output: bin/Release/net8.0/win-x64/publish/
```

**macOS**:
```bash
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true
# Output: bin/Release/net8.0/osx-arm64/publish/
```

**Linux**:
```bash
dotnet publish -c Release -r linux-x64 --self-confined -p:PublishSingleFile=true
# Output: bin/Release/net8.0/linux-x64/publish/
```

### Code Examples

#### Working with SOL Files

```csharp
using CrossPlatformCefFlashBrowser.Sol;

// Load a SOL file
var solFile = SolFile.Parse("game.sol");

if (solFile.IsValid)
{
    // Read values
    var playerName = solFile.Data["playerName"].Get<string>();
    var score = solFile.Data["score"].Get<int>();
    
    Console.WriteLine($"Player: {playerName}, Score: {score}");
    
    // Modify values
    solFile.Data["score"] = SolValue.Integer(9999);
    solFile.Data["level"] = SolValue.Integer(10);
    
    // Add new values
    solFile.Data["newSetting"] = SolValue.String("enabled");
    
    // Save changes
    solFile.Save(); // or solFile.Save("newfile.sol")
}
else
{
    Console.WriteLine($"Error: {solFile.ErrorMessage}");
}
```

#### Working with Complex Objects

```csharp
// Create a complex object structure
var inventory = new SolObject();
inventory.Properties["gold"] = SolValue.Integer(500);
inventory.Properties["items"] = SolValue.Array(new SolArray
{
    Assoc = new Dictionary<string, SolValue>
    {
        ["sword"] = SolValue.Integer(1),
        ["potion"] = SolValue.Integer(5)
    }
});

solFile.Data["inventory"] = SolValue.Object(inventory);

// Read it back
var inv = solFile.Data["inventory"].Get<SolObject>();
var gold = inv!.Properties["gold"].Get<int>();
Console.WriteLine($"Gold: {gold}");
```

#### Using Services in ViewModels

```csharp
using CrossPlatformCefFlashBrowser.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MyViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IFavoritesService _favoritesService;

    [ObservableProperty]
    private Settings _settings;

    public MyViewModel(
        ISettingsService settingsService,
        IFavoritesService favoritesService)
    {
        _settingsService = settingsService;
        _favoritesService = favoritesService;
        
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        Settings = await _settingsService.LoadSettingsAsync();
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _settingsService.SaveSettingsAsync(Settings);
    }
    
    [RelayCommand]
    private async Task AddFavoriteAsync(string url)
    {
        await _favoritesService.AddFavoriteAsync(new Website 
        { 
            Name = "Example", 
            Url = url 
        });
    }
}
```

#### Creating Platform-Specific Services

```csharp
public interface IDialogService
{
    Task<string?> ShowOpenFileDialogAsync();
    Task ShowMessageAsync(string message);
}

#if WINDOWS
public class WindowsDialogService : IDialogService
{
    public async Task<string?> ShowOpenFileDialogAsync()
    {
        // Windows-specific implementation
        var dialog = new OpenFileDialog();
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
#elif OSX
public class MacDialogService : IDialogService
{
    public async Task<string?> ShowOpenFileDialogAsync()
    {
        // macOS-specific implementation
    }
}
#endif
```

### IDE Setup

#### Visual Studio 2022

1. Open `CrossPlatformCefFlashBrowser.sln`
2. Set `CrossPlatformCefFlashBrowser` as startup project
3. Press F5 to run

#### JetBrains Rider

1. Open `CrossPlatformCefFlashBrowser.sln`
2. Right-click project → Run
3. Use built-in Avalonia previewer for XAML

#### VS Code

1. Install C# extension
2. Open CrossPlatform folder
3. Press F5 to debug

**Recommended Extensions**:
- C# (Microsoft)
- Avalonia for VSCode
- .NET Core Test Explorer

### Debugging

**Attach Debugger**:
```csharp
if (Debugger.IsAttached)
{
    // Debug-only code
    Console.WriteLine("Debugger attached");
}
```

**Conditional Breakpoints**:
```csharp
// Only break when score > 1000
// Breakpoint condition: score > 1000
var score = solFile.Data["score"].Get<int>();
```

**Logging**:
```csharp
// Add logging service
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
    builder.SetMinimumLevel(LogLevel.Debug);
});

// Use in code
_logger.LogInformation("Loading SOL file: {Path}", filePath);
```

### Testing

**Writing Tests**:
```csharp
[TestClass]
public class MyTests
{
    [TestInitialize]
    public void Setup()
    {
        // Runs before each test
    }

    [TestMethod]
    public void MyTest_Condition_ExpectedResult()
    {
        // Arrange
        var service = new MyService();
        
        // Act
        var result = service.DoSomething();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expected, result);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Runs after each test
    }
}
```

**Mocking Services**:
```csharp
public class MockFileService : IFileService
{
    private readonly Dictionary<string, byte[]> _files = new();

    public Task<byte[]> ReadFileAsync(string path)
    {
        return Task.FromResult(_files.ContainsKey(path) 
            ? _files[path] 
            : Array.Empty<byte>());
    }

    public Task WriteFileAsync(string path, byte[] data)
    {
        _files[path] = data;
        return Task.CompletedTask;
    }
}
```

### Contributing

**Workflow**:
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Make changes and add tests
4. Run tests: `dotnet test`
5. Commit: `git commit -m "Add feature"`
6. Push: `git push origin feature/my-feature`
7. Create Pull Request

**Code Style**:
- Follow C# conventions
- Use meaningful names
- Add XML documentation for public APIs
- Keep methods small and focused
- Write tests for new features

**Commit Messages**:
```
feat: Add SOL array support
fix: Correct byte order in parser
docs: Update README with examples
test: Add round-trip tests
refactor: Simplify service initialization
```

## Resources

### Documentation
- [README.md](README.md) - Project overview
- [MIGRATION.md](MIGRATION.md) - Migration guide
- [DEVELOPER.md](DEVELOPER.md) - Developer guide
- [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) - Implementation details

### External Links
- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [.NET 8.0 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Flash SOL Format Specification](https://www.adobe.com/content/dam/acom/en/devnet/pdf/amf0-file-format-specification.pdf)

### Support
- GitHub Issues: Report bugs or request features
- GitHub Discussions: Ask questions or share ideas
- Documentation: Check guides for answers

## FAQ

**Q: Is this compatible with the original Windows version?**
A: SOL files are 100% compatible. Settings and favorites need manual migration.

**Q: Can I use this on macOS Apple Silicon?**
A: Yes! Build with `osx-arm64` target for native performance.

**Q: Does it support Flash games?**
A: Flash emulator support (Ruffle) is planned for Phase 2.

**Q: How do I report bugs?**
A: Open an issue on GitHub with steps to reproduce.

**Q: Can I contribute?**
A: Absolutely! See the contributing section above.

**Q: Is my data safe?**
A: Settings are stored in JSON. SOL files are never modified without explicit save.

## Getting Help

1. **Check Documentation**: Review README, MIGRATION, and DEVELOPER guides
2. **Search Issues**: Look for similar problems on GitHub
3. **Ask Questions**: Use GitHub Discussions
4. **Report Bugs**: Create detailed issue with reproduction steps

---

**Version**: 1.0.0 (Phase 1 Complete)
**Last Updated**: 2025-10-10
**Platform**: Windows, macOS, Linux
**License**: Same as original CefFlashBrowser
