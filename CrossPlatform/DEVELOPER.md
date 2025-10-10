# Developer Guide: Cross-Platform CefFlashBrowser

## Table of Contents
1. [Getting Started](#getting-started)
2. [Project Structure](#project-structure)
3. [Core Components](#core-components)
4. [SOL File Format](#sol-file-format)
5. [Adding New Features](#adding-new-features)
6. [Platform-Specific Development](#platform-specific-development)
7. [Testing](#testing)
8. [Deployment](#deployment)

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- IDE: Visual Studio 2022, JetBrains Rider, or VS Code with C# extension
- Git for version control

### Clone and Build

```bash
cd CrossPlatform
dotnet restore
dotnet build
dotnet test
```

### Run the Application

```bash
dotnet run --project src/CrossPlatformCefFlashBrowser
```

## Project Structure

```
CrossPlatform/
├── src/
│   ├── CrossPlatformCefFlashBrowser/           # Main Avalonia application
│   │   ├── Views/                              # XAML views
│   │   ├── ViewModels/                         # View models (MVVM)
│   │   ├── Services/                           # Application services
│   │   │   └── Platform/                       # Platform-specific implementations
│   │   ├── Assets/                             # Images, icons, etc.
│   │   └── App.axaml.cs                        # Application entry point
│   │
│   ├── CrossPlatformCefFlashBrowser.Core/      # Core business logic
│   │   ├── Models/                             # Data models
│   │   │   ├── Settings.cs                     # Application settings
│   │   │   ├── Website.cs                      # Favorite website
│   │   │   └── ...                             # Other models
│   │   └── Services/                           # Service interfaces
│   │       ├── ISettingsService.cs             # Settings management
│   │       ├── IFavoritesService.cs            # Favorites management
│   │       └── IFileService.cs                 # File operations
│   │
│   ├── CrossPlatformCefFlashBrowser.Sol/       # SOL file processing
│   │   ├── SolFile.cs                          # Main SOL file API
│   │   ├── SolParser.cs                        # SOL file reader
│   │   ├── SolWriter.cs                        # SOL file writer
│   │   ├── SolValue.cs                         # SOL value wrapper
│   │   ├── SolArray.cs                         # SOL array type
│   │   ├── SolObject.cs                        # SOL object type
│   │   └── SolEnums.cs                         # SOL type enumerations
│   │
│   └── CrossPlatformCefFlashBrowser.Tests/     # Unit tests
│       ├── SolValueTests.cs                    # SOL value tests
│       └── SolParserWriterTests.cs             # Parser/writer tests
│
├── README.md                                    # Main documentation
├── MIGRATION.md                                 # Migration guide
└── CrossPlatformCefFlashBrowser.sln            # Solution file
```

## Core Components

### 1. Application Initialization

**App.axaml.cs** - Application startup and dependency injection:

```csharp
public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // Register core services
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IFavoritesService, FavoritesService>();
        
        // Register view models
        services.AddTransient<MainWindowViewModel>();
        
        ServiceProvider = services.BuildServiceProvider();
    }
}
```

### 2. MVVM Pattern

**ViewModels** use CommunityToolkit.Mvvm for reduced boilerplate:

```csharp
public partial class MainWindowViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    // Auto-property with change notification
    [ObservableProperty]
    private string _greeting = "Welcome!";

    // Auto-generated command
    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _settingsService.SaveSettingsAsync(Settings);
    }

    // Constructor injection
    public MainWindowViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }
}
```

### 3. Service Layer

**Service interfaces** define contracts:

```csharp
public interface ISettingsService
{
    Task<Settings> LoadSettingsAsync();
    Task SaveSettingsAsync(Settings settings);
}
```

**Service implementations** provide functionality:

```csharp
public class SettingsService : ISettingsService
{
    private readonly IFileService _fileService;
    
    public SettingsService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<Settings> LoadSettingsAsync()
    {
        // Implementation
    }
}
```

## SOL File Format

### Overview

SOL files are Flash Local Shared Objects (LSO), similar to browser cookies but for Flash applications.

### File Structure

```
+----------------+
| Magic (2 bytes)|  0x00 0xBF
+----------------+
| Chunk Size (4) |  File size - 6
+----------------+
| Constant (10)  |  TCSO header
+----------------+
| Name Length (2)|
+----------------+
| Name (variable)|
+----------------+
| Padding (1)    |  0x00
+----------------+
| Version (4)    |  AMF0 (0) or AMF3 (3)
+----------------+
| Data Pairs ... |  Key-value pairs
+----------------+
```

### Data Types (AMF0)

| Type | Byte | Description |
|------|------|-------------|
| Undefined | 0x00 | Undefined value |
| Null | 0x01 | Null value |
| Boolean False | 0x02 | False |
| Boolean True | 0x03 | True |
| Integer | 0x04 | 32-bit signed integer |
| Double | 0x05 | 64-bit IEEE 754 |
| String | 0x06 | UTF-8 string |
| Array | 0x09 | Associative array |
| Object | 0x0A | Named object |
| Binary | 0x0C | Binary data |

### Usage Example

```csharp
// Create a new SOL file
var solFile = new SolFile
{
    SolName = "myGame",
    Version = SolVersion.AMF0
};

// Add game data
solFile.Data["playerName"] = SolValue.String("Alice");
solFile.Data["score"] = SolValue.Integer(1000);
solFile.Data["level"] = SolValue.Integer(5);

// Create a complex object
var inventory = new SolObject();
inventory.Properties["gold"] = SolValue.Integer(500);
inventory.Properties["potions"] = SolValue.Integer(3);
solFile.Data["inventory"] = SolValue.Object(inventory);

// Save to file
solFile.Save("myGame.sol");

// Load from file
var loaded = SolFile.Parse("myGame.sol");
if (loaded.IsValid)
{
    var playerName = loaded.Data["playerName"].Get<string>();
    var score = loaded.Data["score"].Get<int>();
    Console.WriteLine($"{playerName} has score: {score}");
}
```

### Advanced: Working with Arrays

```csharp
// Create an array
var items = new SolArray();

// Associative array (key-value pairs)
items.Assoc["sword"] = SolValue.Integer(1);
items.Assoc["shield"] = SolValue.Integer(1);
items.Assoc["potion"] = SolValue.Integer(5);

// Dense array (indexed)
items.Dense.Add(SolValue.String("Item1"));
items.Dense.Add(SolValue.String("Item2"));

solFile.Data["items"] = SolValue.Array(items);
```

## Adding New Features

### Example: Adding a New Service

1. **Define the interface** in `Core/Services`:

```csharp
namespace CrossPlatformCefFlashBrowser.Core.Services
{
    public interface IHistoryService
    {
        Task<List<HistoryEntry>> LoadHistoryAsync();
        Task AddHistoryEntryAsync(HistoryEntry entry);
        Task ClearHistoryAsync();
    }
}
```

2. **Create the model** in `Core/Models`:

```csharp
namespace CrossPlatformCefFlashBrowser.Core.Models
{
    public class HistoryEntry
    {
        public string Url { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
```

3. **Implement the service** in `Core/Services`:

```csharp
public class HistoryService : IHistoryService
{
    private readonly IFileService _fileService;
    private const string HistoryFileName = "history.json";

    public HistoryService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<List<HistoryEntry>> LoadHistoryAsync()
    {
        // Implementation
    }

    // ... other methods
}
```

4. **Register the service** in `App.axaml.cs`:

```csharp
private void ConfigureServices()
{
    var services = new ServiceCollection();
    
    // Existing services...
    services.AddSingleton<IHistoryService, HistoryService>();
    
    ServiceProvider = services.BuildServiceProvider();
}
```

5. **Use in ViewModel**:

```csharp
public partial class BrowserViewModel : ObservableObject
{
    private readonly IHistoryService _historyService;

    public BrowserViewModel(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [RelayCommand]
    private async Task NavigateAsync(string url)
    {
        // Navigate logic...
        
        // Add to history
        await _historyService.AddHistoryEntryAsync(new HistoryEntry
        {
            Url = url,
            Timestamp = DateTime.Now,
            Title = pageTitle
        });
    }
}
```

### Example: Adding a New View

1. **Create the View** (`Views/HistoryWindow.axaml`):

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
        xmlns:vm="using:CrossPlatformCefFlashBrowser.ViewModels"
        x:Class="CrossPlatformCefFlashBrowser.Views.HistoryWindow"
        Title="History">
    
    <Design.DataContext>
        <vm:HistoryViewModel/>
    </Design.DataContext>

    <ListBox Items="{Binding HistoryEntries}">
        <ListBox.ItemTemplate>
            <DataTemplate>
                <StackPanel>
                    <TextBlock Text="{Binding Title}" FontWeight="Bold"/>
                    <TextBlock Text="{Binding Url}" FontSize="12"/>
                    <TextBlock Text="{Binding Timestamp}" FontSize="10"/>
                </StackPanel>
            </DataTemplate>
        </ListBox.ItemTemplate>
    </ListBox>
</Window>
```

2. **Create the ViewModel**:

```csharp
public partial class HistoryViewModel : ObservableObject
{
    private readonly IHistoryService _historyService;

    [ObservableProperty]
    private ObservableCollection<HistoryEntry> _historyEntries = new();

    public HistoryViewModel(IHistoryService historyService)
    {
        _historyService = historyService;
        _ = LoadHistoryAsync();
    }

    private async Task LoadHistoryAsync()
    {
        var entries = await _historyService.LoadHistoryAsync();
        HistoryEntries = new ObservableCollection<HistoryEntry>(entries);
    }

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        await _historyService.ClearHistoryAsync();
        HistoryEntries.Clear();
    }
}
```

## Platform-Specific Development

### Conditional Compilation

Use preprocessor directives for platform-specific code:

```csharp
#if WINDOWS
    using Windows.Storage.Pickers;
    
    private async Task<string?> PickFileWindows()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".sol");
        var file = await picker.PickSingleFileAsync();
        return file?.Path;
    }
#elif OSX
    using AppKit;
    
    private async Task<string?> PickFileMacOS()
    {
        var dialog = new NSOpenPanel();
        dialog.AllowedFileTypes = new[] { "sol" };
        var result = await dialog.RunModal();
        return result == 1 ? dialog.Url.Path : null;
    }
#else
    // Linux or other platforms
    private async Task<string?> PickFileLinux()
    {
        // GTK file picker implementation
        return null;
    }
#endif
```

### Platform Abstraction

Better approach - use platform abstractions:

```csharp
public interface IDialogService
{
    Task<string?> PickFileAsync(string[] extensions);
    Task<bool> ShowConfirmAsync(string message);
}

// Platform-specific implementations
#if WINDOWS
public class WindowsDialogService : IDialogService { }
#elif OSX
public class MacOSDialogService : IDialogService { }
#else
public class LinuxDialogService : IDialogService { }
#endif

// Register in DI
#if WINDOWS
services.AddSingleton<IDialogService, WindowsDialogService>();
#elif OSX
services.AddSingleton<IDialogService, MacOSDialogService>();
#else
services.AddSingleton<IDialogService, LinuxDialogService>();
#endif
```

## Testing

### Unit Tests

Write platform-independent unit tests:

```csharp
[TestClass]
public class SettingsServiceTests
{
    private IFileService _fileService;
    private ISettingsService _settingsService;

    [TestInitialize]
    public void Setup()
    {
        _fileService = new MockFileService();
        _settingsService = new SettingsService(_fileService);
    }

    [TestMethod]
    public async Task LoadSettings_NewUser_ReturnsDefault()
    {
        var settings = await _settingsService.LoadSettingsAsync();
        
        Assert.IsNotNull(settings);
        Assert.AreEqual("en-US", settings.Language);
        Assert.AreEqual(Theme.Light, settings.Theme);
    }

    [TestMethod]
    public async Task SaveAndLoad_PreservesSettings()
    {
        var settings = new Settings
        {
            Language = "zh-CN",
            Theme = Theme.Dark
        };

        await _settingsService.SaveSettingsAsync(settings);
        var loaded = await _settingsService.LoadSettingsAsync();

        Assert.AreEqual("zh-CN", loaded.Language);
        Assert.AreEqual(Theme.Dark, loaded.Theme);
    }
}
```

### Integration Tests

Test platform-specific functionality on each platform:

```csharp
[TestClass]
public class FileServiceIntegrationTests
{
    [TestMethod]
    public async Task GetAppDataPath_CreatesDirectory()
    {
        var fileService = new FileService();
        var path = await fileService.GetAppDataPathAsync();

        Assert.IsTrue(Directory.Exists(path));
        Assert.IsTrue(path.Contains("CrossPlatformCefFlashBrowser"));
    }
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~SolValueTests"
```

## Deployment

### Build Configurations

#### Development Build
```bash
dotnet build
```

#### Release Build (Self-Contained)
```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# macOS x64 (Intel)
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true

# macOS arm64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true

# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
```

#### Framework-Dependent Build (Smaller size)
```bash
dotnet publish -c Release -r win-x64 --no-self-contained
```

### Packaging

#### Windows
- Use Inno Setup or WiX to create installer
- Sign the executable with code signing certificate

#### macOS
```bash
# Create app bundle
dotnet publish -r osx-x64 -c Release

# Sign (requires Apple Developer certificate)
codesign --sign "Developer ID" --timestamp MyApp.app

# Create DMG
hdiutil create -volname "MyApp" -srcfolder MyApp.app -ov MyApp.dmg
```

#### Linux
```bash
# Create .deb package (Debian/Ubuntu)
# Create .rpm package (Fedora/Red Hat)
# Create AppImage (universal)
```

### Distribution

1. **GitHub Releases**: Upload platform-specific binaries
2. **Microsoft Store**: Windows distribution
3. **Mac App Store**: macOS distribution
4. **Snap Store**: Linux distribution

## Best Practices

### 1. Code Organization
- Keep business logic in Core project
- Put platform-specific code in Platform folders
- Use interfaces for testability

### 2. Performance
- Use async/await for I/O operations
- Cache frequently accessed data
- Use proper disposal (IDisposable)

### 3. Error Handling
```csharp
try
{
    var file = SolFile.Parse(path);
    if (!file.IsValid)
    {
        // Handle invalid file
        LogError(file.ErrorMessage);
    }
}
catch (Exception ex)
{
    LogError($"Error parsing SOL file: {ex.Message}");
}
```

### 4. Logging
Consider adding a logging framework:
```csharp
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
```

### 5. Configuration
Use appsettings.json for app configuration:
```json
{
  "AppSettings": {
    "MaxHistoryEntries": 100,
    "DefaultLanguage": "en-US"
  }
}
```

## Resources

- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Flash/AMF0 Specification](https://www.adobe.com/content/dam/acom/en/devnet/pdf/amf0-file-format-specification.pdf)

## Getting Help

- Review the tests for usage examples
- Check the MIGRATION.md for architecture comparisons
- Open issues on GitHub for bugs or questions
