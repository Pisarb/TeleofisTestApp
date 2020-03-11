namespace Swsu.StreetLights.Common
{
    public interface IConverter<T, TOther>
    {
        #region Methods
        T ConvertFrom(TOther value);

        TOther ConvertTo(T value);
        #endregion
    }
}
