using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrossPlatformCefFlashBrowser.Sol;

public class SolFile
{
    public required string Name { get; init; }
    public Dictionary<string, SolValue> Values { get; } = new();

    public static SolFile Parse(ReadOnlySpan<byte> data)
    {
        var reader = new SolBinaryReader(data);
        reader.ExpectHeader();

        var name = reader.ReadRequiredString();
        reader.ExpectZero();

        var sol = new SolFile { Name = name };
        while (reader.HasRemaining)
        {
            if (!reader.TryReadString(out var key))
            {
                break;
            }

            if (key.Length == 0 && !reader.HasRemaining)
            {
                break;
            }

            var value = reader.ReadAmf0Value();
            sol.Values[key] = value;
        }

        return sol;
    }

    public byte[] ToBytes()
    {
        var writer = new SolBinaryWriter();
        writer.WriteHeader();
        writer.WriteString(Name);
        writer.WriteByte(0);

        foreach (var kvp in Values)
        {
            writer.WriteString(kvp.Key);
            writer.WriteAmf0Value(kvp.Value);
        }

        return writer.ToArray();
    }
}

public abstract record SolValue
{
    public sealed record Number(double Value) : SolValue;
    public sealed record Boolean(bool Value) : SolValue;
    public sealed record String(string Value) : SolValue;
    public sealed record Object(Dictionary<string, SolValue> Members) : SolValue;
    public sealed record Null() : SolValue;
    public sealed record Undefined() : SolValue;
    public sealed record Reference(ushort Index) : SolValue;
    public sealed record EcmaArray(Dictionary<string, SolValue> Members) : SolValue;
    public sealed record StrictArray(List<SolValue> Items) : SolValue;
    public sealed record Date(DateTimeOffset Value) : SolValue;
};

file readonly ref struct SolBinaryReader
{
    private ReadOnlySpan<byte> _data;
    private int _offset;

    public SolBinaryReader(ReadOnlySpan<byte> data)
    {
        _data = data;
        _offset = 0;
    }

    public bool HasRemaining => _offset < _data.Length;

    public void ExpectHeader()
    {
        var signature = ReadBytes(3);
        if (!signature.SequenceEqual("SOL"u8))
        {
            throw new InvalidDataException("Invalid SOL signature");
        }

        var version = ReadByte();
        if (version != 0)
        {
            throw new InvalidDataException($"Unsupported SOL version {version}");
        }

        _ = ReadUInt32();
    }

    public bool TryReadString(out string value)
    {
        value = string.Empty;
        if (!HasRemaining)
        {
            return false;
        }

        if (_offset + 2 > _data.Length)
        {
            return false;
        }

        var length = BinaryPrimitives.ReadUInt16BigEndian(_data.Slice(_offset, 2));
        if (_offset + 2 + length > _data.Length)
        {
            return false;
        }

        _offset += 2;
        var span = ReadBytes(length);
        value = Encoding.UTF8.GetString(span);
        return true;
    }

    public string ReadRequiredString()
    {
        if (!TryReadString(out var value))
        {
            throw new EndOfStreamException();
        }

        return value;
    }

    public void ExpectZero()
    {
        if (ReadByte() != 0)
        {
            throw new InvalidDataException("Expected zero terminator");
        }
    }

    public SolValue ReadAmf0Value()
    {
        var type = ReadByte();
        return type switch
        {
            0x00 => new SolValue.Number(ReadDouble()),
            0x01 => new SolValue.Boolean(ReadByte() != 0),
            0x02 => new SolValue.String(ReadAmf0String()),
            0x03 => new SolValue.Object(ReadObject()),
            0x05 => new SolValue.Null(),
            0x06 => new SolValue.Undefined(),
            0x07 => new SolValue.Reference(ReadUInt16()),
            0x08 => new SolValue.EcmaArray(ReadEcmaArray()),
            0x0A => new SolValue.StrictArray(ReadStrictArray()),
            0x0B => new SolValue.Date(ReadDate()),
            0x0C => new SolValue.String(ReadLongString()),
            _ => throw new NotSupportedException($"Unsupported AMF0 type 0x{type:X2}")
        };
    }

    private Dictionary<string, SolValue> ReadObject()
    {
        var result = new Dictionary<string, SolValue>();
        while (true)
        {
            var key = ReadAmf0String();
            if (string.IsNullOrEmpty(key))
            {
                var type = ReadByte();
                if (type != 0x09)
                {
                    throw new InvalidDataException("Invalid AMF0 object end marker");
                }

                break;
            }

            var value = ReadAmf0Value();
            result[key] = value;
        }

        return result;
    }

    private Dictionary<string, SolValue> ReadEcmaArray()
    {
        _ = ReadUInt32();
        return ReadObject();
    }

    private List<SolValue> ReadStrictArray()
    {
        var count = ReadUInt32();
        var result = new List<SolValue>((int)count);
        for (var i = 0; i < count; i++)
        {
            result.Add(ReadAmf0Value());
        }

        return result;
    }

    private DateTimeOffset ReadDate()
    {
        var ms = ReadDouble();
        _ = ReadUInt16();
        return DateTimeOffset.FromUnixTimeMilliseconds((long)ms);
    }

    private string ReadAmf0String()
    {
        var length = ReadUInt16();
        if (length == 0 && PeekByte() == 0 && PeekByte(1) == 0)
        {
            _offset += 2;
            return string.Empty;
        }

        var span = ReadBytes(length);
        return Encoding.UTF8.GetString(span);
    }

    private string ReadLongString()
    {
        var length = ReadUInt32();
        var span = ReadBytes((int)length);
        return Encoding.UTF8.GetString(span);
    }

    private double ReadDouble()
    {
        var span = ReadBytes(8);
        var value = BinaryPrimitives.ReadDoubleBigEndian(span);
        return value;
    }

    private byte ReadByte()
    {
        if (_offset >= _data.Length)
        {
            throw new EndOfStreamException();
        }

        return _data[_offset++];
    }

    private byte PeekByte(int offset = 0)
    {
        if (_offset + offset >= _data.Length)
        {
            return 0;
        }

        return _data[_offset + offset];
    }

    private ushort ReadUInt16()
    {
        var span = ReadBytes(2);
        return BinaryPrimitives.ReadUInt16BigEndian(span);
    }

    private uint ReadUInt32()
    {
        var span = ReadBytes(4);
        return BinaryPrimitives.ReadUInt32BigEndian(span);
    }

    private ReadOnlySpan<byte> ReadBytes(int length)
    {
        if (_offset + length > _data.Length)
        {
            throw new EndOfStreamException();
        }

        var slice = _data.Slice(_offset, length);
        _offset += length;
        return slice;
    }
}

