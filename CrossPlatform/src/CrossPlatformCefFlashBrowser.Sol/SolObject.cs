namespace CrossPlatformCefFlashBrowser.Sol
{
    public class SolClassDef
    {
        public bool Dynamic { get; set; }
        public bool Externalizable { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Members { get; set; } = new();
    }

    public class SolObject
    {
        public SolClassDef ClassDef { get; set; } = new();
        public Dictionary<string, SolValue> Properties { get; set; } = new();
    }
}
