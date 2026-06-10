using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
    public enum PrefixStyle
    {
        None = 0,
        Base128 = 1,
        Fixed32 = 2,
        Fixed32BigEndian = 3
    }

    public static class Serializer
    {
        public static void Serialize<T>(Stream stream, T instance) where T : LightProto.IProtoParser<T>
        {
            LightProto.Serializer.Serialize(stream, instance);
        }

        public static void Serialize<T>(Stream stream, IEnumerable<T> instance) where T : LightProto.IProtoParser<T>
        {
            if (instance == null) return;
            foreach (var item in instance)
            {
                LightProto.Serializer.SerializeWithLengthPrefix(stream, item, LightProto.PrefixStyle.Base128, 1);
            }
        }

        public static T Deserialize<T>(Stream stream) where T : LightProto.IProtoParser<T>
        {
            return LightProto.Serializer.Deserialize<T>(stream);
        }

        public static T Deserialize<T>(Stream stream, int length) where T : LightProto.IProtoParser<T>
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0) return Activator.CreateInstance<T>();
            
            // Slice the stream by reading `length` bytes into a temporary MemoryStream
            // This is necessary because LightProto.Serializer.Deserialize reads until the end of the stream/segment.
            byte[] buffer = new byte[length];
            int totalRead = 0;
            while (totalRead < length)
            {
                int read = stream.Read(buffer, totalRead, length - totalRead);
                if (read <= 0) break;
                totalRead += read;
            }
            
            using var ms = new MemoryStream(buffer, 0, totalRead);
            return LightProto.Serializer.Deserialize<T>(ms);
        }

        public static T Deserialize<T>(Stream stream, uint length) where T : LightProto.IProtoParser<T>
        {
            return Deserialize<T>(stream, (int)length);
        }

        public static IEnumerable<T> DeserializeItems<T>(Stream stream, PrefixStyle style, int fieldNumber) where T : LightProto.IProtoParser<T>
        {
            return LightProto.Serializer.DeserializeItems<T>(stream, (LightProto.PrefixStyle)style, fieldNumber);
        }
    }

    public struct DiscriminatedUnion32
    {
        private readonly int tag;
        private readonly uint value;

        public DiscriminatedUnion32(int tag, uint value)
        {
            this.tag = tag;
            this.value = value;
        }

        public DiscriminatedUnion32(int tag, int value)
        {
            this.tag = tag;
            this.value = (uint)value;
        }

        public DiscriminatedUnion32(int tag, float value)
        {
            this.tag = tag;
            this.value = BitConverter.SingleToUInt32Bits(value);
        }

        public readonly bool Is(int tag) => this.tag == tag;

        public readonly uint UInt32 => value;
        public readonly int Int32 => (int)value;
        public readonly float Single => BitConverter.UInt32BitsToSingle(value);

        public static void Reset(ref DiscriminatedUnion32 union, int tag)
        {
            if (union.tag == tag)
            {
                union = default;
            }
        }
    }

    public struct DiscriminatedUnion32Object
    {
        private readonly int tag;
        private readonly uint valueUInt32;
        private readonly object valueObject;

        public DiscriminatedUnion32Object(int tag, uint value)
        {
            this.tag = tag;
            this.valueUInt32 = value;
            this.valueObject = null!;
        }

        public DiscriminatedUnion32Object(int tag, int value)
        {
            this.tag = tag;
            this.valueUInt32 = (uint)value;
            this.valueObject = null!;
        }

        public DiscriminatedUnion32Object(int tag, float value)
        {
            this.tag = tag;
            this.valueUInt32 = BitConverter.SingleToUInt32Bits(value);
            this.valueObject = null!;
        }

        public DiscriminatedUnion32Object(int tag, bool value)
        {
            this.tag = tag;
            this.valueUInt32 = value ? 1u : 0u;
            this.valueObject = null!;
        }

        public DiscriminatedUnion32Object(int tag, object value)
        {
            this.tag = tag;
            this.valueUInt32 = 0;
            this.valueObject = value;
        }

        public readonly bool Is(int tag) => this.tag == tag;

        public readonly uint UInt32 => valueUInt32;
        public readonly int Int32 => (int)valueUInt32;
        public readonly float Single => BitConverter.UInt32BitsToSingle(valueUInt32);
        public readonly bool Boolean => valueUInt32 != 0;
        public readonly object Object => valueObject;

        public static void Reset(ref DiscriminatedUnion32Object union, int tag)
        {
            if (union.tag == tag)
            {
                union = default;
            }
        }
    }

    public struct DiscriminatedUnionObject
    {
        private readonly int tag;
        private readonly object valueObject;

        public DiscriminatedUnionObject(int tag, object value)
        {
            this.tag = tag;
            this.valueObject = value;
        }

        public readonly bool Is(int tag) => this.tag == tag;

        public readonly object Object => valueObject;

        public static void Reset(ref DiscriminatedUnionObject union, int tag)
        {
            if (union.tag == tag)
            {
                union = default;
            }
        }
    }

    [Obsolete("compatibility protobuf-net only, no effect.", false)]
    public interface IExtensible : LightProto.IExtensible {}

    [Obsolete("compatibility protobuf-net only, no effect.", false)]
    public interface IExtension : LightProto.IExtension {}

    [Obsolete("compatibility protobuf-net only, no effect.", false)]
    public static class Extensible
    {
        public static LightProto.IExtension GetExtensionObject(ref LightProto.IExtension extensionObject, bool createIfMissing)
        {
            return LightProto.Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }

    [Serializable]
    public class ProtoException : Exception
    {
        public ProtoException() { }
        public ProtoException(string message) : base(message) { }
        public ProtoException(string message, Exception innerException) : base(message, innerException) { }
    }

    public struct SteamBytes : LightProto.IProtoParser<SteamBytes>
    {
        public byte[] Value;

        public static implicit operator byte[](SteamBytes b) => b.Value;
        public static implicit operator SteamBytes(byte[] b) => new SteamBytes { Value = b };

        public static LightProto.IProtoReader<SteamBytes> ProtoReader => Reader.Instance;
        public static LightProto.IProtoWriter<SteamBytes> ProtoWriter => Writer.Instance;

        private class Reader : LightProto.IProtoReader<SteamBytes>
        {
            public static readonly Reader Instance = new();
            public LightProto.WireFormat.WireType WireType => LightProto.WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;
            public SteamBytes ParseFrom(ref LightProto.ReaderContext input)
            {
                return new SteamBytes { Value = global::LightProto.Parser.ByteArrayProtoParser.ProtoReader.ParseMessageFrom(ref input) };
            }
        }

        private class Writer : LightProto.IProtoWriter<SteamBytes>
        {
            public static readonly Writer Instance = new();
            public LightProto.WireFormat.WireType WireType => LightProto.WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;
            public int CalculateSize(SteamBytes value)
            {
                return (int)CalculateLongSize(value);
            }
            public long CalculateLongSize(SteamBytes value)
            {
                if (value.Value == null) return 0;
                return global::LightProto.Parser.ByteArrayProtoParser.ProtoWriter.CalculateLongMessageSize(value.Value);
            }
            public void WriteTo(ref LightProto.WriterContext output, SteamBytes value)
            {
                if (value.Value != null)
                {
                    global::LightProto.Parser.ByteArrayProtoParser.ProtoWriter.WriteMessageTo(ref output, value.Value);
                }
            }
        }
    }
}

