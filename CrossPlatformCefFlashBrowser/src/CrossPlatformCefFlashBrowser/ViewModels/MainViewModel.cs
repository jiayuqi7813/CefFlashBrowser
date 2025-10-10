using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossPlatformCefFlashBrowser.Services;
using Microsoft.Maui.Controls;

namespace CrossPlatformCefFlashBrowser.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IFlashPlayerService _flashPlayerService;

    [ObservableProperty]
    private string address = "https://flash.cnmodern.app";

    [ObservableProperty]
    private bool canGoBack;

    [ObservableProperty]
    private bool canGoForward;

    [ObservableProperty]
    private WebViewSource browserSource = new UrlWebViewSource { Url = "https://flash.cnmodern.app" };

    public MainViewModel(INavigationService navigationService, IFlashPlayerService flashPlayerService)
    {
        _navigationService = navigationService;
        _flashPlayerService = flashPlayerService;
    }

    partial void OnAddressChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            BrowserSource = new UrlWebViewSource { Url = value };
        }
    }

    [RelayCommand]
    private void Navigate()
    {
        if (string.IsNullOrWhiteSpace(Address))
        {
            return;
        }

        BrowserSource = new UrlWebViewSource { Url = Address };
        _navigationService.RecordHistory(Address);
        UpdateNavigationState();
    }

    [RelayCommand]
    private void GoBack()
    {
        var url = _navigationService.GoBack();
        if (url is null)
        {
            return;
        }

        BrowserSource = new UrlWebViewSource { Url = url };
        Address = url;
        UpdateNavigationState();
    }

    [RelayCommand]
    private void GoForward()
    {
        var url = _navigationService.GoForward();
        if (url is null)
        {
            return;
        }

        BrowserSource = new UrlWebViewSource { Url = url };
        Address = url;
        UpdateNavigationState();
    }

    [RelayCommand]
    private void Refresh()
    {
        BrowserSource = new UrlWebViewSource { Url = Address };
    }

    public void UpdateNavigationState()
    {
        CanGoBack = _navigationService.CanGoBack;
        CanGoForward = _navigationService.CanGoForward;
    }
}
