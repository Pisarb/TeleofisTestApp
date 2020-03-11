using System.Net.Sockets;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    public class SocketFactory : ISocketFactory
    {
        #region Fields
        public static readonly ISocketFactory Instance = new SocketFactory();
        #endregion

        #region Constructors
        private SocketFactory()
        {
        }
        #endregion

        #region Methods
        public ISocket Create(SocketType socketType, ProtocolType protocolType)
        {
            return new SocketImpl(new Socket(socketType, protocolType));
        }
        #endregion
    }
}
