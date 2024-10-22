﻿using Swsu.StreetLights.Protocols.Teleofis;
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

        private uint _maxInputVoltage = 5000;

        private double _voltageChangeDelay;

        private bool _isScheduleUsed = false;

        private uint _outputAlarmSchedule;

        private ushort _outputAlarmDueTimeMinutes;

        private uint _outputAlarmDurationSeconds;

        private double _voltageTolerance;

        public string Imei { get; }

        public string AuthorizeSettingsPassword { get; set; }

        public uint BatteryVoltage { get; } = 200;

        public WrxDiagnosticsLevel DiagnosticsLevel { get; set; }

        public WrxGsmMode WrxGsmMode { get; set; }

        public byte SignalStrength { get; } = 20;

        public uint InputVoltage
        {
            get
            {
                var outputState = OutputState;
                return Convert.ToUInt32(PowerSurgesRate() * GetInputVoltage((DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - _outputStateSwitchTime) * 1000 + DateTime.Now.Millisecond, outputState));
            }
        }

        public uint LocalTimeSeconds
        {
            get
            {
                if (_lastTime.Day != DateTime.Now.Day && OutputAlarmSchedule != 0)
                    _isScheduleUsed = true;
                var span = DateTime.Now - _lastTime;
                _localTime += (uint)span.TotalSeconds;
                _lastTime = DateTime.Now;
                return _localTime;
            }
            set => _localTime = value;
        }

        public ushort OutputAlarmDueTimeMinutes
        {
            get => _outputAlarmDueTimeMinutes;
            set
            {
                _outputAlarmDueTimeMinutes = value;
                int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if (now <= OutputAlarmDueTimeMinutes || now * 60 + DateTime.Now.Second <= OutputAlarmDueTimeMinutes * 60 + OutputAlarmDurationSeconds)
                    _isScheduleUsed = true;
            }
        }

        public uint OutputAlarmDurationSeconds
        {
            get => _outputAlarmDurationSeconds;
            set
            {
                _outputAlarmDurationSeconds = value;
                int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if (now <= OutputAlarmDueTimeMinutes || now * 60 + DateTime.Now.Second <= OutputAlarmDueTimeMinutes * 60 + OutputAlarmDurationSeconds)
                    _isScheduleUsed = true;
            }
        }

        public uint OutputAlarmSchedule
        {
            get => _outputAlarmSchedule;
            set
            {
                _outputAlarmSchedule = value;
                int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if (now <= OutputAlarmDueTimeMinutes || now * 60 + DateTime.Now.Second <= OutputAlarmDueTimeMinutes * 60 + OutputAlarmDurationSeconds)
                    _isScheduleUsed = true;
            }
        }

        public WrxAlarmType OutputAlarmType { get; set; }

        public uint SupplyVoltage { get; } = 800;

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
                if (value == WrxOutputState.Off)
                {
                    int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                    if (now > OutputAlarmDueTimeMinutes)
                        _isScheduleUsed = false;
                }
            }
        }

        public uint SwitchbackDelaySeconds
        {
            get;
            set;
        }

        private TeleofisModel(string imei)
        {
            var rm = new ResourceManager("TeleofisTestApp.Properties.Resources", Assembly.GetExecutingAssembly());
            _voltageChangeDelay = Convert.ToDouble(rm.GetString("VoltageChangeDelayMilliseconds"));
            _voltageTolerance = Convert.ToDouble(rm.GetString("VoltageTolerance"));
            Imei = imei;
        }


        public TeleofisModel(string imei, uint maxInputVoltage = 5000, uint batteryVoltage = 200, byte signalStrength = 20, uint supplyVoltage = 800) : this(imei)
        {
            _maxInputVoltage = maxInputVoltage;
            BatteryVoltage = batteryVoltage;
            SignalStrength = signalStrength;
            SupplyVoltage = supplyVoltage;
        }


        private uint GetInputVoltage(int diffMilliseconds, WrxOutputState outputState)
        {
            if (outputState == WrxOutputState.On)
            {
                var diff = (double)_voltageBeforeSwitch / _maxInputVoltage;
                if (diffMilliseconds < 0 || (diffMilliseconds / _voltageChangeDelay) + diff >= 1)
                    return _maxInputVoltage;
                return Convert.ToUInt32(Math.Round(((diffMilliseconds / _voltageChangeDelay) + diff) * _maxInputVoltage, 0));
            }
            else
            {
                var dif = (double)_voltageBeforeSwitch / _maxInputVoltage;
                if (diffMilliseconds < 0 || (((_voltageChangeDelay - diffMilliseconds) / _voltageChangeDelay) - 1 + dif) <= 0)
                    return 0;
                return Convert.ToUInt32(Math.Round((((_voltageChangeDelay - diffMilliseconds) / _voltageChangeDelay) - 1 + dif) * _maxInputVoltage, 0));
            }
        }

        private void CheckOutputState()
        {
            if (_isScheduleUsed && CheckPickedDay(OutputAlarmSchedule, DateTime.Now.Day))
            {
                int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                var diff = (now - OutputAlarmDueTimeMinutes) * 60 + DateTime.Now.Second;
                if (diff >= 0)
                {
                    if (diff <= OutputAlarmDurationSeconds && _outputState != WrxOutputState.On)
                    {
                        _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                        _outputState = WrxOutputState.On;
                        _outputStateSwitchTime = OutputAlarmDueTimeMinutes * 60;
                    }
                    else if (diff > OutputAlarmDurationSeconds && _outputState != WrxOutputState.Off)
                    {
                        _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                        _outputState = WrxOutputState.Off;
                        _outputStateSwitchTime = Convert.ToInt32(OutputAlarmDueTimeMinutes * 60 + OutputAlarmDurationSeconds);
                        _isScheduleUsed = false;
                        SwitchbackDelaySeconds = 0;
                    }
                }
            }
            if (SwitchbackDelaySeconds != 0)
            {
                var diff = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - _outputStateSwitchTime;
                if (diff < SwitchbackDelaySeconds && _outputState != WrxOutputState.On)
                {
                    _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                    _outputState = WrxOutputState.On;
                }
                else if (diff >= SwitchbackDelaySeconds)
                {
                    _voltageBeforeSwitch = GetInputVoltage(diff * 1000 + DateTime.Now.Millisecond, _outputState);
                    _outputStateSwitchTime += (int)SwitchbackDelaySeconds;
                    SwitchbackDelaySeconds = 0;
                    _outputState = WrxOutputState.Off;
                    int now = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                    if (now > OutputAlarmDueTimeMinutes)
                        _isScheduleUsed = false;
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

        private double PowerSurgesRate()
        {
            var rnd = new Random();
            double deviation = rnd.Next(Convert.ToInt32(_voltageTolerance * 10) + 1);
            return (1 - (deviation / 100));
        }

        public void Reset()
        {
            AuthorizeSettingsPassword = "0000";
            OutputAlarmDueTimeMinutes = 0;
            OutputAlarmDurationSeconds = 0;
            OutputAlarmSchedule = 0;
            OutputAlarmType = WrxAlarmType.Off;
            OutputState = WrxOutputState.Off;
            SwitchbackDelaySeconds = 0;
            _isScheduleUsed = false;
        }
    }
}
