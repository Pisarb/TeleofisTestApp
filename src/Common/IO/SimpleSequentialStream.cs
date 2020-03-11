using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleSequentialStream<T> : IAsyncSimpleStream<T>
    {
        #region Fields
        private AsyncDisposableArg<IAsyncSimpleStream<T>>? _currentStream;

        private readonly IAsyncEnumerator<AsyncDisposableArg<IAsyncSimpleStream<T>>> _nextStreams;
        #endregion

        #region Constructors
        public SimpleSequentialStream(params IAsyncSimpleStream<T>[] streams) :
            this(streams.Select(s => AsyncDisposableArg.Create(s, false)))
        {
        }

        public SimpleSequentialStream(IEnumerable<IAsyncSimpleStream<T>> streams, bool doNotDisposeStreams = false) :
            this(streams.Select(s => AsyncDisposableArg.Create(s, doNotDisposeStreams)))
        {
        }

        public SimpleSequentialStream(IEnumerable<AsyncDisposableArg<IAsyncSimpleStream<T>>> streams) :
            this(AsyncDisposableArg.Create(SimpleStreams.Empty<T>(), false), streams)
        {
        }

        public SimpleSequentialStream(IAsyncEnumerable<IAsyncSimpleStream<T>> streams, bool doNotDisposeStreams = false, CancellationToken cancellationToken = default) :
            this(streams.Select(s => AsyncDisposableArg.Create(s, doNotDisposeStreams)), cancellationToken)
        {
        }

        public SimpleSequentialStream(IAsyncEnumerable<AsyncDisposableArg<IAsyncSimpleStream<T>>> streams, CancellationToken cancellationToken = default) :
            this(AsyncDisposableArg.Create(SimpleStreams.Empty<T>(), false), streams, cancellationToken)
        {
        }

        public SimpleSequentialStream(IAsyncEnumerator<AsyncDisposableArg<IAsyncSimpleStream<T>>> streams) :
            this(AsyncDisposableArg.Create(SimpleStreams.Empty<T>(), false), streams)
        {
        }

        private SimpleSequentialStream(AsyncDisposableArg<IAsyncSimpleStream<T>> initialStream, IEnumerable<AsyncDisposableArg<IAsyncSimpleStream<T>>> nextStreams) :
            this(initialStream, nextStreams.ToAsyncEnumerable().GetAsyncEnumerator(default))
        {
        }

        private SimpleSequentialStream(AsyncDisposableArg<IAsyncSimpleStream<T>> initialStream, IAsyncEnumerable<AsyncDisposableArg<IAsyncSimpleStream<T>>> nextStreams, CancellationToken cancellationToken = default) :
            this(initialStream, nextStreams.GetAsyncEnumerator(cancellationToken))
        {
        }

        private SimpleSequentialStream(AsyncDisposableArg<IAsyncSimpleStream<T>> initialStream, IAsyncEnumerator<AsyncDisposableArg<IAsyncSimpleStream<T>>> nextStreams)
        {
            _currentStream = initialStream;
            _nextStreams = nextStreams;
        }
        #endregion

        #region Methods
        public async ValueTask DisposeAsync()
        {
            if (_currentStream.HasValue)
            {
                await _currentStream.Value.DisposeAsync().ConfigureAwait(false);
            }

            await _nextStreams.DisposeAsync().ConfigureAwait(false);
        }

        public ValueTask<int> ReadAsync(Memory<T> buffer, CancellationToken cancellationToken = default)
        {
            return Do(ReadAsync, buffer, cancellationToken);

            #region Local Functions
            static ValueTask<int> ReadAsync(IAsyncSimpleStream<T> stream, Memory<T> buffer, CancellationToken cancellationToken)
            {
                return stream.ReadAsync(buffer, cancellationToken);
            }
            #endregion
        }

        public ValueTask<int> WriteAsync(ReadOnlyMemory<T> buffer, CancellationToken cancellationToken = default)
        {
            return Do(WriteAsync, buffer, cancellationToken);

            #region Local Functions
            static ValueTask<int> WriteAsync(IAsyncSimpleStream<T> stream, ReadOnlyMemory<T> buffer, CancellationToken cancellationToken)
            {
                return stream.WriteAsync(buffer, cancellationToken);
            }
            #endregion
        }

        private async ValueTask<int> Do<TBuffer>(Func<IAsyncSimpleStream<T>, TBuffer, CancellationToken, ValueTask<int>> callback, TBuffer buffer, CancellationToken cancellationToken)
        {
            if (!_currentStream.HasValue)
            {
                return 0;
            }

        Again:
            var stream = _currentStream.Value;
            var count = await callback(stream.Value, buffer, cancellationToken).ConfigureAwait(false);

            if (count == 0)
            {
                _currentStream = null;

                await stream.DisposeAsync().ConfigureAwait(false);

                if (await _nextStreams.MoveNextAsync().ConfigureAwait(false))
                {
                    _currentStream = _nextStreams.Current;
                    goto Again;
                }

                return 0;
            }

            return count;
        }
        #endregion
    }
}
