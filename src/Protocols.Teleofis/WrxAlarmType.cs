namespace Swsu.StreetLights.Protocols.Teleofis
{
    public enum WrxAlarmType : byte
    {
        /// <summary>
        /// Суточное.
        /// </summary>
        Daily = 0,

        /// <summary>
        /// Недельное.
        /// </summary>
        Weekly = 1,

        /// <summary>
        /// Месячное.
        /// </summary>
        Monthly = 2,

        /// <summary>
        /// Отключено.
        /// </summary>
        Off = 3
    }
}
