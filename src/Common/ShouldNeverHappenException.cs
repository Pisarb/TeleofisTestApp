using System;
using System.Runtime.Serialization;

namespace Swsu.StreetLights.Common
{
    [Serializable]
    public class ShouldNeverHappenException : Exception
    {
        #region Constructors
        public ShouldNeverHappenException() :
            base("THIS SHOULD NEVER HAPPEN.")
        {
        }

        protected ShouldNeverHappenException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
        #endregion
    }
}
