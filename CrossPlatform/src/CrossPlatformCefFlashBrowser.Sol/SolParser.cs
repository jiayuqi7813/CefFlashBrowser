using System.Text;

namespace CrossPlatformCefFlashBrowser.Sol
{
    internal class SolRefTable
    {
        public List<string> StringPool { get; set; } = new();
        public List<SolValue> ObjectPool { get; set; } = new();
        public List<SolClassDef> ClassPool { get; set; } = new();
    }

    public class SolParser
    {
        private static readonly byte[] SOL_MAGIC = { 0x00, 0xBF };
        private static readonly byte[] SOL_CONSTANT = { 0x54, 0x43, 0x53, 0x4F, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 };

        private byte[] _data = Array.Empty<byte>();
        private int _index;
        private int _size;
        private SolRefTable _refTable = new();

        public SolFile ParseFile(byte[] data, string filePath)
        {
            var file = new SolFile { FilePath = filePath };
            _data = data;
            _size = data.Length;
            _index = 0;
            _refTable = new SolRefTable();

            try
            {
                if (_size < 18)
                {
                    file.ErrorMessage = "File too small";
                    return file;
                }

                // Check magic bytes
                if (!CheckBytes(SOL_MAGIC))
                {
                    file.ErrorMessage = "File magic mismatch";
                    return file;
                }

                // Read chunk size
                uint chunkSize = ReadUInt32BigEndian();
                if (chunkSize != _size - 6)
                {
                    file.ErrorMessage = "Chunk size mismatch";
                    return file;
                }

                // Check constant
                if (!CheckBytes(SOL_CONSTANT))
                {
                    file.ErrorMessage = "File constant mismatch";
                    return file;
                }

                // Read SOL name
                file.SolName = ReadString(ReadUInt16BigEndian());

                // Read padding
                ReadByte(); // padding

                // Read version (AMF0 or AMF3)
                file.Version = (SolVersion)ReadUInt32BigEndian();

                // Read data
                while (_index < _size)
                {
                    var key = ReadString(ReadUInt16BigEndian());
                    if (string.IsNullOrEmpty(key))
                        break;

                    var value = ReadValue(_refTable);
                    file.Data[key] = value;
                }

                return file;
            }
            catch (Exception ex)
            {
                file.ErrorMessage = ex.Message;
                return file;
            }
        }

        private bool CheckBytes(byte[] expected)
        {
            if (_index + expected.Length > _size)
                return false;

            for (int i = 0; i < expected.Length; i++)
            {
                if (_data[_index + i] != expected[i])
                    return false;
            }

            _index += expected.Length;
            return true;
        }

        private byte ReadByte()
        {
            if (_index >= _size)
                throw new InvalidDataException("Unexpected end of file");

            return _data[_index++];
        }

        private ushort ReadUInt16BigEndian()
        {
            if (_index + 2 > _size)
                throw new InvalidDataException("Unexpected end of file");

            ushort value = (ushort)((_data[_index] << 8) | _data[_index + 1]);
            _index += 2;
            return value;
        }

        private uint ReadUInt32BigEndian()
        {
            if (_index + 4 > _size)
                throw new InvalidDataException("Unexpected end of file");

            uint value = (uint)((_data[_index] << 24) | (_data[_index + 1] << 16) | 
                               (_data[_index + 2] << 8) | _data[_index + 3]);
            _index += 4;
            return value;
        }

        private int ReadInt32BigEndian()
        {
            return (int)ReadUInt32BigEndian();
        }

        private double ReadDoubleBigEndian()
        {
            if (_index + 8 > _size)
                throw new InvalidDataException("Unexpected end of file");

            // Read as big-endian, reverse for little-endian
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bytes[7 - i] = _data[_index + i];
            }
            _index += 8;

            return BitConverter.ToDouble(bytes, 0);
        }

        private string ReadString(int length)
        {
            if (_index + length > _size)
                throw new InvalidDataException("Unexpected end of file");

            string value = Encoding.UTF8.GetString(_data, _index, length);
            _index += length;
            return value;
        }

        private SolValue ReadValue(SolRefTable refTable)
        {
            var type = (SolType)ReadByte();

            switch (type)
            {
                case SolType.Undefined:
                case SolType.Null:
                    return SolValue.Null();

                case SolType.BooleanFalse:
                    return SolValue.Boolean(false);

                case SolType.BooleanTrue:
                    return SolValue.Boolean(true);

                case SolType.Integer:
                    return SolValue.Integer(ReadInt32BigEndian());

                case SolType.Double:
                case SolType.Date:
                    return new SolValue(type, ReadDoubleBigEndian());

                case SolType.String:
                case SolType.XmlDoc:
                case SolType.Xml:
                    return new SolValue(type, ReadString(ReadUInt16BigEndian()));

                case SolType.Array:
                    return SolValue.Array(ReadArray(refTable));

                case SolType.Object:
                    return SolValue.Object(ReadObject(refTable));

                case SolType.Binary:
                    return ReadBinary(refTable);

                default:
                    throw new NotSupportedException($"Unknown SOL type: {type}");
            }
        }

        private SolArray ReadArray(SolRefTable refTable)
        {
            var array = new SolArray();
            uint count = ReadUInt32BigEndian();

            for (uint i = 0; i < count; i++)
            {
                var key = ReadString(ReadUInt16BigEndian());
                var value = ReadValue(refTable);
                array.Assoc[key] = value;
            }

            return array;
        }

        private SolObject ReadObject(SolRefTable refTable)
        {
            var obj = new SolObject();
            
            while (true)
            {
                var keyLength = ReadUInt16BigEndian();
                if (keyLength == 0)
                {
                    var endMarker = ReadByte();
                    if (endMarker == 0x09) // Object end marker
                        break;
                    throw new InvalidDataException("Expected object end marker");
                }

                var key = ReadString(keyLength);
                var value = ReadValue(refTable);
                obj.Properties[key] = value;
            }

            return obj;
        }

        private SolValue ReadBinary(SolRefTable refTable)
        {
            uint length = ReadUInt32BigEndian();
            
            if (_index + length > _size)
                throw new InvalidDataException("Unexpected end of file");

            byte[] data = new byte[length];
            Array.Copy(_data, _index, data, 0, (int)length);
            _index += (int)length;

            return SolValue.Binary(data);
        }
    }
}
