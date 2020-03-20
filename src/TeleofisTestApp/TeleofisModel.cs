using Swsu.StreetLights.Protocols.Teleofis;
using System;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace TeleofisTestApp
{
    class TeleofisModel
    {
        private WrxOutputState _outputState;

        private uint _localTime = Convert.ToUInt32((DateTime.Now - DateTime.UnixEpoch).TotalSeconds);

        private DateTime _lastTime = DateTime.Now;

        private int _outputStateSwitchTime;

        private uint _voltageBeforeSwitch = 0;

        private uint _maxVoltage = 5000;

        private double _delay;

        public string Immei { get; set; }

        public string AuthorizeSettingsPassword { get; set; }

        public uint BatteryVoltage { get; set; }

        public WrxDiagnosticsLevel DiagnosticsLevel { get; set; }

        public WrxGsmMode WrxGsmMode { get; set; }

        public byte SignalStrength { get; set; }

        public uint InputVoltage => GetCurrentInputVoltage(DateTime.Now);

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
                _voltageBeforeSwitch = GetCurrentInputVoltage(DateTime.Now);
                _outputState = value;
                _outputStateSwitchTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
            }
        }
        //TODO : GetCurrentInputVoltage if was switched while not fully charged      
        //TODO : power surges simulation
        //Max
        public uint Delay
        {
            get;
            set;
        }

        public TeleofisModel()
        {
            var rm = new ResourceManager("TeleofisTestApp.Properties.Resources", Assembly.GetExecutingAssembly());
            _delay = Convert.ToDouble(rm.GetString("Delay"));
        }


        private uint GetCurrentInputVoltage(DateTime dateTime)
        {
            
            if (OutputState == WrxOutputState.On)
            {
                var milliseconds = (dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second - _outputStateSwitchTime) * 1000 + dateTime.Millisecond;
                var dif = (double)_voltageBeforeSwitch / _maxVoltage;
                if (milliseconds < 0 || (milliseconds / _delay) + dif >= 1)
                    return 5000;
                return Convert.ToUInt32(Math.Round(((milliseconds / _delay) + dif) * _maxVoltage, 0));
            }
            else
            {
                var milliseconds = (dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second - _outputStateSwitchTime) * 1000 + dateTime.Millisecond;
                var dif = (double)_voltageBeforeSwitch / _maxVoltage;
                if (milliseconds < 0 || (((_delay - milliseconds) / _delay) - 1 + dif) <= 0)
                    return 0;
                return Convert.ToUInt32(Math.Round((((_delay - milliseconds) / _delay) - 1 + dif) * _maxVoltage, 0));
            }
        }

        private void CheckOutputState()
        {
            if (CheckPickedDay(OutputAlarmSchedule, DateTime.Now.Day))
            {
                int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                var dif = (now - OutputAlarmDueTime) * 60;
                if (dif >= 0 && dif <= OutputAlarmDuration && _outputState != WrxOutputState.On)
                {
                    _outputState = WrxOutputState.On;
                    _outputStateSwitchTime = OutputAlarmDueTime * 60;
                }
                else if (_outputState != WrxOutputState.Off)
                {
                    _outputState = WrxOutputState.Off;
                    _outputStateSwitchTime = Convert.ToInt32(OutputAlarmDueTime * 60 + OutputAlarmDuration);
                    Delay = 0;
                }
            }
            if (Delay != 0)
            {
                var dif = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - _outputStateSwitchTime;
                if (dif < Delay)
                {
                    _outputState = WrxOutputState.On;
                }
                else
                {                   
                    int hour = _outputStateSwitchTime / 3600;
                    int minute = (_outputStateSwitchTime - hour * 3600) / 60;
                    int second = _outputStateSwitchTime - hour * 3600 - minute * 60;
                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second);
                    _outputStateSwitchTime += (int)Delay;
                    Delay = 0;
                    _voltageBeforeSwitch = GetCurrentInputVoltage(dt);
                    _outputState = WrxOutputState.Off;
                }
            }
        }

        private bool CheckPickedDay(uint schedule, int day)
        {
            if (day > 32 || day < 0)
                return false;
            var bin = Convert.ToString(schedule, 2);
            if (day > bin.Length)
                return false;
            bin = new string(bin.ToCharArray().Reverse().ToArray());
            return bin[day - 1] == '1';
        }
    }
}
