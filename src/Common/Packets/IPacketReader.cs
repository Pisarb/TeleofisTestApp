using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Packets
{
    public interface IPacketReader<TPacket> : IAsyncDisposable
    {
        #region Methods
        ValueTask<TPacket> ReadAsync(CancellationToken cancellationToken = default);
        #endregion
    }
}
