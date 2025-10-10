namespace CrossPlatformCefFlashBrowser.Sol
{
    public class SolValue
    {
        public SolType Type { get; set; }
        public object? Value { get; set; }

        public SolValue()
        {
            Type = SolType.Null;
            Value = null;
        }

        public SolValue(SolType type, object? value)
        {
            Type = type;
            Value = value;
        }

        public static SolValue Null() => new SolValue(SolType.Null, null);
        public static SolValue Boolean(bool value) => new SolValue(value ? SolType.BooleanTrue : SolType.BooleanFalse, value);
        public static SolValue Integer(int value) => new SolValue(SolType.Integer, value);
        public static SolValue Double(double value) => new SolValue(SolType.Double, value);
        public static SolValue String(string value) => new SolValue(SolType.String, value);
        public static SolValue Array(SolArray value) => new SolValue(SolType.Array, value);
        public static SolValue Object(SolObject value) => new SolValue(SolType.Object, value);
        public static SolValue Binary(byte[] value) => new SolValue(SolType.Binary, value);

        public T? Get<T>()
        {
            if (Value is T typedValue)
                return typedValue;
            return default;
        }

        public bool IsNull() => Type == SolType.Null || Type == SolType.Undefined;
        public bool IsBoolean() => Type == SolType.BooleanFalse || Type == SolType.BooleanTrue;
        public bool IsInteger() => Type == SolType.Integer;
        public bool IsDouble() => Type == SolType.Double || Type == SolType.Date;
        public bool IsString() => Type == SolType.String || Type == SolType.XmlDoc || Type == SolType.Xml;
        public bool IsArray() => Type == SolType.Array;
        public bool IsObject() => Type == SolType.Object;
        public bool IsBinary() => Type == SolType.Binary;
    }
}
