namespace CrossPlatformCefFlashBrowser.Sol
{
    public class SolArray
    {
        public Dictionary<string, SolValue> Assoc { get; set; } = new();
        public List<SolValue> Dense { get; set; } = new();
    }
}
