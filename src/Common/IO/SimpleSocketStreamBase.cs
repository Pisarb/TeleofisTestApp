using Swsu.StreetLights.Common.Net.Abstractions;
using System;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.IO
{
    public abstract class SimpleSocketStreamBase : IAsyncDisposable, IDisposable
    {
        #region Fields
        private readonly DisposableArg<ISocket> _socket;
        #endregion

        #region Constructors
        protected SimpleSocketStreamBase(ISocket socket, bool doNotDisposeSocket = false) :
            this(DisposableArg.Create(socket, doNotDisposeSocket))
        {
        }

        protected SimpleSocketStreamBase(DisposableArg<ISocket> socket)
        {
            _socket = socket;
        }
        #endregion

        #region Properties
        protected ISocket Socket
        {
            get
            {
                return _socket.Value;
            }
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            _socket.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }
        #endregion
    }
}
