namespace Swsu.StreetLights.Common
{
    public static class BitHelpers
    {
        #region Methods
        public static byte ChangeBitOrder(byte value)
        {
            // Найдено на Stack Overflow.

            value = (byte)((value & 0xF0) >> 4 | (value & 0x0F) << 4);
            value = (byte)((value & 0xCC) >> 2 | (value & 0x33) << 2);
            value = (byte)((value & 0xAA) >> 1 | (value & 0x55) << 1);

            return value;
        }

        public static ushort ChangeBitOrder(ushort value)
        {
            // Найдено на Stack Overflow.

            value = (ushort)((value & 0xFF00) >> 8 | (value & 0x00FF) << 8);
            value = (ushort)((value & 0xF0F0) >> 4 | (value & 0x0F0F) << 4);
            value = (ushort)((value & 0xCCCC) >> 2 | (value & 0x3333) << 2);
            value = (ushort)((value & 0xAAAA) >> 1 | (value & 0x5555) << 1);

            return value;
        }
        #endregion
    }
}
