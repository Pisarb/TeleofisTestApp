using Swsu.StreetLights.Common.Net.Abstractions;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleSocketOutputStream : SimpleSocketStreamBase, IAsyncSimpleOutputStream<byte>
    {
        #region Constructors
        public SimpleSocketOutputStream(DisposableArg<ISocket> socket) :
            base(socket)
        {
        }

        public SimpleSocketOutputStream(ISocket socket, bool doNotDisposeSocket = false) :
            base(socket, doNotDisposeSocket)
        {
        }
        #endregion

        #region Methods        
        public async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return await Socket.SendAsync(buffer, SocketFlags.None, cancellationToken);
        }
        #endregion
    }
}
