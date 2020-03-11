using System;

namespace Swsu.StreetLights.Common
{
    public static class DisposableArg
    {
        #region Methods
        public static DisposableArg<T> Create<T>(T value, bool doNotDisposeValue = false)
            where T : IDisposable
        {
            return new DisposableArg<T>(value, doNotDisposeValue);
        }
        #endregion
    }

    public readonly struct DisposableArg<T> : IDisposable
        where T : IDisposable
    {
        #region Constructors
        public DisposableArg(T value, bool doNotDisposeValue)
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

        public void Dispose()
        {
            if (!DoNotDisposeValue)
            {
                Value.Dispose();
            }
        }
        #endregion
    }
}
