using Swsu.StreetLights.Common;
using Swsu.StreetLights.Common.IO;
using Swsu.StreetLights.Common.Packets;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis.Packets
{
    public class WrxPacketReader : DerivedAsyncDisposable<IAsyncSimpleInputStream<byte>>, IPacketReader<WrxPacket>
    {
        #region Constructors
        public WrxPacketReader(IAsyncSimpleInputStream<byte> @base, bool leaveBaseUndisposed = false) :
            base(@base, leaveBaseUndisposed)
        {
        }
        #endregion

        #region Methods
        public ValueTask<WrxPacket> ReadAsync(CancellationToken cancellationToken = default)
        {
            return WrxByteStuffing.ReadPacketAsync(Base, (s, t) => WrxPacket.ReadAsync(s, t), cancellationToken);
        }
        #endregion
    }
}
