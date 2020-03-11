using System.Net.Sockets;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    public interface ISocketFactory
    {
        #region Methods
        ISocket Create(SocketType socketType, ProtocolType protocolType);
        #endregion
    }
}
