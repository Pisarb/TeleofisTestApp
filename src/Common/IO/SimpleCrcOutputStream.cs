using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleCrcOutputStream<T> : DerivedAsyncDisposable<IAsyncSimpleOutputStream<byte>>,
        IAsyncSimpleOutputStream<byte>
    {
        #region Constructors
        public SimpleCrcOutputStream(IAsyncSimpleOutputStream<byte> @base, CrcParameters<T> crcParameters, bool leaveBaseUndisposed = false) :
            this(@base, crcParameters.CreateCalculator(), leaveBaseUndisposed)
        {
        }

        public SimpleCrcOutputStream(IAsyncSimpleOutputStream<byte> @base, CrcCalculator<T> crcCalculator, bool leaveBaseUndisposed = false) :
            base(@base, leaveBaseUndisposed)
        {
            CrcCalculator = crcCalculator;
        }
        #endregion

        #region Properties
        public T Crc
        {
            get
            {
                return CrcCalculator.Crc;
            }
        }

        public CrcCalculator<T> CrcCalculator
        {
            get;
        }
        #endregion

        #region Methods
        public async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var count = await Base.WriteAsync(buffer, cancellationToken);
            CrcCalculator.Append(buffer.Span[..count]);
            return count;
        }
        #endregion
    }
}
