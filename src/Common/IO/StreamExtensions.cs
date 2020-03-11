using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public static class StreamExtensions
    {
        #region Methods
        public static async ValueTask<byte> ReadByteAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            return await stream.ReadByteAsync(MemoryPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask<byte> ReadByteAsync(this Stream stream, MemoryPool<byte> bufferPool, CancellationToken cancellationToken = default)
        {
            using var buffer = bufferPool.Rent(1);
            await stream.ReadFullyAsync(buffer.Memory[..1], cancellationToken);
            return buffer.Memory.Span[0];
        }

        public static async ValueTask ReadFullyAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            for (int index = 0, count; index < buffer.Length; index += count)
            {
                count = await stream.ReadAsync(buffer[index..], cancellationToken);

                if (count == 0)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        public static async ValueTask<ushort> ReadUInt16LittleEndianAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            return await stream.ReadUInt16LittleEndianAsync(MemoryPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask<ushort> ReadUInt16LittleEndianAsync(this Stream stream, MemoryPool<byte> bufferPool, CancellationToken cancellationToken = default)
        {
            using var buffer = bufferPool.Rent(sizeof(ushort));
            await stream.ReadFullyAsync(buffer.Memory[..sizeof(ushort)], cancellationToken);
            return BinaryPrimitives.ReadUInt16LittleEndian(buffer.Memory.Span);
        }

        public static async ValueTask WriteByteAsync(this Stream stream, byte value, CancellationToken cancellationToken = default)
        {
            await stream.WriteByteAsync(value, MemoryPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask WriteByteAsync(this Stream stream, byte value, MemoryPool<byte> bufferPool, CancellationToken cancellationToken = default)
        {
            using var buffer = bufferPool.Rent(1);
            buffer.Memory.Span[0] = value;
            await stream.WriteAsync(buffer.Memory[..1], cancellationToken);
        }

        public static async ValueTask WriteUInt16LittleEndianAsync(this Stream stream, ushort value, CancellationToken cancellationToken = default)
        {
            await stream.WriteUInt16LittleEndianAsync(value, MemoryPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask WriteUInt16LittleEndianAsync(this Stream stream, ushort value, MemoryPool<byte> bufferPool, CancellationToken cancellationToken = default)
        {
            using var buffer = bufferPool.Rent(sizeof(ushort));
            BinaryPrimitives.WriteUInt16LittleEndian(buffer.Memory.Span, value);
            await stream.WriteAsync(buffer.Memory[..sizeof(ushort)], cancellationToken);
        }
        #endregion
    }
}
