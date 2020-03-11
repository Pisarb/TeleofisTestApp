using System;
using System.Net.Sockets;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    internal class TcpClientImpl : ITcpClient
    {
        #region Fields
        private readonly TcpClient _impl;
        #endregion

        #region Constructors
        internal TcpClientImpl(TcpClient impl)
        {
            _impl = impl;
        }
        #endregion

        #region Methods
        public void Close()
        {
            _impl.Close();
        }

        public void Dispose()
        {
            _impl.Dispose();
        }

        public INetworkStream GetStream()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
