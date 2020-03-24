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

        private double _voltageChangeDelay;

        private bool _isScheduleUsed = false;

        private uint _outputAlarmSchedule;

        public string Immei { get; set; }

        public string AuthorizeSettingsPassword { get; set; }

        public uint BatteryVoltage { get; set; }

        public WrxDiagnosticsLevel DiagnosticsLevel { get; set; }

        public WrxGsmMode WrxGsmMode { get; set; }

        public byte SignalStrength { get; set; }

        public uint InputVoltage
        {
            get
            {
                var outputState = OutputState;
                return GetInputVoltage((DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - _outputStateSwitchTime) * 1000 + DateTime.Now.Millisecond, outputState);
            }
        }

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

        public uint OutputAlarmSchedule
        {
            get => _outputAlarmSchedule;
            set
            {
                _outputAlarmSchedule = value;
                _isScheduleUsed = true;
            }
        }

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
                _voltageBeforeSwitch = GetInputVoltage((DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - _outputStateSwitchTime) * 1000 + DateTime.Now.Millisecond, _outputState);
                _outputStateSwitchTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                _outputState = value;
            }
        } 
        //TODO : power surges simulation
        public uint Delay
        {
            get;
            set;
        }

        public TeleofisModel()
        {
            var rm = new ResourceManager("TeleofisTestApp.Properties.Resources", Assembly.GetExecutingAssembly());
            _voltageChangeDelay = Convert.ToDouble(rm.GetString("Delay"));
        }


        private uint GetInputVoltage(int diffMilliseconds, WrxOutputState outputState)
        {

            if (outputState == WrxOutputState.On)
            {
                var diff = (double)_voltageBeforeSwitch / _maxVoltage;
                if (diffMilliseconds < 0 || (diffMilliseconds / _voltageChangeDelay) + diff >= 1)
                    return _maxVoltage;
                return Convert.ToUInt32(Math.Round(((diffMilliseconds / _voltageChangeDelay) + diff) * _maxVoltage, 0));
            }
            else
            {
                var dif = (double)_voltageBeforeSwitch / _maxVoltage;
                if (diffMilliseconds < 0 || (((_voltageChangeDelay - diffMilliseconds) / _voltageChangeDelay) - 1 + dif) <= 0)
                    return 0;
                return Convert.ToUInt32(Math.Round((((_voltageChangeDelay - diffMilliseconds) / _voltageChangeDelay) - 1 + dif) * _maxVoltage, 0));
            }
        }

        private void CheckOutputState()
        {
            if (CheckPickedDay(OutputAlarmSchedule, DateTime.Now.Day) && _isScheduleUsed) //ну такое
            {
                int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                var diff = (now - OutputAlarmDueTime) * 60 + DateTime.Now.Second;
                if (diff >= 0)
                {
                    if (diff <= OutputAlarmDuration && _outputState != WrxOutputState.On)
                    {
                        _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                        _outputState = WrxOutputState.On;
                        _outputStateSwitchTime = OutputAlarmDueTime * 60;
                    }
                    else if (diff > OutputAlarmDuration && _outputState != WrxOutputState.Off)
                    {
                        _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                        _outputState = WrxOutputState.Off;
                        _outputStateSwitchTime = Convert.ToInt32(OutputAlarmDueTime * 60 + OutputAlarmDuration);
                        _isScheduleUsed = false;
                        Delay = 0;
                    }
                }
            }
            if (Delay != 0)
            {
                var diff = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - _outputStateSwitchTime;
                if (diff < Delay && _outputState != WrxOutputState.On)
                {
                    _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                    _outputState = WrxOutputState.On;
                }
                else if (diff >= Delay)
                {
                    _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                    _outputStateSwitchTime += (int)Delay;
                    Delay = 0;
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
