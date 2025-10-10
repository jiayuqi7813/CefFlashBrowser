using System;
using System.Collections.Generic;

namespace CrossPlatformCefFlashBrowser.Services;

public class NavigationService : INavigationService
{
    private readonly List<string> _history = new();
    private int _currentIndex = -1;

    public bool CanGoBack => _currentIndex > 0;
    public bool CanGoForward => _currentIndex >= 0 && _currentIndex < _history.Count - 1;

    public void RecordHistory(string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        if (CanGoForward)
        {
            _history.RemoveRange(_currentIndex + 1, _history.Count - (_currentIndex + 1));
        }

        _history.Add(url);
        _currentIndex = _history.Count - 1;
    }

    public string? GoBack()
    {
        if (!CanGoBack)
        {
            return null;
        }

        _currentIndex--;
        return _history[_currentIndex];
    }

    public string? GoForward()
    {
        if (!CanGoForward)
        {
            return null;
        }

        _currentIndex++;
        return _history[_currentIndex];
    }
}
