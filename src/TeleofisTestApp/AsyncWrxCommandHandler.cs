using Swsu.StreetLights.Protocols.Teleofis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TeleofisTestApp
{
    class AsyncWrxCommandHandler : IAsyncWrxCommandHandler
    {
        private TeleofisModel _model;

        public AsyncWrxCommandHandler(TeleofisModel teleofisModel)
        {
            _model = teleofisModel;
        }
        public ValueTask AuthorizeSettingsPasswordAsync(string value, CancellationToken cancellationToken = default)
        {
            _model.AuthorizeSettingsPassword = value;
            return new ValueTask();
        }

        public ValueTask<uint> GetBatteryVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<uint>(_model.BatteryVoltage);
        }

        public ValueTask<WrxDiagnosticsLevel> GetDiagnosticsLevelAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<WrxDiagnosticsLevel>(_model.DiagnosticsLevel);
        }

        public ValueTask<string> GetGsmImeiAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<string>(_model.Immei);
        }

        public ValueTask<string> GetGsmIpAddressAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<string>("125.7.7.8"); //Zd
        }

        public ValueTask<WrxGsmMode> GetGsmModeAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<WrxGsmMode>(_model.WrxGsmMode);
        }

        public ValueTask<byte> GetGsmSignalStrengthAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<byte>(_model.SignalStrength);
        }

        public ValueTask<uint> GetInputVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<uint>(_model.InputVoltage);
        }

        public ValueTask<uint> GetLocalTimeSecondsAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<uint>(_model.LocalTime);
        }

        public ValueTask<ushort> GetOutputAlarmDueTimeMinutesAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<ushort>(_model.OutputAlarmDueTime);
        }

        public ValueTask<uint> GetOutputAlarmDurationSecondsAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<uint>(_model.OutputAlarmDuration);
        }

        public ValueTask<uint> GetOutputAlarmScheduleAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<uint>(_model.OutputAlarmSchedule);
        }

        public ValueTask<WrxAlarmType> GetOutputAlarmTypeAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<WrxAlarmType>(_model.OutputAlarmType);
        }

        public ValueTask<(WrxOutputState, uint)> GetOutputStateWithSwitchbackSecondsAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<(WrxOutputState, uint)>((_model.OutputState, 0));
        }

        public ValueTask<uint> GetSupplyVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<uint>(_model.SupplyVoltage);
        }

        public ValueTask SetDiagnosticsLevelAsync(WrxDiagnosticsLevel value, CancellationToken cancellationToken = default)
        {
            _model.DiagnosticsLevel = value;
            return new ValueTask();
        }

        public ValueTask SetLocalTimeSecondsAsync(uint value, CancellationToken cancellationToken = default)
        {
            _model.LocalTime = value;
            return new ValueTask();
        }

        public ValueTask SetOutputAlarmDueTimeMinutesAsync(ushort value, CancellationToken cancellationToken = default)
        {
            _model.OutputAlarmDueTime = value;
            return new ValueTask();
        }

        public ValueTask SetOutputAlarmDurationSecondsAsync(uint value, CancellationToken cancellationToken = default)
        {
            _model.OutputAlarmDuration = value;
            return new ValueTask();
        }

        public ValueTask SetOutputAlarmScheduleAsync(uint value, CancellationToken cancellationToken = default)
        {
            _model.OutputAlarmSchedule = value;
            return new ValueTask();
        }

        public ValueTask SetOutputAlarmTypeAsync(WrxAlarmType value, CancellationToken cancellationToken = default)
        {
            _model.OutputAlarmType = value;
            return new ValueTask();
        }

        public ValueTask SetOutputStateAsync(WrxOutputState value, CancellationToken cancellationToken = default)
        {
            _model.OutputState = value;
            return new ValueTask();
        }

        public ValueTask SetOutputStateWithSwitchbackSecondsAsync((WrxOutputState, uint) value, CancellationToken cancellationToken = default)
        {
            _model.OutputStateWithSwitchback = value;
            return new ValueTask();
        }

        public ValueTask SetResetAsync(uint value, CancellationToken cancellationToken = default)
        {
            //_model.Reset = value;
            return new ValueTask();
        }

        public ValueTask SetSettingsPasswordAsync(string value, CancellationToken cancellationToken = default)
        {

            return new ValueTask();
        }
    }
}
