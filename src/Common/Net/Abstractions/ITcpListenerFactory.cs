using System.Net;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    public interface ITcpListenerFactory
    {
        #region Methods
        ITcpListener Create(IPEndPoint localEP);

        ITcpListener Create(IPAddress localaddr, int port);
        #endregion
    }
}
