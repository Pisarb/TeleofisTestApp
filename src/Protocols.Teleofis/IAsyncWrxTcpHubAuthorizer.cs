using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public interface IAsyncWrxTcpHubAuthorizer<T>
    {
        #region Methods
        ValueTask<T> ValidateDataAsync(WrxTcpAuthorizationData data, CancellationToken cancellatonToken = default);
        #endregion
    }
}
