using Swsu.StreetLights.Common.IO;
using Swsu.StreetLights.Common.Net.Abstractions;
using Swsu.StreetLights.Protocols.Teleofis;
using Swsu.StreetLights.Protocols.Teleofis.Packets;
using System;
using System.Net;
using System.Net.Sockets;

namespace TeleofisTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var imei = TeleofisHelper.GetNewImei();
            AsyncWrxTcpDeviceAuthorizer deviceAuthorizer = new AsyncWrxTcpDeviceAuthorizer(0, imei, "0.1", "1.0", "0000", WrxTcpChannelType.Service);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
            var socket = SocketFactory.Instance.Create(SocketType.Stream, ProtocolType.Tcp);
            socket.ConnectAsync(endPoint);
            SimpleSocketInputStream inputStream = new SimpleSocketInputStream(socket);
            SimpleSocketOutputStream outputStream = new SimpleSocketOutputStream(socket);
            var model = new TeleofisModel(imei)
            {
                AuthorizeSettingsPassword = "0000",
                WrxGsmMode = WrxGsmMode.ThreeG,
                DiagnosticsLevel = WrxDiagnosticsLevel.Messages,
                OutputAlarmDueTimeMinutes = 0,
                OutputAlarmDurationSeconds = 0,
                OutputAlarmSchedule = 0,
                OutputAlarmType = WrxAlarmType.Monthly
            };
            AsyncWrxCommandHandler handler = new AsyncWrxCommandHandler(model);
            WrxPacketReader reader = new WrxPacketReader(inputStream);
            WrxPacketWriter writer = new WrxPacketWriter(outputStream);
            WrxCommandServer wrxCommandServer = new WrxCommandServer(inputStream, outputStream, handler);
            Authorize(reader, writer, deviceAuthorizer, wrxCommandServer);
            Console.WriteLine("Включился");
            Console.ReadKey();
        }
        private static async void Authorize(WrxPacketReader reader, WrxPacketWriter writer, AsyncWrxTcpDeviceAuthorizer deviceAuthorizer, WrxCommandServer wrxCommandServer)
        {
            try
            {
                await WrxTcpDeviceAuthorization.AuthorizeAsync(reader, writer, deviceAuthorizer);
                while (true)
                    await wrxCommandServer.ProcessNextAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