file sealed class SolBinaryWriter
{
    private readonly MemoryStream _stream = new();

    public void WriteHeader()
    {
        _stream.Write("SOL"u8);
        _stream.WriteByte(0);
        WriteUInt32(0);
    }

    public void WriteString(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        WriteUInt16((ushort)bytes.Length);
        _stream.Write(bytes);
    }

    public void WriteByte(byte value) => _stream.WriteByte(value);

    public void WriteAmf0Value(SolValue value)
    {
        switch (value)
        {
            case SolValue.Number number:
                WriteByte(0x00);
                WriteDouble(number.Value);
                break;
            case SolValue.Boolean boolean:
                WriteByte(0x01);
                WriteByte(boolean.Value ? (byte)1 : (byte)0);
                break;
            case SolValue.String str:
                WriteStringValue(str.Value);
                break;
            case SolValue.Object obj:
                WriteByte(0x03);
                foreach (var kvp in obj.Members)
                {
                    WriteAmf0String(kvp.Key);
                    WriteAmf0Value(kvp.Value);
                }

                WriteUInt16(0);
                WriteByte(0x09);
                break;
            case SolValue.Null:
                WriteByte(0x05);
                break;
            case SolValue.Undefined:
                WriteByte(0x06);
                break;
            case SolValue.Reference reference:
                WriteByte(0x07);
                WriteUInt16(reference.Index);
                break;
            case SolValue.EcmaArray array:
                WriteByte(0x08);
                WriteUInt32((uint)array.Members.Count);
                foreach (var kvp in array.Members)
                {
                    WriteAmf0String(kvp.Key);
                    WriteAmf0Value(kvp.Value);
                }

                WriteUInt16(0);
                WriteByte(0x09);
                break;
            case SolValue.StrictArray strictArray:
                WriteByte(0x0A);
                WriteUInt32((uint)strictArray.Items.Count);
                foreach (var item in strictArray.Items)
                {
                    WriteAmf0Value(item);
                }

                break;
            case SolValue.Date date:
                WriteByte(0x0B);
                WriteDouble(date.Value.ToUnixTimeMilliseconds());
                WriteUInt16(0);
                break;
            default:
                throw new NotSupportedException($"Unsupported SOL value type {value.GetType().Name}");
        }
    }

    public byte[] ToArray() => _stream.ToArray();

    private void WriteAmf0String(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length > ushort.MaxValue)
        {
            throw new InvalidOperationException("AMF0 short string exceeds maximum length");
        }

        WriteUInt16((ushort)bytes.Length);
        _stream.Write(bytes);
    }

    private void WriteStringValue(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length <= ushort.MaxValue)
        {
            WriteByte(0x02);
            WriteUInt16((ushort)bytes.Length);
            _stream.Write(bytes);
        }
        else
        {
            WriteByte(0x0C);
            WriteUInt32((uint)bytes.Length);
            _stream.Write(bytes);
        }
    }

    private void WriteUInt16(ushort value)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        _stream.Write(buffer);
    }

    private void WriteUInt32(uint value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        _stream.Write(buffer);
    }

    private void WriteDouble(double value)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
        _stream.Write(buffer);
    }
}
