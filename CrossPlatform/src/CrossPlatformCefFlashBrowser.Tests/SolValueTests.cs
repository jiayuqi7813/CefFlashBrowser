using CrossPlatformCefFlashBrowser.Sol;

namespace CrossPlatformCefFlashBrowser.Tests;

[TestClass]
public class SolValueTests
{
    [TestMethod]
    public void SolValue_Null_CreatesNullValue()
    {
        var value = SolValue.Null();
        
        Assert.AreEqual(SolType.Null, value.Type);
        Assert.IsTrue(value.IsNull());
    }

    [TestMethod]
    public void SolValue_Boolean_True_CreatesCorrectValue()
    {
        var value = SolValue.Boolean(true);
        
        Assert.AreEqual(SolType.BooleanTrue, value.Type);
        Assert.IsTrue(value.IsBoolean());
        Assert.AreEqual(true, value.Get<bool>());
    }

    [TestMethod]
    public void SolValue_Boolean_False_CreatesCorrectValue()
    {
        var value = SolValue.Boolean(false);
        
        Assert.AreEqual(SolType.BooleanFalse, value.Type);
        Assert.IsTrue(value.IsBoolean());
        Assert.AreEqual(false, value.Get<bool>());
    }

    [TestMethod]
    public void SolValue_Integer_CreatesCorrectValue()
    {
        var value = SolValue.Integer(42);
        
        Assert.AreEqual(SolType.Integer, value.Type);
        Assert.IsTrue(value.IsInteger());
        Assert.AreEqual(42, value.Get<int>());
    }

    [TestMethod]
    public void SolValue_Double_CreatesCorrectValue()
    {
        var value = SolValue.Double(3.14159);
        
        Assert.AreEqual(SolType.Double, value.Type);
        Assert.IsTrue(value.IsDouble());
        Assert.AreEqual(3.14159, value.Get<double>(), 0.00001);
    }

    [TestMethod]
    public void SolValue_String_CreatesCorrectValue()
    {
        var value = SolValue.String("Hello, World!");
        
        Assert.AreEqual(SolType.String, value.Type);
        Assert.IsTrue(value.IsString());
        Assert.AreEqual("Hello, World!", value.Get<string>());
    }

    [TestMethod]
    public void SolValue_Array_CreatesCorrectValue()
    {
        var array = new SolArray();
        array.Assoc["key"] = SolValue.Integer(123);
        
        var value = SolValue.Array(array);
        
        Assert.AreEqual(SolType.Array, value.Type);
        Assert.IsTrue(value.IsArray());
        Assert.IsNotNull(value.Get<SolArray>());
    }

    [TestMethod]
    public void SolValue_Object_CreatesCorrectValue()
    {
        var obj = new SolObject();
        obj.Properties["property"] = SolValue.String("value");
        
        var value = SolValue.Object(obj);
        
        Assert.AreEqual(SolType.Object, value.Type);
        Assert.IsTrue(value.IsObject());
        Assert.IsNotNull(value.Get<SolObject>());
    }

    [TestMethod]
    public void SolValue_Binary_CreatesCorrectValue()
    {
        byte[] data = { 0x01, 0x02, 0x03, 0x04 };
        var value = SolValue.Binary(data);
        
        Assert.AreEqual(SolType.Binary, value.Type);
        Assert.IsTrue(value.IsBinary());
        CollectionAssert.AreEqual(data, value.Get<byte[]>());
    }
}
