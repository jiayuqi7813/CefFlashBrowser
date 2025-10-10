using CommunityToolkit.Mvvm.DependencyInjection;
using CrossPlatformCefFlashBrowser.Core;
using CrossPlatformCefFlashBrowser.Services;
using CrossPlatformCefFlashBrowser.ViewModels;
using CrossPlatformCefFlashBrowser.Utils;
using Microsoft.Extensions.Logging;

namespace CrossPlatformCefFlashBrowser;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddCoreServices();
        builder.Services.AddSingleton<IFileService, FileService>();
        builder.Services.AddSingleton<IWebViewBridge, WebViewBridge>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IFavoritesService, FavoritesService>();
        builder.Services.AddSingleton<IFlashPlayerService, RuffleFlashPlayerService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<Views.MainPage>();

        Ioc.Default.ConfigureServices(builder.Services.BuildServiceProvider());

        return builder.Build();
    }
}
