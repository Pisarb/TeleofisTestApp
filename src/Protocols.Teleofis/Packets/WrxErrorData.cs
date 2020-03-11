using System;
using System.Buffers.Binary;

namespace Swsu.StreetLights.Protocols.Teleofis.Packets
{
    public readonly struct WrxErrorData
    {
        #region Constructors
        public WrxErrorData(int command, WrxPacketError error)
        {
            Command = command;
            Error = error;
        }
        #endregion

        #region Properties
        public int Command
        {
            get;
        }

        public WrxPacketError Error
        {
            get;
        }
        #endregion

        #region Methods
        public void Deconstruct(out int command, out WrxPacketError error)
        {
            command = Command;
            error = Error;
        }

        public static int GetSize(int protocol)
        {
            return protocol switch
            {
                0 => 2,
                1 => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(protocol)),
            };
        }

        public static WrxErrorData Read(int protocol, ReadOnlySpan<byte> buffer)
        {
            return protocol switch
            {
                0 => new WrxErrorData(buffer[0], (WrxPacketError)buffer[1]),
                1 => new WrxErrorData(BinaryPrimitives.ReadUInt16LittleEndian(buffer[..2]), (WrxPacketError)buffer[2]),
                _ => throw new ArgumentOutOfRangeException(nameof(protocol)),
            };
        }

        public void Write(int protocol, Span<byte> buffer)
        {
            switch (protocol)
            {
                case 0:
                    buffer[0] = (byte)Command;
                    buffer[1] = (byte)Error;
                    break;

                case 1:
                    BinaryPrimitives.WriteUInt16LittleEndian(buffer[..2], (ushort)Command);
                    buffer[2] = (byte)Error;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(protocol));
            }
        }
        #endregion
    }
}
