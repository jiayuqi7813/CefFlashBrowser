namespace CrossPlatformCefFlashBrowser.Core.Models
{
    public class Website
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public Website() { }

        public Website(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
