using System.Net;
using System.Net.Sockets;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    public class TcpListenerFactory : ITcpListenerFactory
    {
        #region Fields
        public static readonly ITcpListenerFactory Instance = new TcpListenerFactory();
        #endregion

        #region Constructors
        private TcpListenerFactory()
        {
        }
        #endregion

        #region Methods
        public ITcpListener Create(IPEndPoint localEP)
        {
            return new TcpListenerImpl(new TcpListener(localEP));
        }

        public ITcpListener Create(IPAddress localaddr, int port)
        {
            return new TcpListenerImpl(new TcpListener(localaddr, port));
        }
        #endregion
    }
}
