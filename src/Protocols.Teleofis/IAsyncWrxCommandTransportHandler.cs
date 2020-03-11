using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public interface IAsyncWrxCommandTransportHandler
    {
        #region Methods
        ValueTask AuthorizeDataAsync(int command, ReadOnlyMemory<byte> dataBuffer, CancellationToken cancellationToken = default);

        ValueTask<int> GetDataAsync(int command, Memory<byte> dataBuffer, CancellationToken cancellationToken = default);

        ValueTask SetDataAsync(int command, ReadOnlyMemory<byte> dataBuffer, CancellationToken cancellationToken = default);
        #endregion
    }
}
