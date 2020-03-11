using Swsu.StreetLights.Common.Packets;
using Swsu.StreetLights.Protocols.Teleofis.Packets;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public static class WrxTcpDeviceAuthorization
    {
        #region Methods
        public static ValueTask AuthorizeAsync(IPacketReader<WrxPacket> requestReader, IPacketWriter<WrxPacket> responseWriter, IAsyncWrxTcpDeviceAuthorizer authorizer, CancellationToken cancellationToken = default)
        {
            return AuthorizeAsync(requestReader, responseWriter, authorizer, ArrayPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask AuthorizeAsync(IPacketReader<WrxPacket> requestReader, IPacketWriter<WrxPacket> responseWriter, IAsyncWrxTcpDeviceAuthorizer authorizer, ArrayPool<byte> byteArrayPool, CancellationToken cancellationToken = default)
        {
            using var requestPacket = await requestReader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var (protocol, requestAction, requestCommand, _) = requestPacket;

            if (requestAction != WrxPacketAction.TcpAuthorizationRequest)
            {
                throw new Exception();
            }

            if (requestCommand != 0)
            {
                throw new Exception();
            }

            var byteArray = byteArrayPool.Rent(WrxTcpAuthorizationData.Size);

            try
            {
                var bytes = byteArray.AsMemory();
                (await authorizer.ProvideDataAsync(cancellationToken).ConfigureAwait(false)).Write(bytes.Span);
                using var responsePacket = new WrxPacket(protocol, WrxPacketAction.TcpAuthorizationResponse, 0, bytes[..WrxTcpAuthorizationData.Size]);
                await responseWriter.WriteAsync(responsePacket, cancellationToken).ConfigureAwait(false);

                using var acknowledgementPacket = await requestReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var (_, acknowledgementAction, acknowledgementCommand, _) = acknowledgementPacket;

                if (acknowledgementAction != WrxPacketAction.TcpAuthorizationRequest)
                {
                    throw new Exception();
                }

                if (acknowledgementCommand != 1)
                {
                    throw new Exception();
                }
            }
            finally
            {
                byteArrayPool.Return(byteArray);
            }
        }
        #endregion
    }
}
