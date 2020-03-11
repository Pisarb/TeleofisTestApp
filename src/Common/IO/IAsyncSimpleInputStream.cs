using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    /// <summary>
    /// Упрощенный и обобщенный асинхронный поток данных, допускающий только последовательное чтение.
    /// </summary>
    /// <typeparam name="T">Элемент потока.</typeparam>
    public interface IAsyncSimpleInputStream<T> : IAsyncDisposable
    {
        #region Methods
        ValueTask<int> ReadAsync(Memory<T> buffer, CancellationToken cancellationToken = default);
        #endregion
    }
}
