using Swsu.StreetLights.Protocols.Teleofis.Packets;
using System;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public class WrxException : Exception
    {
        #region Constructors
        public WrxException(WrxPacketError error)
        {
            Error = error;
        }
        #endregion

        #region Properties
        public WrxPacketError Error
        {
            get;
        }
        #endregion
    }
}
