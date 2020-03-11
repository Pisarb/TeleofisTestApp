using Swsu.StreetLights.Common.IO;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis.Packets
{
    public static class WrxByteStuffing
    {
        #region Methods
        public static async ValueTask ReadPacketAsync(
            IAsyncSimpleInputStream<byte> stream,
            Func<IAsyncSimpleInputStream<byte>, CancellationToken, ValueTask> callback,
            CancellationToken cancellationToken = default)
        {
            await PreReadAsync(stream, cancellationToken).ConfigureAwait(false);
            await using var packetStream = new PacketInputStream(stream);

            try
            {
                await callback(packetStream, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await PostReadAsync(packetStream, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async ValueTask ReadPacketAsync<TState>(
            IAsyncSimpleInputStream<byte> stream,
            Func<IAsyncSimpleInputStream<byte>, TState, CancellationToken, ValueTask> callback,
            TState state,
            CancellationToken cancellationToken = default)
        {
            await PreReadAsync(stream, cancellationToken).ConfigureAwait(false);
            await using var packetStream = new PacketInputStream(stream);

            try
            {
                await callback(packetStream, state, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await PostReadAsync(packetStream, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async ValueTask<TResult> ReadPacketAsync<TResult>(
            IAsyncSimpleInputStream<byte> stream,
            Func<IAsyncSimpleInputStream<byte>, CancellationToken, ValueTask<TResult>> callback,
            CancellationToken cancellationToken = default)
        {
            await PreReadAsync(stream, cancellationToken).ConfigureAwait(false);
            await using var packetStream = new PacketInputStream(stream);

            try
            {
                return await callback(packetStream, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await PostReadAsync(packetStream, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async ValueTask<TResult> ReadPacketAsync<TResult, TState>(
            IAsyncSimpleInputStream<byte> stream,
            Func<IAsyncSimpleInputStream<byte>, TState, CancellationToken, ValueTask<TResult>> callback,
            TState state,
            CancellationToken cancellationToken = default)
        {
            await PreReadAsync(stream, cancellationToken).ConfigureAwait(false);
            await using var packetStream = new PacketInputStream(stream);

            try
            {
                return await callback(packetStream, state, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await PostReadAsync(packetStream, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async ValueTask WritePacketAsync<TState>(
            IAsyncSimpleOutputStream<byte> stream,
            Func<IAsyncSimpleOutputStream<byte>, TState, CancellationToken, ValueTask> callback,
            TState state,
            CancellationToken cancellationToken = default)
        {
            await PreWriteAsync(stream, cancellationToken).ConfigureAwait(false);
            await using var packetStream = new PacketOutputStream(stream);

            try
            {
                await callback(packetStream, state, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await PostWriteAsync(stream, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async ValueTask PostReadAsync(PacketInputStream packetStream, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(256);

            try
            {
                while (await packetStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false) != 0)
                {
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private static async ValueTask PostWriteAsync(IAsyncSimpleOutputStream<byte> stream, CancellationToken cancellationToken)
        {
            await stream.WriteByteAsync(0xC2, cancellationToken);
        }

        private static async ValueTask PreReadAsync(IAsyncSimpleInputStream<byte> stream, CancellationToken cancellationToken)
        {
            if (await stream.ReadByteAsync(cancellationToken) != 0xC0)
            {
                throw new Exception();
            }
        }

        private static async ValueTask PreWriteAsync(IAsyncSimpleOutputStream<byte> stream, CancellationToken cancellationToken)
        {
            await stream.WriteByteAsync(0xC0, cancellationToken);
        }
        #endregion

        #region Nested Types
        private class PacketInputStream : IAsyncSimpleInputStream<byte>
        {
            #region Fields
            private readonly IAsyncSimpleInputStream<byte> _base;

            private bool _atEnd = false;
            #endregion

            #region Constructors
            internal PacketInputStream(IAsyncSimpleInputStream<byte> @base)
            {
                _base = @base;
            }
            #endregion

            #region Methods
            public ValueTask DisposeAsync()
            {
                return default;
            }

            public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                var index = 0;

                while (!_atEnd && index < buffer.Length)
                {
                    var value = await _base.ReadByteAsync(cancellationToken).ConfigureAwait(false);

                    switch (value)
                    {
                        case 0xC0:
                            throw new Exception();

                        case 0xC2:
                            _atEnd = true;
                            continue;

                        case 0xC4:
                            value = await _base.ReadByteAsync(cancellationToken).ConfigureAwait(false) switch
                            {
                                0xC1 => 0xC0,
                                0xC3 => 0xC2,
                                0xC4 => 0xC4,
                                _ => throw new Exception(),
                            };
                            break;
                    }

                    buffer.Span[index++] = value;
                }

                return index;
            }
            #endregion
        }

        private class PacketOutputStream : IAsyncSimpleOutputStream<byte>
        {
            #region Fields
            private readonly IAsyncSimpleOutputStream<byte> _base;
            #endregion

            #region Constructors
            internal PacketOutputStream(IAsyncSimpleOutputStream<byte> @base)
            {
                _base = @base;
            }
            #endregion

            #region Methods
            public ValueTask DisposeAsync()
            {
                return default;
            }

            public async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                var index = 0;

                while (index < buffer.Length)
                {
                    var value = buffer.Span[index++];

                    switch (value)
                    {
                        case 0xC0:
                            await _base.WriteByteAsync(0xC4, cancellationToken).ConfigureAwait(false);
                            value = 0xC1;
                            break;

                        case 0xC2:
                            await _base.WriteByteAsync(0xC4, cancellationToken).ConfigureAwait(false);
                            value = 0xC3;
                            break;

                        case 0xC4:
                            await _base.WriteByteAsync(0xC4, cancellationToken).ConfigureAwait(false);
                            value = 0xC4;
                            break;
                    }

                    await _base.WriteByteAsync(value, cancellationToken).ConfigureAwait(false);
                }

                return index;
            }
            #endregion
        }
        #endregion
    }
}
