using Swsu.StreetLights.Protocols.Teleofis;
using System;

namespace TeleofisTestApp
{
    class TeleofisModel
    {
        public string Immei { get; set; }

        public string AuthorizeSettingsPassword { get; set; }

        public uint BatteryVoltage { get; set; }

        public WrxDiagnosticsLevel DiagnosticsLevel { get; set; }

        public WrxGsmMode WrxGsmMode { get; set; }

        public byte SignalStrength { get; set; }

        public uint InputVoltage { get; set; } //5 V on, 0 V off

        public uint LocalTime 
        { 
            get => Convert.ToUInt32((DateTime.Now - DateTime.UnixEpoch).TotalSeconds);
            set { }
        }

        public ushort OutputAlarmDueTime { get; set; }

        public uint OutputAlarmDuration { get; set; }

        public uint OutputAlarmSchedule { get; set; }

        public WrxAlarmType OutputAlarmType { get; set; }

        public uint SupplyVoltage { get; set; }

        public WrxOutputState OutputState { get; set; }

        public (WrxOutputState, uint) OutputStateWithSwitchback { get; set; }
       
    }
}
