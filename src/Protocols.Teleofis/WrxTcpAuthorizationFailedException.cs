using System;
using System.Runtime.Serialization;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    [Serializable]
    public class WrxTcpAuthorizationFailedException : Exception
    {
        #region Constructors
        public WrxTcpAuthorizationFailedException()
        {
        }

        public WrxTcpAuthorizationFailedException(string message) :
            base(message)
        {
        }

        public WrxTcpAuthorizationFailedException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        protected WrxTcpAuthorizationFailedException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
        #endregion
    }
}
