using Swsu.StreetLights.Protocols.Teleofis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TeleofisTestServer
{
    class AsyncWrxCommandHandler : IAsyncWrxCommandHandler
    {
        public ValueTask AuthorizeSettingsPasswordAsync(string value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<uint> GetBatteryVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<WrxDiagnosticsLevel> GetDiagnosticsLevelAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> GetGsmImeiAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> GetGsmIpAddressAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<WrxGsmMode> GetGsmModeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<byte> GetGsmSignalStrengthAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<uint> GetInputVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<uint> GetLocalTimeSecondsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<ushort> GetOutputAlarmDueTimeMinutesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<uint> GetOutputAlarmDurationSecondsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<uint> GetOutputAlarmScheduleAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<WrxAlarmType> GetOutputAlarmTypeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<(WrxOutputState, uint)> GetOutputStateWithSwitchbackSecondsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<uint> GetSupplyVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetDiagnosticsLevelAsync(WrxDiagnosticsLevel value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetLocalTimeSecondsAsync(uint value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetOutputAlarmDueTimeMinutesAsync(ushort value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetOutputAlarmDurationSecondsAsync(uint value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetOutputAlarmScheduleAsync(uint value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetOutputAlarmTypeAsync(WrxAlarmType value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetOutputStateAsync(WrxOutputState value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetOutputStateWithSwitchbackSecondsAsync((WrxOutputState, uint) value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetResetAsync(uint value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetSettingsPasswordAsync(string value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
