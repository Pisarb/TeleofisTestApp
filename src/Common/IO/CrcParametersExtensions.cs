namespace Swsu.StreetLights.Common.IO
{
    public static class CrcParametersExtensions
    {
        #region Methods
        public static SimpleCrcInputStream<T> Wrap<T>(this CrcParameters<T> parameters, IAsyncSimpleInputStream<byte> @base, bool leaveBaseUndisposed = false)
        {
            return new SimpleCrcInputStream<T>(@base, parameters, leaveBaseUndisposed);
        }

        public static SimpleCrcOutputStream<T> Wrap<T>(this CrcParameters<T> parameters, IAsyncSimpleOutputStream<byte> @base, bool leaveBaseUndisposed = false)
        {
            return new SimpleCrcOutputStream<T>(@base, parameters, leaveBaseUndisposed);
        }
        #endregion
    }
}
