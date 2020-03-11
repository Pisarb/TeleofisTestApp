using Swsu.StreetLights.Common.Packets;
using Swsu.StreetLights.Protocols.Teleofis.Packets;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public static class WrxTcpHubAuthorization
    {
        #region Fields
        private static readonly byte[] _zeroes = new byte[0x14];
        #endregion

        #region Methods
        /// <exception cref="WrxTcpAuthorizationFailedException">Если авторизация не удалась.</exception>
        public static ValueTask<T> AuthorizeAsync<T>(IPacketWriter<WrxPacket> requestWriter, IPacketReader<WrxPacket> responseReader, IAsyncWrxTcpHubAuthorizer<T> authorizer, CancellationToken cancellationToken = default)
        {
            return AuthorizeAsync(requestWriter, responseReader, authorizer, ArrayPool<byte>.Shared, cancellationToken);
        }

        /// <exception cref="WrxTcpAuthorizationFailedException">Если авторизация не удалась.</exception>
        public static async ValueTask<T> AuthorizeAsync<T>(IPacketWriter<WrxPacket> requestWriter, IPacketReader<WrxPacket> responseReader, IAsyncWrxTcpHubAuthorizer<T> authorizer, ArrayPool<byte> byteArrayPool, CancellationToken cancellationToken = default)
        {
            using var requestPacket = new WrxPacket(0, WrxPacketAction.TcpAuthorizationRequest, 0, _zeroes.AsMemory());
            await requestWriter.WriteAsync(requestPacket, cancellationToken).ConfigureAwait(false);
            using var responsePacket = await responseReader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var (_, responseAction, responseCommand, responseData) = responsePacket;

            if (responseAction != WrxPacketAction.TcpAuthorizationResponse)
            {
                throw new Exception("Invalid action.");
            }

            if (responseCommand != 0)
            {
                throw new Exception("Invalid command.");
            }

            var authorizationData = WrxTcpAuthorizationData.Read(responseData.Span);
            var info = await authorizer.ValidateDataAsync(authorizationData, cancellationToken).ConfigureAwait(false);

            using var acknowledgementPacket = new WrxPacket(1, WrxPacketAction.TcpAuthorizationRequest, 1, _zeroes.AsMemory());
            await requestWriter.WriteAsync(acknowledgementPacket, cancellationToken).ConfigureAwait(false);

            return info;
        }
        #endregion
    }
}
