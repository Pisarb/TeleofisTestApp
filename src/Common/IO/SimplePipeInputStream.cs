using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimplePipeInputStream<T> : SimplePipeStreamBase<T>, IAsyncSimpleInputStream<T>
    {
        #region Constructors
        internal SimplePipeInputStream(SimplePipe<T> pipe) :
            base(pipe)
        {
        }
        #endregion

        #region Methods        
        public ValueTask<int> ReadAsync(Memory<T> buffer, CancellationToken cancellationToken = default)
        {
            return Pipe.ReadInputAsync(buffer, cancellationToken);
        }
        #endregion
    }
}
