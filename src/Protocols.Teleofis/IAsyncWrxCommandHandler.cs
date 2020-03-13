using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public interface IAsyncWrxCommandHandler
    {
        #region Methods
        #region GSM
        /// <summary>
        /// Чтение IMEI модема.
        /// Команда: <c>0x76</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<string> GetGsmImeiAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Чтение IP-адреса устройства.
        /// Команда: <c>0x7A</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<string> GetGsmIpAddressAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Чтение уровня сигнала GSM.
        /// Команда: <c>0x7B</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<byte> GetGsmSignalStrengthAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Текущее время.
        /// Команда: <c>0x8D</c>.
        /// Примечание: отсчитывается от 1 января 1970 года.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<uint> GetLocalTimeSecondsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Текущее время.
        /// Команда: <c>0x8D</c>.
        /// Примечание: отсчитывается от 1 января 1970 года.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetLocalTimeSecondsAsync(uint value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Чтение текущего режима работы GSM (2G или 3G).
        /// Команда: <c>0x94</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<WrxGsmMode> GetGsmModeAsync(CancellationToken cancellationToken = default);
        #endregion

        #region UART
        #endregion

        #region SIM1 & SIM2
        #endregion

        #region CSD
        #endregion

        #region Служебный канал
        #endregion

        #region Будильник
        #endregion

        #region Рабочий режим        
        #endregion

        #region Время
        #endregion

        #region Настройки администратора
        /// <summary>
        /// Выбор уровня диагностики.
        /// Команда: <c>0x4F</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<WrxDiagnosticsLevel> GetDiagnosticsLevelAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Выбор уровня диагностики.
        /// Команда: <c>0x4F</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetDiagnosticsLevelAsync(WrxDiagnosticsLevel value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Чтение значения внешнего напряжения.
        /// Команда: <c>0x83</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<uint> GetSupplyVoltageMillivoltsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Чтение значения напряжения на входе 1.
        /// Команда: <c>0x84</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<uint> GetInputVoltageMillivoltsAsync(CancellationToken cancellationToken = default);
        #endregion

        #region Служебные команды
        /// <summary>
        /// Сброс устройства.
        /// Команда: <c>0x7C</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetResetAsync(uint value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Запись пароля для доступа к настройкам.
        /// Команда: <c>0x4E</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask AuthorizeSettingsPasswordAsync(string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Запись пароля для доступа к настройкам.
        /// Команда: <c>0x4E</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetSettingsPasswordAsync(string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Установка состояния реле.
        /// Команда: <c>0x81</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        ValueTask SetOutputStateAsync(WrxOutputState value, CancellationToken cancellationToken = default);

        /// <summary>
        /// НЕДОКУМЕНТИРОВАННАЯ ВОЗМОЖНОСТЬ: чтение состояния реле.
        /// Команда: <c>0x92</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<(WrxOutputState, uint)> GetOutputStateWithSwitchbackSecondsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Установить состояние реле с временем обратного переключения.
        /// Команда: <c>0x92</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetOutputStateWithSwitchbackSecondsAsync((WrxOutputState, uint) value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Уровень напряжения на батарейке.
        /// Команда: <c>0xBB</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<uint> GetBatteryVoltageMillivoltsAsync(CancellationToken cancellationToken = default);
        #endregion

        #region АЦП
        #endregion

        #region Линии ввода-вывода
        /// <summary>
        /// Время включения нагрузки.
        /// Команда: <c>0xB4</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<ushort> GetOutputAlarmDueTimeMinutesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Время включения нагрузки.
        /// Команда: <c>0xB4</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetOutputAlarmDueTimeMinutesAsync(ushort value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Тип будильника для управления выходом.
        /// Команда: <c>0xB5</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<WrxAlarmType> GetOutputAlarmTypeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Тип будильника для управления выходом.
        /// Команда: <c>0xB5</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetOutputAlarmTypeAsync(WrxAlarmType value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Расписание будильника для управления выходом.
        /// Команда: <c>0xB6</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<uint> GetOutputAlarmScheduleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Расписание будильника для управления выходом.
        /// Команда: <c>0xB6</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetOutputAlarmScheduleAsync(uint value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Длительность включения нагрузки.
        /// Команда: <c>0xB7</c>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<uint> GetOutputAlarmDurationSecondsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Длительность включения нагрузки.
        /// Команда: <c>0xB7</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask SetOutputAlarmDurationSecondsAsync(uint value, CancellationToken cancellationToken = default);
        #endregion
        #endregion
    }
}
