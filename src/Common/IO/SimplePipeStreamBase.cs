using System;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public abstract class SimplePipeStreamBase<T> : IAsyncDisposable
    {
        #region Constructors
        private protected SimplePipeStreamBase(SimplePipe<T> pipe)
        {
            Pipe = pipe;
        }
        #endregion

        #region Properties
        public SimplePipe<T> Pipe
        {
            get;
        }
        #endregion

        #region Methods
        public ValueTask DisposeAsync()
        {
            return default;
        }
        #endregion
    }
}
