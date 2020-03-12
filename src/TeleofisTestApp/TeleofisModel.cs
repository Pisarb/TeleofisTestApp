using Swsu.StreetLights.Protocols.Teleofis;
using System;

namespace TeleofisTestApp
{
    class TeleofisModel
    {
        private WrxOutputState _outputState;

        public string Immei { get; set; }

        public string AuthorizeSettingsPassword { get; set; }

        public uint BatteryVoltage { get; set; }

        public WrxDiagnosticsLevel DiagnosticsLevel { get; set; }

        public WrxGsmMode WrxGsmMode { get; set; }

        public byte SignalStrength { get; set; }

        public uint InputVoltage
        {
            get => GetCurrentInputVoltage();
        } //5 V on, 0 V off

        public uint LocalTime { get; set; } = Convert.ToUInt32((DateTime.Now - DateTime.UnixEpoch).TotalSeconds);

        public ushort OutputAlarmDueTime { get; set; }

        public uint OutputAlarmDuration { get; set; }

        public uint OutputAlarmSchedule { get; set; }

        public WrxAlarmType OutputAlarmType { get; set; }

        public uint SupplyVoltage { get; set; }

        public WrxOutputState OutputState
        {
            get => _outputState;
            set
            {
                _outputState = value;
                _outputStateSwitchTime = DateTime.Now;
            }
        }

        public (WrxOutputState, uint) OutputStateWithSwitchback { get; set; }

        private DateTime _outputStateSwitchTime;

        private uint GetCurrentInputVoltage()
        {
            var span = DateTime.Now - _outputStateSwitchTime;
            if (OutputState == WrxOutputState.On)
            {
                if (span.TotalSeconds > 0 || span.TotalMilliseconds > 20)
                    return 5000;
                return Convert.ToUInt32(Math.Round(Math.Sin(Math.PI / 2 * 1 / (21 - span.TotalMilliseconds)) * 5000, 0));
            }
            else
            {
                if (span.TotalSeconds > 0 || span.TotalMilliseconds > 20)
                    return 0;
                return Convert.ToUInt32(Math.Round(Math.Cos(Math.PI / 2 * 1 / (21 - span.TotalMilliseconds)) * 5000, 0));
            }

        }

    }
}
