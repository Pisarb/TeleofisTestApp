using Swsu.StreetLights.Common;
using Swsu.StreetLights.Common.IO;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis.Packets
{
    public readonly struct WrxPacket : IDisposable
    {
        #region Constructors
        public WrxPacket(int protocol, WrxPacketAction action, int command, ReadOnlyMemory<byte> data = default, Action<object?>? onDispose = default, object? onDisposeState = default)
        {
            Protocol = protocol;
            Action = action;
            Command = command;
            Data = data;
            OnDispose = onDispose;
            OnDisposeState = onDisposeState;
        }
        #endregion

        #region Properties
        public WrxPacketAction Action
        {
            get;
        }

        public int Command
        {
            get;
        }

        public ReadOnlyMemory<byte> Data
        {
            get;
        }

        public int Protocol
        {
            get;
        }

        private Action<object?>? OnDispose
        {
            get;
        }

        private object? OnDisposeState
        {
            get;
        }
        #endregion

        #region Methods       
        public void Deconstruct(out int protocol, out WrxPacketAction action, out int command, out ReadOnlyMemory<byte> data)
        {
            protocol = Protocol;
            action = Action;
            command = Command;
            data = Data;
        }

        public void Dispose()
        {
            OnDispose?.Invoke(OnDisposeState);
        }

        public static async ValueTask<WrxPacket> ReadAsync(IAsyncSimpleInputStream<byte> stream, CancellationToken cancellationToken = default)
        {
            WrxPacket packet;
            ushort crc;

            await using (var crcStream = Crc16.CcittFalse.Wrap(stream, leaveBaseUndisposed: true))
            {
                packet = await crcStream.ReadByteAsync(cancellationToken).ConfigureAwait(false) switch
                {
                    0 => await ReadProtocol0Async(crcStream, cancellationToken).ConfigureAwait(false),
                    1 => await ReadProtocol1Async(crcStream, cancellationToken).ConfigureAwait(false),
                    _ => throw new Exception(),
                };

                crc = crcStream.Crc;
            }

            if (await stream.ReadUInt16LittleEndianAsync(cancellationToken).ConfigureAwait(false) != crc)
            {
                packet.Dispose();
                throw new Exception("CRC verification failed.");
            }

            return packet;
        }

        public async ValueTask WriteAsync(IAsyncSimpleOutputStream<byte> stream, CancellationToken cancellationToken = default)
        {
            ushort crc;

            await using (var crcStream = Crc16.CcittFalse.Wrap(stream, leaveBaseUndisposed: true))
            {
                await crcStream.WriteByteAsync((byte)Protocol, cancellationToken).ConfigureAwait(false);

                switch (Protocol)
                {
                    case 0:
                        await WriteProtocol0Async(crcStream, cancellationToken).ConfigureAwait(false);
                        break;

                    case 1:
                        await WriteProtocol1Async(crcStream, cancellationToken).ConfigureAwait(false);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                crc = crcStream.Crc;
            }

            await stream.WriteUInt16LittleEndianAsync(crc, cancellationToken).ConfigureAwait(false);
        }

        private static async ValueTask<WrxPacket> ReadCommonAsync(int protocol, WrxPacketAction action, int command, int dataLength, IAsyncSimpleInputStream<byte> stream, CancellationToken cancellationToken)
        {
            var data = ArrayPool<byte>.Shared.Rent(dataLength);
            await stream.ReadFullyAsync(data.AsMemory(..dataLength), cancellationToken).ConfigureAwait(false);

            return new WrxPacket(protocol, action, command, data.AsMemory(..dataLength), OnDispose, data);

            #region Local Functions
            static void OnDispose(object? state)
            {
                ArrayPool<byte>.Shared.Return((byte[])state!);
            }
            #endregion
        }

        private static async ValueTask<WrxPacket> ReadProtocol0Async(IAsyncSimpleInputStream<byte> stream, CancellationToken cancellationToken)
        {
            var action = (WrxPacketAction)await stream.ReadByteAsync(cancellationToken);
            var command = (int)await stream.ReadByteAsync(cancellationToken);
            var dataLength = (int)await stream.ReadByteAsync(cancellationToken);

            return await ReadCommonAsync(0, action, command, dataLength, stream, cancellationToken).ConfigureAwait(false);
        }

        private static async ValueTask<WrxPacket> ReadProtocol1Async(IAsyncSimpleInputStream<byte> stream, CancellationToken cancellationToken)
        {
            var action = (WrxPacketAction)await stream.ReadByteAsync(cancellationToken);
            var command = (int)await stream.ReadUInt16LittleEndianAsync(cancellationToken);
            var dataLength = (int)await stream.ReadByteAsync(cancellationToken);

            return await ReadCommonAsync(1, action, command, dataLength, stream, cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask WriteProtocol0Async(IAsyncSimpleOutputStream<byte> stream, CancellationToken cancellationToken)
        {
            await stream.WriteByteAsync((byte)Action, cancellationToken).ConfigureAwait(false);
            await stream.WriteByteAsync((byte)Command, cancellationToken).ConfigureAwait(false);
            await stream.WriteByteAsync((byte)Data.Length, cancellationToken).ConfigureAwait(false);
            await stream.WriteFullyAsync(Data, cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask WriteProtocol1Async(IAsyncSimpleOutputStream<byte> stream, CancellationToken cancellationToken)
        {
            await stream.WriteByteAsync((byte)Action, cancellationToken).ConfigureAwait(false);
            await stream.WriteUInt16LittleEndianAsync((ushort)Command, cancellationToken).ConfigureAwait(false);
            await stream.WriteByteAsync((byte)Data.Length, cancellationToken).ConfigureAwait(false);
            await stream.WriteFullyAsync(Data, cancellationToken).ConfigureAwait(false);
        }
        #endregion
    }
}
