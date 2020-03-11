using System;
using System.Threading;

namespace Swsu.StreetLights.Common.Lending
{
    public interface IObjectLending : IDisposable
    {
        #region Properties
        bool IsMarkedAsBroken
        {
            get;
        }

        /// <summary>
        /// Сигнализирует, что доступ к объекту должен быть прекращен.
        /// </summary>
        CancellationToken StoppingToken
        {
            get;
        }
        #endregion

        #region Methods
        void MarkAsBroken(Exception? exception = default);
        #endregion
    }

    public interface IObjectLending<out T> : IObjectLending
    {
        #region Properties
        T Object
        {
            get;
        }
        #endregion
    }
}
