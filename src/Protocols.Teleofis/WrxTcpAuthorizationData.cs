using System;
using System.Text;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public readonly struct WrxTcpAuthorizationData : IEquatable<WrxTcpAuthorizationData>
    {
        #region Constants
        public const int Size = 0x3F;
        #endregion

        #region Constructors
        public WrxTcpAuthorizationData(int counter, string modemImei, string softwareVersion, string hardwareVersion, string key, WrxTcpChannelType channelType)
        {
            Counter = counter;
            ModemImei = modemImei;
            SoftwareVersion = softwareVersion;
            HardwareVersion = hardwareVersion;
            Key = key;
            ChannelType = channelType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Счетчик авторизаций.
        /// </summary>
        public int Counter
        {
            get;
        }

        /// <summary>
        /// IMEI модема.
        /// </summary>
        public string ModemImei
        {
            get;
        }

        /// <summary>
        /// Версия ПО.
        /// </summary>
        public string SoftwareVersion
        {
            get;
        }

        /// <summary>
        /// Версия железа.
        /// </summary>
        public string HardwareVersion
        {
            get;
        }

        /// <summary>
        /// Ключ авторизации или <see cref="string.Empty"/>.
        /// </summary>
        public string Key
        {
            get;
        }

        /// <summary>
        /// Канал авторизации.
        /// </summary>
        public WrxTcpChannelType ChannelType
        {
            get;
        }
        #endregion

        #region Methods
        public void Deconstruct(out int counter, out string modemImei, out string softwareVersion, out string hardwareVersion, out string key, out WrxTcpChannelType channelType)
        {
            counter = Counter;
            modemImei = ModemImei;
            softwareVersion = SoftwareVersion;
            hardwareVersion = HardwareVersion;
            key = Key;
            channelType = ChannelType;
        }

        public override bool Equals(object obj)
        {
            return obj is WrxTcpAuthorizationData other && Equals(other);
        }

        public bool Equals(WrxTcpAuthorizationData other)
        {
            return Counter.Equals(other.Counter)
                && string.Equals(ModemImei, other.ModemImei)
                && string.Equals(SoftwareVersion, other.SoftwareVersion)
                && string.Equals(HardwareVersion, other.HardwareVersion)
                && string.Equals(Key, other.Key)
                && ChannelType.Equals(other.ChannelType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Counter, ModemImei, SoftwareVersion, HardwareVersion, Key, ChannelType);
        }

        public static WrxTcpAuthorizationData Read(ReadOnlySpan<byte> bytes)
        {
            var counter = (int)bytes[0x00];
            var modemImei = ReadString(bytes.Slice(0x01, 0x0F));
            var softwareVersion = ReadString(bytes.Slice(0x10, 0x11));
            var hardwareVersion = ReadString(bytes.Slice(0x21, 0x0A));
            var key = ReadString(bytes.Slice(0x2B, 0x10));
            var channel = (WrxTcpChannelType)bytes[0x3E];

            return new WrxTcpAuthorizationData(counter, modemImei, softwareVersion, hardwareVersion, key, channel);
        }

        public void Write(Span<byte> bytes)
        {
            bytes[0x00] = (byte)Counter;
            WriteString(bytes.Slice(0x01, 0x0F), ModemImei);
            WriteString(bytes.Slice(0x10, 0x11), SoftwareVersion);
            WriteString(bytes.Slice(0x21, 0x0A), HardwareVersion);
            WriteString(bytes.Slice(0x2B, 0x10), Key);
            bytes[0x3E] = (byte)ChannelType;
        }

        private static string ReadString(ReadOnlySpan<byte> bytes)
        {
            var length = bytes.Length;

            while (length > 0 && bytes[length - 1] == 0)
            {
                --length;
            }

            var value = Encoding.UTF8.GetString(bytes[..length]);
            return value;
        }

        private static void WriteString(Span<byte> bytes, string value)
        {
            var length = Encoding.UTF8.GetBytes(value, bytes);
            bytes[length..].Fill(0);
        }
        #endregion

        #region Operators
        public static bool operator ==(WrxTcpAuthorizationData left, WrxTcpAuthorizationData right)
        {
            return left.Counter == right.Counter
                && left.ModemImei == right.ModemImei
                && left.SoftwareVersion == right.SoftwareVersion
                && left.HardwareVersion == right.HardwareVersion
                && left.Key == right.Key
                && left.ChannelType == right.ChannelType;
        }

        public static bool operator !=(WrxTcpAuthorizationData left, WrxTcpAuthorizationData right)
        {
            return left.Counter != right.Counter
                || left.ModemImei != right.ModemImei
                || left.SoftwareVersion != right.SoftwareVersion
                || left.HardwareVersion != right.HardwareVersion
                || left.Key != right.Key
                || left.ChannelType != right.ChannelType;
        }
        #endregion
    }
}
