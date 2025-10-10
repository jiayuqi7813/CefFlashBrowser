namespace CrossPlatformCefFlashBrowser.Models;

public class Settings
{
    public bool FirstStart { get; set; } = true;
    public string Language { get; set; } = "zh-CN";
    public Theme Theme { get; set; } = Theme.Light;
    public SearchEngine SearchEngine { get; set; } = SearchEngine.DuckDuckGo;
    public ProxySettings Proxy { get; set; } = new();
    public UserAgentSetting UserAgent { get; set; } = new();
    public FakeFlashVersionSetting FlashVersion { get; set; } = new();

    public static Settings CreateDefault() => new();
}
