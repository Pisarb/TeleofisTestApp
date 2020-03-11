using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public class SimpleCrcInputStream<T> : DerivedAsyncDisposable<IAsyncSimpleInputStream<byte>>,
        IAsyncSimpleInputStream<byte>
    {
        #region Constructors
        public SimpleCrcInputStream(IAsyncSimpleInputStream<byte> @base, CrcParameters<T> crcParameters, bool leaveBaseUndisposed = false) :
            this(@base, crcParameters.CreateCalculator(), leaveBaseUndisposed)
        {
        }

        public SimpleCrcInputStream(IAsyncSimpleInputStream<byte> @base, CrcCalculator<T> crcCalculator, bool leaveBaseUndisposed = false) :
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
        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var count = await Base.ReadAsync(buffer, cancellationToken);
            CrcCalculator.Append(buffer.Span[..count]);
            return count;
        }
        #endregion
    }
}
