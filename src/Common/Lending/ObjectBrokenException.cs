using System;

namespace Swsu.StreetLights.Common.Lending
{
    public class ObjectBrokenException : Exception
    {
        #region Constructors
        public ObjectBrokenException() :
            base("Object is broken.")
        {
        }

        public ObjectBrokenException(Exception innerException) :
            base("Object is broken.", innerException)
        {
        }
        #endregion
    }
}
