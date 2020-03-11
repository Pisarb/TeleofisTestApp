using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Packets
{
    /// <summary>
    /// Пишет пакеты в память. В основном предназначен для тестирования.
    /// </summary>
    /// <typeparam name="T">Тип пакета.</typeparam>
    public class MemoryPacketWriter<T> : AsyncDisposableBase, IPacketWriter<T>
    {
        #region Constructors
        public MemoryPacketWriter(Memory<T> memory, Func<object?, ValueTask>? onDisposedAsync = default, object? onDisposedState = default) :
            base(onDisposedAsync, onDisposedState)
        {
            Memory = memory;
            Position = 0;
        }
        #endregion

        #region Properties
        public Memory<T> Memory
        {
            get;
        }

        public int Position
        {
            get;
            private set;
        }
        #endregion

        #region Methods       
        public ValueTask WriteAsync(T packet, CancellationToken cancellationToken = default)
        {
            // TODO: Check if already disposed.

            if (Position >= Memory.Length)
            {
                throw new Exception();
            }

            Memory.Span[Position++] = packet;
            return new ValueTask();
        }
        #endregion
    }
}
