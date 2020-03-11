namespace Swsu.StreetLights.Protocols.Teleofis
{
    public static class WrxDataCommands
    {
        #region Constants
        #region GSM
        /// <summary>
        /// IMEI модема.
        /// </summary>
        public const int GsmImei = 0x76;

        /// <summary>
        /// IP-адрес устройства.
        /// </summary>
        public const int GsmIpAddress = 0x7A;

        /// <summary>
        /// Уровень сигнала GSM.
        /// </summary>
        public const int GsmSignalStrength = 0x7B;

        /// <summary>
        /// Текущее время. Примечание: отсчитывается от 1 января 1970 года.
        /// </summary>
        public const int LocalTimeSeconds = 0x8D;

        /// <summary>
        /// Текущий режим работы GSM (2G или 3G).
        /// </summary>
        public const int GsmMode = 0x94;
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
        /// </summary>
        public const int DiagnosticsLevel = 0x4F;

        /// <summary>
        /// Чтение значения внешнего напряжения.
        /// </summary>
        public const int SupplyVoltageMillivolts = 0x83;

        /// <summary>
        /// Чтение значения напряжения на входе 1.
        /// </summary>
        public const int InputVoltageMillivolts = 0x84;
        #endregion

        #region Служебные команды
        /// <summary>
        /// Сброс устройства.
        /// </summary>
        public const int Reset = 0x7C;

        /// <summary>
        /// Пароль для доступа к настройкам.
        /// </summary>
        public const int SettingsPassword = 0x4E;

        /// <summary>
        /// Состояние реле.
        /// </summary>
        public const int OutputState = 0x81;

        /// <summary>
        /// Установить состояние реле с временем обратного переключения.
        /// </summary>
        public const int OutputStateWithSwitchbackSeconds = 0x92;

        /// <summary>
        /// Уровень напряжения на батарейке.
        /// </summary>
        public const int BatteryVoltageMillivolts = 0xBB;
        #endregion

        #region АЦП
        #endregion

        #region Линии ввода-вывода
        /// <summary>
        /// Время включения нагрузки.
        /// </summary>
        public const int OutputAlarmDueTimeMinutes = 0xB4;

        /// <summary>
        /// Тип будильника для управления выходом.
        /// </summary>
        public const int OutputAlarmType = 0xB5;

        /// <summary>
        /// Расписание будильника для управления выходом.
        /// </summary>
        public const int OutputAlarmSchedule = 0xB6;

        /// <summary>
        /// Длительность включения нагрузки.
        /// </summary>
        public const int OutputAlarmDurationSeconds = 0xB7;
        #endregion
        #endregion
    }
}
