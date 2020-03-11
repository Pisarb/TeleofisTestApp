using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    /// <summary>
    /// Позволяет при ошибке в одном потоке продолжить чтение/запись с использованием другого потока.
    /// </summary>
    /// <typeparam name="T">Тип элемента потока.</typeparam>
    public class SimpleReliableStream<T> : IAsyncSimpleStream<T>
    {
        #region Fields
        private Exception? _exception;

        private readonly Func<Exception?, CancellationToken, ValueTask<AsyncDisposableArg<IAsyncSimpleStream<T>>>> _onRecreate;

        private AsyncDisposableArg<IAsyncSimpleStream<T>>? _recreateResult;
        #endregion

        #region Constructors
        public SimpleReliableStream(
            Func<Exception?, CancellationToken, ValueTask<AsyncDisposableArg<IAsyncSimpleStream<T>>>> onRecreate,
            Exception? exception = default)
        {
            _exception = exception;
            _onRecreate = onRecreate;
        }
        #endregion

        #region Methods
        public async ValueTask DisposeAsync()
        {
            if (_recreateResult.HasValue)
            {
                var (stream, doNotDisposeStream) = _recreateResult.Value;

                if (!doNotDisposeStream)
                {
                    await stream.DisposeAsync();
                }
            }
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

        private async ValueTask<int> Do<TBuffer>(Func<IAsyncSimpleStream<T>, TBuffer, CancellationToken, ValueTask<int>> callback, TBuffer buffer, CancellationToken cancellationToken = default)
        {
            if (!_recreateResult.HasValue)
            {
                goto Recreate;
            }

        Retry:
            try
            {
                var (stream, _) = _recreateResult.Value;
                return await callback(stream, buffer, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                throw;
            }
            catch (Exception ex)
            {
                _recreateResult = null;
                _exception = ex;
            }

        Recreate:
            _recreateResult = await _onRecreate(_exception, cancellationToken).ConfigureAwait(false);
            goto Retry;
        }
        #endregion        
    }
}
