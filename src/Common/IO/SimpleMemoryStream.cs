using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleMemoryStream<T> : IAsyncSimpleStream<T>
    {
        #region Fields
        private readonly Memory<T> _memory;

        private readonly Func<object?, ValueTask>? _onDisposeAsync;

        private readonly object? _onDisposeState;

        private int _position;
        #endregion

        #region Constructors
        public SimpleMemoryStream(Memory<T> memory, Func<object?, ValueTask>? onDisposeAsync = default, object? onDisposeState = default)
        {
            _memory = memory;
            _onDisposeAsync = onDisposeAsync;
            _onDisposeState = onDisposeState;
            _position = 0;
        }
        #endregion

        #region Methods
        public async ValueTask DisposeAsync()
        {
            if (_onDisposeAsync != null)
            {
                await _onDisposeAsync(_onDisposeState).ConfigureAwait(false);
            }
        }

        public ValueTask<int> ReadAsync(Memory<T> buffer, CancellationToken cancellationToken = default)
        {
            if (buffer.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer));
            }

            var count = Math.Min(buffer.Length, _memory.Length - _position);
            var newPosition = _position + count;

            _memory[_position..newPosition].CopyTo(buffer[..count]);
            _position = newPosition;
            return new ValueTask<int>(count);
        }

        public ValueTask<int> WriteAsync(ReadOnlyMemory<T> buffer, CancellationToken cancellationToken = default)
        {
            if (buffer.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer));
            }

            var count = Math.Min(buffer.Length, _memory.Length - _position);
            var newPosition = _position + count;

            buffer[..count].CopyTo(_memory[_position..newPosition]);
            _position = newPosition;
            return new ValueTask<int>(count);
        }
        #endregion
    }
}
