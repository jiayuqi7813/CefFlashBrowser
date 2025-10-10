namespace CrossPlatformCefFlashBrowser.Sol
{
    public class SolFile
    {
        public string FilePath { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string SolName { get; set; } = string.Empty;
        public SolVersion Version { get; set; }
        public Dictionary<string, SolValue> Data { get; set; } = new();

        public bool IsValid => string.IsNullOrEmpty(ErrorMessage);

        public static SolFile Parse(string filePath)
        {
            var solFile = new SolFile { FilePath = filePath };
            
            try
            {
                var fileData = File.ReadAllBytes(filePath);
                var parser = new SolParser();
                return parser.ParseFile(fileData, filePath);
            }
            catch (Exception ex)
            {
                solFile.ErrorMessage = ex.Message;
                return solFile;
            }
        }

        public bool Save(string? filePath = null)
        {
            try
            {
                var targetPath = filePath ?? FilePath;
                var writer = new SolWriter();
                var data = writer.WriteFile(this);
                File.WriteAllBytes(targetPath, data);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
