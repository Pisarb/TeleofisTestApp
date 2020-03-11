namespace Swsu.StreetLights.Common.IO
{
    public static class SimpleStreams
    {
        #region Methods
        public static IAsyncSimpleStream<T> Empty<T>()
        {
            return SimpleEmptyStream<T>.Instance;
        }
        #endregion
    }
}
