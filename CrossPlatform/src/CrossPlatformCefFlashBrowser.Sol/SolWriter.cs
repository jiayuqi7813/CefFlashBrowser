using System.Text;

namespace CrossPlatformCefFlashBrowser.Sol
{
    public class SolWriter
    {
        private static readonly byte[] SOL_MAGIC = { 0x00, 0xBF };
        private static readonly byte[] SOL_CONSTANT = { 0x54, 0x43, 0x53, 0x4F, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 };

        private List<byte> _buffer = new();

        public byte[] WriteFile(SolFile file)
        {
            _buffer = new List<byte>();

            // Write magic
            _buffer.AddRange(SOL_MAGIC);

            // Placeholder for chunk size (will be filled later)
            int chunkSizePos = _buffer.Count;
            WriteUInt32BigEndian(0);

            // Write constant
            _buffer.AddRange(SOL_CONSTANT);

            // Write SOL name
            var solNameBytes = Encoding.UTF8.GetBytes(file.SolName);
            WriteUInt16BigEndian((ushort)solNameBytes.Length);
            _buffer.AddRange(solNameBytes);

            // Write padding
            WriteByte(0);

            // Write version
            WriteUInt32BigEndian((uint)file.Version);

            // Write data
            foreach (var kvp in file.Data)
            {
                var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
                WriteUInt16BigEndian((ushort)keyBytes.Length);
                _buffer.AddRange(keyBytes);
                WriteValue(kvp.Value);
            }

            // Update chunk size
            uint chunkSize = (uint)(_buffer.Count - 6);
            byte[] chunkSizeBytes = new byte[4];
            chunkSizeBytes[0] = (byte)(chunkSize >> 24);
            chunkSizeBytes[1] = (byte)(chunkSize >> 16);
            chunkSizeBytes[2] = (byte)(chunkSize >> 8);
            chunkSizeBytes[3] = (byte)chunkSize;
            _buffer[chunkSizePos] = chunkSizeBytes[0];
            _buffer[chunkSizePos + 1] = chunkSizeBytes[1];
            _buffer[chunkSizePos + 2] = chunkSizeBytes[2];
            _buffer[chunkSizePos + 3] = chunkSizeBytes[3];

            return _buffer.ToArray();
        }

        private void WriteByte(byte value)
        {
            _buffer.Add(value);
        }

        private void WriteUInt16BigEndian(ushort value)
        {
            _buffer.Add((byte)(value >> 8));
            _buffer.Add((byte)value);
        }

        private void WriteUInt32BigEndian(uint value)
        {
            _buffer.Add((byte)(value >> 24));
            _buffer.Add((byte)(value >> 16));
            _buffer.Add((byte)(value >> 8));
            _buffer.Add((byte)value);
        }

        private void WriteInt32BigEndian(int value)
        {
            WriteUInt32BigEndian((uint)value);
        }

        private void WriteDoubleBigEndian(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            
            // Convert to big-endian
            for (int i = 7; i >= 0; i--)
            {
                _buffer.Add(bytes[i]);
            }
        }

        private void WriteValue(SolValue value)
        {
            WriteByte((byte)value.Type);

            switch (value.Type)
            {
                case SolType.Undefined:
                case SolType.Null:
                case SolType.BooleanFalse:
                case SolType.BooleanTrue:
                    // No additional data
                    break;

                case SolType.Integer:
                    WriteInt32BigEndian(value.Get<int>());
                    break;

                case SolType.Double:
                case SolType.Date:
                    WriteDoubleBigEndian(value.Get<double>());
                    break;

                case SolType.String:
                case SolType.XmlDoc:
                case SolType.Xml:
                    var str = value.Get<string>() ?? string.Empty;
                    var strBytes = Encoding.UTF8.GetBytes(str);
                    WriteUInt16BigEndian((ushort)strBytes.Length);
                    _buffer.AddRange(strBytes);
                    break;

                case SolType.Array:
                    WriteArray(value.Get<SolArray>()!);
                    break;

                case SolType.Object:
                    WriteObject(value.Get<SolObject>()!);
                    break;

                case SolType.Binary:
                    var binary = value.Get<byte[]>() ?? Array.Empty<byte>();
                    WriteUInt32BigEndian((uint)binary.Length);
                    _buffer.AddRange(binary);
                    break;

                default:
                    throw new NotSupportedException($"Unknown SOL type: {value.Type}");
            }
        }

        private void WriteArray(SolArray array)
        {
            WriteUInt32BigEndian((uint)array.Assoc.Count);

            foreach (var kvp in array.Assoc)
            {
                var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
                WriteUInt16BigEndian((ushort)keyBytes.Length);
                _buffer.AddRange(keyBytes);
                WriteValue(kvp.Value);
            }
        }

        private void WriteObject(SolObject obj)
        {
            foreach (var kvp in obj.Properties)
            {
                var keyBytes = Encoding.UTF8.GetBytes(kvp.Key);
                WriteUInt16BigEndian((ushort)keyBytes.Length);
                _buffer.AddRange(keyBytes);
                WriteValue(kvp.Value);
            }

            // Write object end marker
            WriteUInt16BigEndian(0);
            WriteByte(0x09);
        }
    }
}
