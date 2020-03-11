using System;
using System.Text;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public readonly struct WrxServerAddress
    {
        #region Properties
        public string Host
        {
            get;
        }

        public int Port
        {
            get;
        }
        #endregion

        #region Methods
        public static WrxServerAddress CreateFromBytes(ReadOnlySpan<byte> bytes, out int count)
        {
            throw new NotImplementedException();
        }

        public void ToBytes(Span<byte> bytes, out int count)
        {
            count = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", Host, Port), bytes);
        }
        #endregion
    }
}
