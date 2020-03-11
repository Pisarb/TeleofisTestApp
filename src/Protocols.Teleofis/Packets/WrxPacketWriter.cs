using Swsu.StreetLights.Common;
using Swsu.StreetLights.Common.IO;
using Swsu.StreetLights.Common.Packets;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis.Packets
{
    public class WrxPacketWriter : DerivedAsyncDisposable<IAsyncSimpleOutputStream<byte>>, IPacketWriter<WrxPacket>
    {
        #region Constructors
        public WrxPacketWriter(IAsyncSimpleOutputStream<byte> @base, bool leaveBaseUndisposed = false) :
            base(@base, leaveBaseUndisposed)
        {
        }
        #endregion

        #region Methods
        public ValueTask WriteAsync(WrxPacket packet, CancellationToken cancellationToken = default)
        {
            return WrxByteStuffing.WritePacketAsync(Base, (s, p, t) => p.WriteAsync(s, t), packet, cancellationToken);
        }
        #endregion
    }
}
