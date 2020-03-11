using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Packets
{
    /// <summary>
    /// Читает пакеты из памяти. В основном предназначен для тестирования.
    /// </summary>
    /// <typeparam name="T">Тип пакета.</typeparam>
    public class MemoryPacketReader<T> : AsyncDisposableBase, IPacketReader<T>
    {
        #region Constructors
        public MemoryPacketReader(ReadOnlyMemory<T> memory, Func<object?, ValueTask>? onDisposedAsync = default, object? onDisposedState = default) :
            base(onDisposedAsync, onDisposedState)
        {
            Memory = memory;
            Position = 0;
        }
        #endregion

        #region Properties
        public ReadOnlyMemory<T> Memory
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
        public ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Check if already disposed.

            if (Position >= Memory.Length)
            {
                throw new Exception();
            }

            var packet = Memory.Span[Position++];
            return new ValueTask<T>(packet);
        }
        #endregion
    }
}
