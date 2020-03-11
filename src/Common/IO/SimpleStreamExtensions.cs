using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public static class SimpleStreamExtensions
    {
        #region Methods
        public static ValueTask<byte> ReadByteAsync(this IAsyncSimpleInputStream<byte> stream, CancellationToken cancellationToken = default)
        {
            return stream.ReadByteAsync(ArrayPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask<byte> ReadByteAsync(this IAsyncSimpleInputStream<byte> stream, ArrayPool<byte> arrayPool, CancellationToken cancellationToken = default)
        {
            var array = arrayPool.Rent(1);

            try
            {
                await stream.ReadFullyAsync(array.AsMemory(..1)).ConfigureAwait(false);
                return array[0];
            }
            finally
            {
                arrayPool.Return(array);
            }
        }

        public static async ValueTask ReadFullyAsync<T>(this IAsyncSimpleInputStream<T> stream, Memory<T> buffer, CancellationToken cancellationToken = default)
        {
            for (int index = 0, count; index < buffer.Length; index += count)
            {
                count = await stream.ReadAsync(buffer[index..], cancellationToken).ConfigureAwait(false);

                if (count == 0)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        public static ValueTask<ushort> ReadUInt16LittleEndianAsync(this IAsyncSimpleInputStream<byte> stream, CancellationToken cancellationToken = default)
        {
            return stream.ReadUInt16LittleEndianAsync(ArrayPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask<ushort> ReadUInt16LittleEndianAsync(this IAsyncSimpleInputStream<byte> stream, ArrayPool<byte> arrayPool, CancellationToken cancellationToken = default)
        {
            var array = arrayPool.Rent(sizeof(ushort));

            try
            {
                await stream.ReadFullyAsync(array.AsMemory(..sizeof(ushort))).ConfigureAwait(false);
                return BinaryPrimitives.ReadUInt16LittleEndian(array);
            }
            finally
            {
                arrayPool.Return(array);
            }
        }

        public static ValueTask WriteByteAsync(this IAsyncSimpleOutputStream<byte> stream, byte value, CancellationToken cancellationToken = default)
        {
            return stream.WriteByteAsync(value, ArrayPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask WriteByteAsync(this IAsyncSimpleOutputStream<byte> stream, byte value, ArrayPool<byte> arrayPool, CancellationToken cancellationToken = default)
        {
            var array = arrayPool.Rent(1);

            try
            {
                array[0] = value;
                await stream.WriteFullyAsync(array.AsMemory(..1)).ConfigureAwait(false);
            }
            finally
            {
                arrayPool.Return(array);
            }
        }

        public static async ValueTask WriteFullyAsync<T>(this IAsyncSimpleOutputStream<T> stream, ReadOnlyMemory<T> buffer, CancellationToken cancellationToken = default)
        {
            for (int index = 0, count; index < buffer.Length; index += count)
            {
                count = await stream.WriteAsync(buffer[index..], cancellationToken).ConfigureAwait(false);

                if (count == 0)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        public static ValueTask WriteUInt16LittleEndianAsync(this IAsyncSimpleOutputStream<byte> stream, ushort value, CancellationToken cancellationToken = default)
        {
            return stream.WriteUInt16LittleEndianAsync(value, ArrayPool<byte>.Shared, cancellationToken);
        }

        public static async ValueTask WriteUInt16LittleEndianAsync(this IAsyncSimpleOutputStream<byte> stream, ushort value, ArrayPool<byte> arrayPool, CancellationToken cancellationToken = default)
        {
            var array = arrayPool.Rent(sizeof(ushort));

            try
            {
                BinaryPrimitives.WriteUInt16LittleEndian(array, value);
                await stream.WriteFullyAsync(array.AsMemory(..sizeof(ushort))).ConfigureAwait(false);
            }
            finally
            {
                arrayPool.Return(array);
            }
        }
        #endregion
    }
}
