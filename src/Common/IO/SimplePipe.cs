using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    /// <summary>
    /// Простейший канал без буфера.
    /// </summary>
    /// <typeparam name="T">Тип элемента.</typeparam>
    public class SimplePipe<T>
    {
        #region Fields
        private readonly MatchingQueue<ReadOnlyMemory<T>, Memory<T>, int, int> _rendezvous;
        #endregion

        #region Constructors
        public SimplePipe()
        {
            InputStream = new SimplePipeInputStream<T>(this);
            OutputStream = new SimplePipeOutputStream<T>(this);

            _rendezvous = new MatchingQueue<ReadOnlyMemory<T>, Memory<T>, int, int>(Copy);
        }
        #endregion

        #region Properties
        public SimplePipeInputStream<T> InputStream
        {
            get;
        }

        public SimplePipeOutputStream<T> OutputStream
        {
            get;
        }
        #endregion

        #region Methods
        internal ValueTask<int> ReadInputAsync(Memory<T> buffer, CancellationToken cancellationToken)
        {
            return _rendezvous.MatchAsync(buffer, cancellationToken);
        }

        internal ValueTask<int> WriteOutputAsync(ReadOnlyMemory<T> buffer, CancellationToken cancellationToken)
        {
            return _rendezvous.MatchAsync(buffer, cancellationToken);
        }

        private (int, int) Copy(ReadOnlyMemory<T> outputBuffer, Memory<T> inputBuffer, CancellationToken arg3, CancellationToken arg4)
        {
            var count = Math.Min(outputBuffer.Length, inputBuffer.Length);
            outputBuffer[..count].CopyTo(inputBuffer[..count]);
            return (count, count);
        }
        #endregion
    }
}
