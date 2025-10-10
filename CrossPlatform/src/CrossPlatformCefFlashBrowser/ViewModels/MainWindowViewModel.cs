using CrossPlatformCefFlashBrowser.Core.Services;
using CrossPlatformCefFlashBrowser.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CrossPlatformCefFlashBrowser.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IFavoritesService _favoritesService;

    [ObservableProperty]
    private string _greeting = "Welcome to Cross-Platform CefFlashBrowser!";

    [ObservableProperty]
    private Settings _settings = Settings.Default;

    public MainWindowViewModel(
        ISettingsService settingsService,
        IFavoritesService favoritesService)
    {
        _settingsService = settingsService;
        _favoritesService = favoritesService;
        
        _ = LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        Settings = await _settingsService.LoadSettingsAsync();
        Greeting = $"Welcome! Language: {Settings.Language}, Theme: {Settings.Theme}";
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _settingsService.SaveSettingsAsync(Settings);
    }
}
