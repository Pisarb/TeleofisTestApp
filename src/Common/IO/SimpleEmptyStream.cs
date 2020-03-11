using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleEmptyStream<T> : IAsyncSimpleStream<T>
    {
        #region Fields
        public static readonly SimpleEmptyStream<T> Instance = new SimpleEmptyStream<T>();
        #endregion

        #region Constructors
        private SimpleEmptyStream()
        {
        }
        #endregion

        #region Methods
        public ValueTask DisposeAsync()
        {
            return default;
        }

        public ValueTask<int> ReadAsync(Memory<T> buffer, CancellationToken cancellationToken = default)
        {
            return new ValueTask<int>(0);
        }

        public ValueTask<int> WriteAsync(ReadOnlyMemory<T> buffer, CancellationToken cancellationToken = default)
        {
            return new ValueTask<int>(0);
        }
        #endregion
    }
}
