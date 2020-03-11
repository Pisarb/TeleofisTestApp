using System;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common
{
    public static class DisposableHelpers
    {
        #region Methods
        public static bool Dispose(object? obj)
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
                return true;
            }

            return false;
        }

        public static async ValueTask<bool> DisposeAsync(object? obj)
        {
            if (obj is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }
        #endregion
    }
}
