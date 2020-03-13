using Swsu.StreetLights.Protocols.Teleofis.Infrastructure;
using System.Collections.Generic;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    using static FixedSizeDataInfo;
    using static VariableSizeDataInfo;

    public partial class WrxCommandServer
    {
        #region Nested Types
        private static class Commands
        {
            #region Fields
            private static readonly Dictionary<int, CommandEntry> _entryByCommand = new Dictionary<int, CommandEntry>()
            {
                #region GSM
                [WrxDataCommands.GsmImei] = new CommandEntry<string>(
                    String(15),
                    onGetData: (h, t) => h.GetGsmImeiAsync(t)),

                [WrxDataCommands.GsmIpAddress] = new CommandEntry<string>(
                    String(32),
                    onGetData: (h, t) => h.GetGsmIpAddressAsync(t)),

                [WrxDataCommands.GsmSignalStrength] = new CommandEntry<byte>(
                    Byte,
                    onGetData: (h, t) => h.GetGsmSignalStrengthAsync(t)),

                [WrxDataCommands.LocalTimeSeconds] = new CommandEntry<uint>(
                    UInt32,
                    onGetData: (h, t) => h.GetLocalTimeSecondsAsync(t),
                    onSetData: (h, v, t) => h.SetLocalTimeSecondsAsync(v, t)),

                [WrxDataCommands.GsmMode] = new CommandEntry<WrxGsmMode>(
                    Enum<WrxGsmMode>(Byte),
                    onGetData: (h, t) => h.GetGsmModeAsync(t)),
                #endregion

                #region Настройки администратора
                [WrxDataCommands.DiagnosticsLevel] = new CommandEntry<WrxDiagnosticsLevel>(
                    Enum<WrxDiagnosticsLevel>(Byte),
                    onGetData: (h, t) => h.GetDiagnosticsLevelAsync(t),
                    onSetData: (h, v, t) => h.SetDiagnosticsLevelAsync(v, t)),

                [WrxDataCommands.SupplyVoltageMillivolts] = new CommandEntry<uint>(
                    UInt32,
                    onGetData: (h, t) => h.GetSupplyVoltageMillivoltsAsync(t)),

                [WrxDataCommands.InputVoltageMillivolts] = new CommandEntry<uint>(
                    UInt32,
                    onGetData: (h, t) => h.GetInputVoltageMillivoltsAsync(t)),
                #endregion

                #region Служебные команды
                [WrxDataCommands.Reset] = new CommandEntry<uint>(
                    UInt32,
                    onSetData: (h, v, t) => h.SetResetAsync(v, t)),

                [WrxDataCommands.SettingsPassword] = new CommandEntry<string>(
                    String(32),
                    onSetData: (h, v, t) => h.SetSettingsPasswordAsync(v, t),
                    onAuthorizeData: (h, v, t) => h.AuthorizeSettingsPasswordAsync(v, t)),

                [WrxDataCommands.OutputState] = new CommandEntry<WrxOutputState>(
                    Enum<WrxOutputState>(Byte),
                    onSetData: (h, v, t) => h.SetOutputStateAsync(v, t)),

                [WrxDataCommands.OutputStateWithSwitchbackSeconds] = new CommandEntry<(WrxOutputState, uint)>(
                    Tuple(Enum<WrxOutputState>(Byte), UInt32),
                    onGetData: (h, t) => h.GetOutputStateWithSwitchbackSecondsAsync(t),
                    onSetData: (h, v, t) => h.SetOutputStateWithSwitchbackSecondsAsync(v, t)),

                [WrxDataCommands.BatteryVoltageMillivolts] = new CommandEntry<uint>(
                    UInt32,
                    onGetData: (h, t) => h.GetBatteryVoltageMillivoltsAsync(t)),
                #endregion

                #region Линии ввода-вывода
                [WrxDataCommands.OutputAlarmDueTimeMinutes] = new CommandEntry<ushort>(
                    UInt16,
                    onGetData: (h, t) => h.GetOutputAlarmDueTimeMinutesAsync(t),
                    onSetData: (h, v, t) => h.SetOutputAlarmDueTimeMinutesAsync(v, t)),

                [WrxDataCommands.OutputAlarmDurationSeconds] = new CommandEntry<uint>(
                    UInt32,
                    onGetData: (h, t) => h.GetOutputAlarmDurationSecondsAsync(t),
                    onSetData: (h, v, t) => h.SetOutputAlarmDurationSecondsAsync(v, t)),

                [WrxDataCommands.OutputAlarmSchedule] = new CommandEntry<uint>(
                    UInt32,
                    onGetData: (h, t) => h.GetOutputAlarmScheduleAsync(t),
                    onSetData: (h, v, t) => h.SetOutputAlarmScheduleAsync(v, t)),

                [WrxDataCommands.OutputAlarmType] = new CommandEntry<WrxAlarmType>(
                    Enum<WrxAlarmType>(Byte),
                    onGetData: (h, t) => h.GetOutputAlarmTypeAsync(t),
                    onSetData: (h, v, t) => h.SetOutputAlarmTypeAsync(v, t))
                #endregion
            };
            #endregion

            #region Methods
            internal static bool TryGetEntry(int command, out CommandEntry value)
            {
                return _entryByCommand.TryGetValue(command, out value);
            }
            #endregion
        }
        #endregion
    }
}
