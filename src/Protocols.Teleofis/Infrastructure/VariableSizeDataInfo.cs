using System;
using System.Text;

namespace Swsu.StreetLights.Protocols.Teleofis.Infrastructure
{
    public static class VariableSizeDataInfo
    {
        #region Methods
        public static VariableSizeDataInfo<string> String(int maxLength)
        {
            return String(maxLength, Encoding.ASCII);
        }

        public static VariableSizeDataInfo<string> String(int maxLength, Encoding encoding)
        {
            return new VariableSizeDataInfo<string>(
                maxLength,
                (buffer) => encoding.GetString(buffer),
                (buffer, value) => encoding.GetBytes(value, buffer));
        }
        #endregion

        #region Methods
        public static VariableSizeDataInfo<T> FromFixed<T>(IFixedSizeDataInfo<T> source)
        {
            return new VariableSizeDataInfo<T>(
                source.SizeInBytes,
                (buffer) =>
                {
                    return source.Read(buffer);
                },
                (buffer, value) =>
                {
                    source.Write(buffer, value);
                    return source.SizeInBytes;
                });
        }
        #endregion

        #region Nested Types
        private delegate T ReadFixedCallback<T>(ReadOnlySpan<byte> buffer);

        private delegate void WriteFixedCallback<T>(Span<byte> buffer, T value);
        #endregion
    }

    public class VariableSizeDataInfo<T> : IVariableSizeDataInfo<T>
    {
        #region Fields
        private readonly ReadCallback _onRead;

        private readonly WriteCallback _onWrite;
        #endregion

        #region Constructors
        public VariableSizeDataInfo(int maximumSizeInBytes, ReadCallback onRead, WriteCallback onWrite)
        {
            MaximumSizeInBytes = maximumSizeInBytes;
            _onRead = onRead;
            _onWrite = onWrite;
        }
        #endregion

        #region Properties
        public int MaximumSizeInBytes
        {
            get;
        }
        #endregion

        #region Methods
        public T Read(ReadOnlySpan<byte> buffer)
        {
            return _onRead(buffer);
        }

        public int Write(Span<byte> buffer, T value)
        {
            return _onWrite(buffer, value);
        }
        #endregion

        #region Nested Types
        public delegate T ReadCallback(ReadOnlySpan<byte> buffer);

        public delegate int WriteCallback(Span<byte> buffer, T value);
        #endregion
    }
}
