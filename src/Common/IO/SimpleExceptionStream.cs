using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleExceptionStream<T> : IAsyncSimpleStream<T>
    {
        #region Fields
        private readonly Func<Exception> _exceptionFactory;
        #endregion

        #region Constructors
        public SimpleExceptionStream(Func<Exception> exceptionFactory)
        {
            _exceptionFactory = exceptionFactory;
        }
        #endregion

        #region Methods
        public ValueTask DisposeAsync()
        {
            return default;
        }

        public async ValueTask<int> ReadAsync(Memory<T> buffer, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            throw _exceptionFactory();
        }

        public async ValueTask<int> WriteAsync(ReadOnlyMemory<T> buffer, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            throw _exceptionFactory();
        }
        #endregion
    }
}
