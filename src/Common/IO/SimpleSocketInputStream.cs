using Swsu.StreetLights.Common.Net.Abstractions;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleSocketInputStream : SimpleSocketStreamBase, IAsyncSimpleInputStream<byte>
    {
        #region Methods
        public SimpleSocketInputStream(DisposableArg<ISocket> socket) :
            base(socket)
        {
        }

        public SimpleSocketInputStream(ISocket socket, bool doNotDisposeSocket = false) :
            base(socket, doNotDisposeSocket)
        {
        }
        #endregion

        #region Methods
        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return await Socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
        }
        #endregion
    }
}
