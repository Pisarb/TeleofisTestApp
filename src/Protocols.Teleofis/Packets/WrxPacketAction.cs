namespace Swsu.StreetLights.Protocols.Teleofis.Packets
{
    public enum WrxPacketAction
    {
        /// <summary>
        /// Запрос данных по команде.
        /// </summary>
        GetDataRequest = 0,

        /// <summary>
        /// Запись данных по команде.
        /// </summary>
        SetDataRequest = 1,

        /// <summary>
        /// Ответ на команду запроса данных.
        /// </summary>
        GetDataResponse = 2,

        /// <summary>
        /// Ответ на команду установки данных.
        /// </summary>
        SetDataResponse = 3,

        /// <summary>
        /// Запрос на начало сессии настройки модема.
        /// </summary>
        AuthorizeDataRequest = 4,

        /// <summary>
        /// Ответ на начало сессии настройки модема.
        /// </summary>
        AuthorizeDataResponse = 5,

        /// <summary>
        /// Запрос авторизации устройства по TCP.
        /// </summary>
        TcpAuthorizationRequest = 6,

        /// <summary>
        /// Ответ авторизации устройства по TCP.
        /// </summary>
        TcpAuthorizationResponse = 7,

        /// <summary>
        /// Передача спорадических данных по TCP.
        /// </summary>
        TcpSporadicDataTransferRequest = 8,

        /// <summary>
        /// Подтверждение о получении спорадических данных по TCP.
        /// </summary>
        TcpSporadicDataTransferResponse = 9
    }
}
