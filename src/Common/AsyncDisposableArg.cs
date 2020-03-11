using System;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common
{
    public static class AsyncDisposableArg
    {
        #region Methods
        public static AsyncDisposableArg<T> Create<T>(T value, bool doNotDisposeValue = false)
            where T : IAsyncDisposable
        {
            return new AsyncDisposableArg<T>(value, doNotDisposeValue);
        }
        #endregion
    }

    public readonly struct AsyncDisposableArg<T> : IAsyncDisposable
        where T : IAsyncDisposable
    {
        #region Constructors
        public AsyncDisposableArg(T value, bool doNotDisposeValue = false)
        {
            Value = value;
            DoNotDisposeValue = doNotDisposeValue;
        }
        #endregion

        #region Properties
        public T Value
        {
            get;
        }

        public bool DoNotDisposeValue
        {
            get;
        }
        #endregion

        #region Methods
        public void Deconstruct(out T value, out bool doNotDisposeValue)
        {
            value = Value;
            doNotDisposeValue = DoNotDisposeValue;
        }

        public async ValueTask DisposeAsync()
        {
            if (!DoNotDisposeValue)
            {
                await Value.DisposeAsync();
            }
        }
        #endregion
    }
}
