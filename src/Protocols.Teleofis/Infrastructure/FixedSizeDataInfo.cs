using Swsu.StreetLights.Common;
using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Swsu.StreetLights.Protocols.Teleofis.Infrastructure
{
    public static class FixedSizeDataInfo
    {
        #region Fields
        public static readonly FixedSizeDataInfo<byte> Byte = new FixedSizeDataInfo<byte>(
            1,
            (buffer) => buffer[0],
            (buffer, value) => buffer[0] = value);

        public static readonly FixedSizeDataInfo<ushort> UInt16 = new FixedSizeDataInfo<ushort>(
            sizeof(ushort),
            (buffer) => BinaryPrimitives.ReadUInt16LittleEndian(buffer),
            (buffer, value) => BinaryPrimitives.WriteUInt16LittleEndian(buffer, value));

        public static readonly FixedSizeDataInfo<uint> UInt32 = new FixedSizeDataInfo<uint>(
            sizeof(uint),
            (buffer) => BinaryPrimitives.ReadUInt32LittleEndian(buffer),
            (buffer, value) => BinaryPrimitives.WriteUInt32LittleEndian(buffer, value));
        #endregion

        #region Methods        
        public static FixedSizeDataInfo<T> Create<T, TRaw>(FixedSizeDataInfo<TRaw> raw, IConverter<T, TRaw> converter)
        {
            return new FixedSizeDataInfo<T>(
                raw.SizeInBytes,
                (buffer) => converter.ConvertFrom(raw.Read(buffer)),
                (buffer, value) => raw.Write(buffer, converter.ConvertTo(value)));
        }

        public static FixedSizeDataInfo<T> Enum<T>(FixedSizeDataInfo<byte> underlying)
            where T : Enum
        {
            return Enum<T, byte>(underlying);
        }

        public static FixedSizeDataInfo<T> Enum<T, TUnderlying>(FixedSizeDataInfo<TUnderlying> underlying)
            where T : Enum
            where TUnderlying : struct
        {
            return Create(underlying, EnumToUnderlyingConverter<T, TUnderlying>.Instance);
        }

        [Obsolete]
        public static FixedSizeDataInfo<T> RepresentAs<T, TBase>(FixedSizeDataInfo<TBase> @base, Func<TBase, T> fromBase, Func<T, TBase> toBase)
        {
            return new FixedSizeDataInfo<T>(
                @base.SizeInBytes,
                (buffer) => fromBase(@base.Read(buffer)),
                (buffer, value) => @base.Write(buffer, toBase(value)));
        }

        public static FixedSizeDataInfo<(T1, T2)> Tuple<T1, T2>(IFixedSizeDataInfo<T1> item1, IFixedSizeDataInfo<T2> item2)
        {
            var offset1 = item1.SizeInBytes;

            return new FixedSizeDataInfo<(T1, T2)>(
                item1.SizeInBytes + item2.SizeInBytes,
                (buffer) =>
                {
                    var value1 = item1.Read(buffer[..offset1]);
                    var value2 = item2.Read(buffer[offset1..]);
                    return (value1, value2);
                },
                (buffer, value) =>
                {
                    var (value1, value2) = value;
                    item1.Write(buffer[..offset1], value1);
                    item2.Write(buffer[offset1..], value2);
                });
        }
        #endregion
    }

    public class FixedSizeDataInfo<T> : IFixedSizeDataInfo<T>
    {
        #region Fields
        private readonly ReadCallback _onRead;

        private readonly WriteCallback _onWrite;
        #endregion

        #region Constructors
        public FixedSizeDataInfo(int sizeInBytes, ReadCallback onRead, WriteCallback onWrite)
        {
            SizeInBytes = sizeInBytes;
            _onRead = onRead;
            _onWrite = onWrite;
        }
        #endregion

        #region Properties
        public int SizeInBytes
        {
            get;
        }
        #endregion

        #region Methods
        public T Read(ReadOnlySpan<byte> buffer)
        {
            return _onRead(buffer);
        }

        public void Write(Span<byte> buffer, T value)
        {
            _onWrite(buffer, value);
        }
        #endregion

        #region Nested Types
        public delegate T ReadCallback(ReadOnlySpan<byte> buffer);

        public delegate void WriteCallback(Span<byte> buffer, T value);
        #endregion
    }
}
