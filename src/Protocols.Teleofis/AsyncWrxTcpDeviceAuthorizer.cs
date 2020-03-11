using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public class AsyncWrxTcpDeviceAuthorizer : IAsyncWrxTcpDeviceAuthorizer
    {
        #region Fields
        private readonly WrxTcpAuthorizationData _data;
        #endregion

        #region Constructors
        public AsyncWrxTcpDeviceAuthorizer(int counter, string modemImei, string softwareVersion, string hardwareVersion, string key, WrxTcpChannelType channelType)
        {
            _data = new WrxTcpAuthorizationData(counter, modemImei, softwareVersion, hardwareVersion, key, channelType);
        }

        public AsyncWrxTcpDeviceAuthorizer(WrxTcpAuthorizationData data)
        {
            _data = data;
        }
        #endregion

        #region Methods
        public ValueTask<WrxTcpAuthorizationData> ProvideDataAsync(CancellationToken cancellatonToken = default)
        {
            return new ValueTask<WrxTcpAuthorizationData>(_data);
        }
        #endregion
    }
}
