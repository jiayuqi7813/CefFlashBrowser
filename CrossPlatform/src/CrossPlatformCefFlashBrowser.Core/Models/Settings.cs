namespace CrossPlatformCefFlashBrowser.Core.Models
{
    public class Settings
    {
        public bool FirstStart { get; set; }
        public string Language { get; set; } = "en-US";
        public Theme Theme { get; set; }
        public SearchEngine SearchEngine { get; set; }
        public ProxySettings ProxySettings { get; set; } = new();
        public UserAgentSetting UserAgentSetting { get; set; } = new();
        public FakeFlashVersionSetting FakeFlashVersionSetting { get; set; } = new();
        public bool SaveZoomLevel { get; set; }
        public double BrowserZoomLevel { get; set; }
        public bool FollowSystemTheme { get; set; }

        public static Settings Default => new Settings
        {
            FirstStart = true,
            Language = "en-US",
            Theme = Theme.Light,
            SearchEngine = SearchEngine.Bing,
            ProxySettings = new ProxySettings(),
            UserAgentSetting = new UserAgentSetting(),
            FakeFlashVersionSetting = new FakeFlashVersionSetting(),
            SaveZoomLevel = true,
            BrowserZoomLevel = 0.0,
            FollowSystemTheme = true
        };

        public void SetNullPropertiesToDefault()
        {
            var defaultSettings = Default;

            foreach (var property in GetType().GetProperties())
            {
                if (property.GetValue(this) == null)
                    property.SetValue(this, property.GetValue(defaultSettings));
            }
        }
    }
}
