using System;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public static class MemoryExtensions
    {
        #region Methods
        public static SimpleMemoryStream<T> CreateSimpleStream<T>(this Memory<T> memory, Func<object?, ValueTask>? onDisposeAsync = default, object? onDisposeState = default)
        {
            return new SimpleMemoryStream<T>(memory, onDisposeAsync, onDisposeState);
        }

        public static SimpleReadOnlyMemoryStream<T> CreateSimpleStream<T>(this ReadOnlyMemory<T> memory, Func<object?, ValueTask>? onDisposeAsync = default, object? onDisposeState = default)
        {
            return new SimpleReadOnlyMemoryStream<T>(memory, onDisposeAsync, onDisposeState);
        }
        #endregion
    }
}
