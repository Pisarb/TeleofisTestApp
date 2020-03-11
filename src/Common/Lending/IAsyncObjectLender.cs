using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Lending
{
    /// <summary>
    /// Предоставляет временный асинхронный доступ к объектам.
    /// </summary>
    /// <typeparam name="T">Тип объекта.</typeparam>
    public interface IAsyncObjectLender<T>
    {
        #region Methods
        ValueTask<IObjectLending<T>> LendAsync(CancellationToken cancellationToken = default);
        #endregion
    }
}
