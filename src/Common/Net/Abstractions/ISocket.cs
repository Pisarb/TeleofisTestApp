using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    public interface ISocket : IDisposable
    {
        #region Properties
        EndPoint LocalEndPoint
        {
            get;
        }

        EndPoint RemoteEndPoint
        {
            get;
        }
        #endregion

        #region Methods
        /// <remarks>
        /// Метода с такой сигнатурой в классе <see cref="Socket"/> нет, но ожидается,
        /// что его удастся реализовать на основе метода <see cref="Socket.AcceptAsync(SocketAsyncEventArgs)"/>.
        /// </remarks>
        ValueTask<ISocket> AcceptAsync(CancellationToken cancellationToken = default);

        void Bind(EndPoint localEP);

        ValueTask ConnectAsync(EndPoint remoteEP, CancellationToken cancellationToken = default);

        void Listen(int backlog);

        ValueTask<int> ReceiveAsync(Memory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default);

        ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default);
        #endregion
    }
}
