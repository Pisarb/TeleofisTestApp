using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public interface IAsyncWrxTcpDeviceAuthorizer
    {
        #region Methods
        ValueTask<WrxTcpAuthorizationData> ProvideDataAsync(CancellationToken cancellatonToken = default);
        #endregion
    }
}
