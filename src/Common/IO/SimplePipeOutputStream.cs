using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimplePipeOutputStream<T> : SimplePipeStreamBase<T>, IAsyncSimpleOutputStream<T>
    {
        #region Constructors
        internal SimplePipeOutputStream(SimplePipe<T> pipe) :
            base(pipe)
        {

        }
        #endregion

        #region Methods
        public ValueTask<int> WriteAsync(ReadOnlyMemory<T> buffer, CancellationToken cancellationToken = default)
        {
            return Pipe.WriteOutputAsync(buffer, cancellationToken);
        }
        #endregion
    }
}
