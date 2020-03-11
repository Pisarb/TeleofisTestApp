using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Packets
{
    public interface IPacketWriter<TPacket> : IAsyncDisposable
    {
        #region Methods
        ValueTask WriteAsync(TPacket packet, CancellationToken cancellationToken = default);
        #endregion
    }
}
