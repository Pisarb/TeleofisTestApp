using System;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common
{
    public class AsyncDisposableBase : IAsyncDisposable
    {
        #region Fields
        private readonly Func<object?, ValueTask>? _onDisposedAsync;

        private readonly object? _onDisposedState;
        #endregion

        #region Constructors
        protected AsyncDisposableBase(Func<object?, ValueTask>? onDisposedAsync = default, object? onDisposedState = default)
        {
            _onDisposedAsync = onDisposedAsync;
            _onDisposedState = onDisposedState;
        }
        #endregion

        #region Methods
        public ValueTask DisposeAsync()
        {
            return DisposeCoreAsync();
        }

        protected virtual async ValueTask DisposeCoreAsync()
        {
            if (_onDisposedAsync != null)
            {
                await _onDisposedAsync(_onDisposedState).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
