using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    internal class TcpListenerImpl : ITcpListener
    {
        #region Fields
        private readonly TcpListener _impl;
        #endregion

        #region Constructors
        internal TcpListenerImpl(TcpListener impl)
        {
            _impl = impl;
        }
        #endregion

        #region Methods
        public ITcpClient AcceptTcpClient()
        {
            var impl = _impl.AcceptTcpClient();
            return new TcpClientImpl(impl);
        }

        public async Task<ITcpClient> AcceptTcpClientAsync()
        {
            var impl = await _impl.AcceptTcpClientAsync();
            return new TcpClientImpl(impl);
        }

        public void Start()
        {
            _impl.Start();
        }

        public void Stop()
        {
            _impl.Stop();
        }
        #endregion
    }
}
