using System;

namespace Swsu.StreetLights.Protocols.Teleofis.Infrastructure
{
    public interface IFixedSizeDataInfo
    {
        #region Properties
        int SizeInBytes
        {
            get;
        }
        #endregion
    }

    public interface IFixedSizeDataInfo<T> : IFixedSizeDataInfo
    {
        #region Methods
        T Read(ReadOnlySpan<byte> buffer);

        void Write(Span<byte> buffer, T value);
        #endregion
    }
}
