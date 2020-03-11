namespace Swsu.StreetLights.Protocols.Teleofis.Packets
{
    // TODO: Move to Swsu.StreetLights.Protocols.Teleofis namespace.
    public enum WrxPacketError
    {
        /// <summary>
        /// Не выполнена авторизация в приборе.
        /// </summary>
        NotAuthorized = 0x00,

        /// <summary>
        /// Неверный пароль авторизации.
        /// </summary>
        WrongPassword = 0x01,

        /// <summary>
        /// Команда не поддерживается устройством.
        /// </summary>
        CommandNotSupported = 0x02,

        /// <summary>
        /// Неверный формат данных.
        /// </summary>
        InvalidDataFormat = 0x03,

        /// <summary>
        /// Ошибка уровня доступа.
        /// </summary>
        BadAccessLevel = 0x04
    }
}
