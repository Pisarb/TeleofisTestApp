using System;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common
{
    public abstract class DerivedAsyncDisposable<TBase> : IAsyncDisposable
        where TBase : IAsyncDisposable
    {
        #region Fields
        private readonly bool _leaveBaseUndisposed;
        #endregion

        #region Constructors
        protected DerivedAsyncDisposable(TBase @base, bool leaveBaseUndisposed = false)
        {
            Base = @base ?? throw new ArgumentNullException(nameof(@base));
            _leaveBaseUndisposed = leaveBaseUndisposed;
        }
        #endregion

        #region Properties
        protected TBase Base
        {
            get;
        }
        #endregion

        #region Methods
        public async ValueTask DisposeAsync()
        {
            if (!_leaveBaseUndisposed)
            {
                await Base.DisposeAsync().ConfigureAwait(false);
            }
        }
        #endregion
    }
}
