using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    internal class SocketImpl : ISocket
    {
        #region Fields
        private readonly Socket _impl;
        #endregion

        #region Constructors
        internal SocketImpl(Socket impl)
        {
            _impl = impl;
        }
        #endregion

        #region Properties
        public EndPoint LocalEndPoint
        {
            get
            {
                return _impl.LocalEndPoint;
            }
        }

        public EndPoint RemoteEndPoint
        {
            get
            {
                return _impl.RemoteEndPoint;
            }
        }
        #endregion

        #region Methods
        public async ValueTask<ISocket> AcceptAsync(CancellationToken cancellationToken = default)
        {
            return new SocketImpl(await _impl.AcceptAsync(cancellationToken));
        }

        public void Bind(EndPoint localEP)
        {
            _impl.Bind(localEP);
        }

        public ValueTask ConnectAsync(EndPoint remoteEP, CancellationToken cancellationToken = default)
        {
            return _impl.ConnectAsync(remoteEP, cancellationToken);
        }

        public void Dispose()
        {
            _impl.Dispose();
        }

        public void Listen(int backlog)
        {
            _impl.Listen(backlog);
        }

        public ValueTask<int> ReceiveAsync(Memory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default)
        {
            return _impl.ReceiveAsync(buffer, socketFlags, cancellationToken);
        }

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default)
        {
            return _impl.SendAsync(buffer, socketFlags, cancellationToken);
        }
        #endregion
    }
}
