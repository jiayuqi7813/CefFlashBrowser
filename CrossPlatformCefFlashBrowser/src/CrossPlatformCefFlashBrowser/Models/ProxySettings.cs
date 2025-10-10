namespace CrossPlatformCefFlashBrowser.Models;

public class ProxySettings
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 8080;
    public string? Username { get; set; }
    public string? Password { get; set; }
}
