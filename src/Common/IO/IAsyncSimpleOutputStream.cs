using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    /// <summary>
    /// Упрощенный и обобщенный асинхронный поток данных, допускающий только последовательную запись.
    /// </summary>
    /// <typeparam name="T">Элемент потока.</typeparam>
    public interface IAsyncSimpleOutputStream<T> : IAsyncDisposable
    {
        #region Methods
        ValueTask<int> WriteAsync(ReadOnlyMemory<T> buffer, CancellationToken cancellationToken = default);
        #endregion
    }
}
