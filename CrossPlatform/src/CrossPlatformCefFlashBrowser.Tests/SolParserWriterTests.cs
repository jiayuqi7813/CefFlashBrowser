using CrossPlatformCefFlashBrowser.Sol;

namespace CrossPlatformCefFlashBrowser.Tests;

[TestClass]
public class SolParserWriterTests
{
    [TestMethod]
    public void SolFile_RoundTrip_PreservesData()
    {
        // Create a test SOL file
        var originalFile = new SolFile
        {
            SolName = "testSave",
            Version = SolVersion.AMF0
        };

        // Add various data types
        originalFile.Data["stringValue"] = SolValue.String("Hello");
        originalFile.Data["intValue"] = SolValue.Integer(42);
        originalFile.Data["doubleValue"] = SolValue.Double(3.14159);
        originalFile.Data["boolTrue"] = SolValue.Boolean(true);
        originalFile.Data["boolFalse"] = SolValue.Boolean(false);
        originalFile.Data["nullValue"] = SolValue.Null();

        // Add an object
        var obj = new SolObject();
        obj.Properties["name"] = SolValue.String("Player");
        obj.Properties["score"] = SolValue.Integer(1000);
        originalFile.Data["gameData"] = SolValue.Object(obj);

        // Write to bytes
        var writer = new SolWriter();
        var bytes = writer.WriteFile(originalFile);

        // Parse back
        var parser = new SolParser();
        var parsedFile = parser.ParseFile(bytes, "test.sol");

        // Verify
        Assert.IsTrue(parsedFile.IsValid, parsedFile.ErrorMessage);
        Assert.AreEqual(originalFile.SolName, parsedFile.SolName);
        Assert.AreEqual(originalFile.Version, parsedFile.Version);
        Assert.AreEqual(originalFile.Data.Count, parsedFile.Data.Count);

        // Check individual values
        Assert.AreEqual("Hello", parsedFile.Data["stringValue"].Get<string>());
        Assert.AreEqual(42, parsedFile.Data["intValue"].Get<int>());
        Assert.AreEqual(3.14159, parsedFile.Data["doubleValue"].Get<double>(), 0.00001);
        Assert.AreEqual(true, parsedFile.Data["boolTrue"].Get<bool>());
        Assert.AreEqual(false, parsedFile.Data["boolFalse"].Get<bool>());
        Assert.IsTrue(parsedFile.Data["nullValue"].IsNull());

        // Check object
        var parsedObj = parsedFile.Data["gameData"].Get<SolObject>();
        Assert.IsNotNull(parsedObj);
        Assert.AreEqual("Player", parsedObj!.Properties["name"].Get<string>());
        Assert.AreEqual(1000, parsedObj.Properties["score"].Get<int>());
    }

    [TestMethod]
    public void SolFile_EmptyData_RoundTrip()
    {
        var originalFile = new SolFile
        {
            SolName = "empty",
            Version = SolVersion.AMF0
        };

        var writer = new SolWriter();
        var bytes = writer.WriteFile(originalFile);

        var parser = new SolParser();
        var parsedFile = parser.ParseFile(bytes, "empty.sol");

        Assert.IsTrue(parsedFile.IsValid);
        Assert.AreEqual("empty", parsedFile.SolName);
        Assert.AreEqual(0, parsedFile.Data.Count);
    }

    [TestMethod]
    public void SolParser_InvalidMagic_ReturnsError()
    {
        byte[] invalidData = { 0xFF, 0xFF, 0x00, 0x00 };
        
        var parser = new SolParser();
        var file = parser.ParseFile(invalidData, "invalid.sol");

        Assert.IsFalse(file.IsValid);
        Assert.IsFalse(string.IsNullOrEmpty(file.ErrorMessage));
    }

    [TestMethod]
    public void SolParser_TooSmall_ReturnsError()
    {
        byte[] tooSmall = { 0x00, 0xBF };
        
        var parser = new SolParser();
        var file = parser.ParseFile(tooSmall, "small.sol");

        Assert.IsFalse(file.IsValid);
        Assert.IsFalse(string.IsNullOrEmpty(file.ErrorMessage));
    }
}
