using Swsu.StreetLights.Common.IO;
using Swsu.StreetLights.Common.Net.Abstractions;
using Swsu.StreetLights.Protocols.Teleofis;
using Swsu.StreetLights.Protocols.Teleofis.Packets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TeleofisTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncWrxTcpDeviceAuthorizer deviceAuthorizer = new AsyncWrxTcpDeviceAuthorizer(0, "354190023896443", "0.1", "1.0", "0000", WrxTcpChannelType.Service);
            AsyncWrxTcpDeviceAuthorizer deviceAuthorizer1 = new AsyncWrxTcpDeviceAuthorizer(0, "354190023896443", "0.1", "1.0", "0000", WrxTcpChannelType.Main);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
            var sock = SocketFactory.Instance.Create(SocketType.Stream, ProtocolType.Tcp);
            sock.ConnectAsync(endPoint);
            SimpleSocketInputStream inputStream = new SimpleSocketInputStream(sock);
            SimpleSocketOutputStream outputStream = new SimpleSocketOutputStream(sock);
            var model = new TeleofisModel
            {
                AuthorizeSettingsPassword = "0000",
                BatteryVoltage = 200,
                SignalStrength = 20,
                WrxGsmMode = WrxGsmMode.ThreeG,
                DiagnosticsLevel = WrxDiagnosticsLevel.Messages,
                Immei = "354190023896443",
                InputVoltage = 500,
                LocalTime = 100000000,
                SupplyVoltage = 800,
                OutputAlarmDueTime = 1,
                OutputAlarmDuration = 3,
                OutputAlarmSchedule = 6,
                OutputAlarmType = WrxAlarmType.Monthly
            };
            AsyncWrxCommandHandler handler = new AsyncWrxCommandHandler(model);
            WrxPacketReader reader = new WrxPacketReader(inputStream);
            WrxPacketWriter writer = new WrxPacketWriter(outputStream);
            WrxCommandServer wrxCommandServer = new WrxCommandServer(inputStream, outputStream, handler);
            Authorize(reader, writer, deviceAuthorizer, wrxCommandServer);
            Console.WriteLine(Convert.ToString(model.OutputAlarmSchedule, 2));
            Console.ReadKey();
        }
        private static async void Authorize(WrxPacketReader reader, WrxPacketWriter writer, AsyncWrxTcpDeviceAuthorizer deviceAuthorizer, WrxCommandServer wrxCommandServer)
        {
             await WrxTcpDeviceAuthorization.AuthorizeAsync(reader, writer, deviceAuthorizer);
            while (true)
                await wrxCommandServer.ProcessNextAsync();
        }
    }
}
