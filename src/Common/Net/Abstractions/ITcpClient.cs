using System;

namespace Swsu.StreetLights.Common.Net.Abstractions
{
    public interface ITcpClient : IDisposable
    {
        #region Methods
        void Close();

        INetworkStream GetStream();
        #endregion
    }
}
