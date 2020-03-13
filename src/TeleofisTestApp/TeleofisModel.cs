using Swsu.StreetLights.Protocols.Teleofis;
using System;

namespace TeleofisTestApp
{
    class TeleofisModel
    {
        private WrxOutputState _outputState;

        private bool _isForcedSwitch;

        private uint _localTime = Convert.ToUInt32((DateTime.Now - DateTime.UnixEpoch).TotalSeconds);

        private DateTime _lastTime = DateTime.Now;

        private DateTime _outputStateSwitchTime  = DateTime.Now;

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

        public uint LocalTime
        {
            get
            {
                var span = DateTime.Now - _lastTime;
                _localTime += (uint)span.TotalSeconds;
                _lastTime = DateTime.Now;
                return _localTime;
            }
            set => _localTime = value;
        }

        public ushort OutputAlarmDueTime { get; set; }

        public uint OutputAlarmDuration { get; set; }

        public uint OutputAlarmSchedule { get; set; }

        public WrxAlarmType OutputAlarmType { get; set; }

        public uint SupplyVoltage { get; set; }

        public WrxOutputState OutputState
        {
            get
            {
                CheckOutputState();
                return _outputState;
            }
            set
            {
                _outputState = value;
                _outputStateSwitchTime = DateTime.Now;
            }
        }

        public (WrxOutputState, uint) OutputStateWithSwitchback { get; set; }



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
            //Вынести delay в config
        }

        private void CheckOutputState() //monthly
        {
            int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            var dif = (now - OutputAlarmDueTime) * 60;
            if (dif >= 0 && dif <= OutputAlarmDuration)
                _outputState = WrxOutputState.On;
            else
                _outputState = WrxOutputState.Off;
        }

    }
}
