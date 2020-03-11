namespace Swsu.StreetLights.Common
{
    public static class Crc16
    {
        #region Fields
        /// <summary>
        /// CRC-16/CCITT-FALSE.
        /// </summary>
        public static readonly CrcParameters<ushort> CcittFalse = new CrcParameters<ushort>(0x1021, 0xFFFF);

        /// <summary>
        /// CRC-16/MODBUS.
        /// </summary>
        public static readonly CrcParameters<ushort> Modbus = new CrcParameters<ushort>(0x8005, 0xFFFF, reflectInput: true, reflectOutput: true);
        #endregion
    }
}
