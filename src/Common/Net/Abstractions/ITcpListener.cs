using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    public interface ITcpListener
    {
        #region Methods
        ITcpClient AcceptTcpClient();

        Task<ITcpClient> AcceptTcpClientAsync();

        void Start();

        void Stop();
        #endregion
    }
}
