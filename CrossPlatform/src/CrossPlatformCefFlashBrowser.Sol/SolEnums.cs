namespace CrossPlatformCefFlashBrowser.Sol
{
    public enum SolType : byte
    {
        Undefined = 0x00,
        Null = 0x01,
        BooleanFalse = 0x02,
        BooleanTrue = 0x03,
        Integer = 0x04,
        Double = 0x05,
        String = 0x06,
        XmlDoc = 0x07,
        Date = 0x08,
        Array = 0x09,
        Object = 0x0A,
        Xml = 0x0B,
        Binary = 0x0C
    }

    public enum AMF0Type : byte
    {
        Number = 0x00,
        Boolean = 0x01,
        String = 0x02,
        Object = 0x03,
        MovieClip = 0x04,
        Null = 0x05,
        Undefined = 0x06,
        Reference = 0x07,
        EcmaArray = 0x08,
        ObjectEnd = 0x09,
        StrictArray = 0x0A,
        Date = 0x0B,
        LongString = 0x0C,
        Unsupported = 0x0D,
        Recordset = 0x0E,
        XMLDoc = 0x0F,
        TypedObject = 0x10
    }

    public enum SolVersion : uint
    {
        AMF0 = 0,
        AMF3 = 3
    }
}
