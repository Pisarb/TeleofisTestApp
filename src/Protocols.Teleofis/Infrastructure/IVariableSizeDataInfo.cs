using System;

namespace Swsu.StreetLights.Protocols.Teleofis.Infrastructure
{
    public interface IVariableSizeDataInfo
    {
        #region Properties
        int MaximumSizeInBytes
        {
            get;
        }
        #endregion
    }

    public interface IVariableSizeDataInfo<T> : IVariableSizeDataInfo
    {
        #region Methods
        T Read(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Записывает значение набор байтов.
        /// </summary>
        /// <param name="buffer">Буфер, размером не менее <see cref="IVariableSizeDataInfo.MaximumSizeInBytes"/>.</param>
        /// <param name="value">Значение.</param>
        /// <returns>Использованное количество байтов в <paramref name="buffer"/>.</returns>
        int Write(Span<byte> buffer, T value);
        #endregion
    }
}
