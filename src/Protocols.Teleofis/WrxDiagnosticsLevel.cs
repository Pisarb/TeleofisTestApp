namespace Swsu.StreetLights.Protocols.Teleofis
{
    public enum WrxDiagnosticsLevel : byte
    {
        /// <summary>
        /// Выключена.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Сообщения.
        /// </summary>
        Messages = 1,

        /// <summary>
        /// AT-команды.
        /// </summary>
        AtCommands = 3,

        /// <summary>
        /// Сообщения и данные.
        /// </summary>
        MessagesAndData = 4
    }
}
