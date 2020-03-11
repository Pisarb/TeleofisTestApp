using Swsu.StreetLights.Common;
using Swsu.StreetLights.Common.IO;
using Swsu.StreetLights.Common.Packets;
using Swsu.StreetLights.Protocols.Teleofis.Infrastructure;
using Swsu.StreetLights.Protocols.Teleofis.Packets;
using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    using static FixedSizeDataInfo;
    using static VariableSizeDataInfo;

    public class WrxCommandClient : IAsyncDisposable, IAsyncWrxCommandHandler
    {
        #region Fields
        private readonly ArrayPool<byte> _byteArrayPool;

        private readonly AsyncDisposableArg<IPacketReader<WrxPacket>> _responseReader;

        private readonly AsyncDisposableArg<IPacketWriter<WrxPacket>> _requestWriter;
        #endregion

        #region Constructors        
        public WrxCommandClient(
            IAsyncSimpleOutputStream<byte> requestStream,
            IAsyncSimpleInputStream<byte> responseStream,
            bool doNotDisposeRequestStream = false,
            bool doNotDisposeResponseStream = false) :
            this(
                requestStream,
                responseStream,
                ArrayPool<byte>.Shared,
                doNotDisposeRequestStream,
                doNotDisposeResponseStream)
        {
        }

        public WrxCommandClient(
            IAsyncSimpleOutputStream<byte> requestStream,
            IAsyncSimpleInputStream<byte> responseStream,
            ArrayPool<byte> byteArrayPool,
            bool doNotDisposeRequestStream = false,
            bool doNotDisposeResponseStream = false) :
            this(
                new WrxPacketWriter(requestStream, doNotDisposeRequestStream),
                new WrxPacketReader(responseStream, doNotDisposeResponseStream),
                byteArrayPool)
        {
        }

        public WrxCommandClient(
            IPacketWriter<WrxPacket> requestWriter,
            IPacketReader<WrxPacket> responseReader,
            bool doNotDisposeRequestWriter = false,
            bool doNotDisposeResponseReader = false) :
            this(
                requestWriter,
                responseReader,
                ArrayPool<byte>.Shared,
                doNotDisposeRequestWriter,
                doNotDisposeResponseReader)
        {
        }

        public WrxCommandClient(
            IPacketWriter<WrxPacket> requestWriter,
            IPacketReader<WrxPacket> responseReader,
            ArrayPool<byte> byteArrayPool,
            bool doNotDisposeRequestWriter = false,
            bool doNotDisposeResponseReader = false)
        {
            _requestWriter = AsyncDisposableArg.Create(requestWriter, doNotDisposeRequestWriter);
            _responseReader = AsyncDisposableArg.Create(responseReader, doNotDisposeResponseReader);
            _byteArrayPool = byteArrayPool;
        }
        #endregion

        #region Methods
        public ValueTask AuthorizeSettingsPasswordAsync(string value, CancellationToken cancellationToken = default)
        {
            return AuthorizeDataAsync(WrxDataCommands.SettingsPassword, String(32), value, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _responseReader.DisposeAsync().ConfigureAwait(false);
            await _requestWriter.DisposeAsync().ConfigureAwait(false);
        }

        public ValueTask<uint> GetBatteryVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.BatteryVoltageMillivolts, UInt32, cancellationToken);
        }

        public ValueTask<WrxDiagnosticsLevel> GetDiagnosticsLevelAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.DiagnosticsLevel, Enum<WrxDiagnosticsLevel>(Byte), cancellationToken);
        }

        public ValueTask<string> GetGsmImeiAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.GsmImei, String(15), cancellationToken);
        }

        public ValueTask<string> GetGsmIpAddressAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.GsmIpAddress, String(32), cancellationToken);
        }

        public ValueTask<WrxGsmMode> GetGsmModeAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.GsmMode, Enum<WrxGsmMode>(Byte), cancellationToken);
        }

        public ValueTask<byte> GetGsmSignalStrengthAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.GsmSignalStrength, Byte, cancellationToken);
        }

        public ValueTask<uint> GetInputVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.InputVoltageMillivolts, UInt32, cancellationToken);
        }

        public ValueTask<uint> GetLocalTimeSecondsAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.LocalTimeSeconds, UInt32, cancellationToken);
        }

        public ValueTask<ushort> GetOutputAlarmDueTimeMinutesAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.OutputAlarmDueTimeMinutes, UInt16, cancellationToken);
        }

        public ValueTask<uint> GetOutputAlarmDurationSecondsAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.OutputAlarmDurationSeconds, UInt32, cancellationToken);
        }

        public ValueTask<uint> GetOutputAlarmScheduleAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.OutputAlarmSchedule, UInt32, cancellationToken);
        }

        public ValueTask<WrxAlarmType> GetOutputAlarmTypeAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.OutputAlarmType, Enum<WrxAlarmType>(Byte), cancellationToken);
        }

        public ValueTask<byte> GetOutputStateAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.OutputState, Byte, cancellationToken);
        }

        public ValueTask<uint> GetSupplyVoltageMillivoltsAsync(CancellationToken cancellationToken = default)
        {
            return GetDataAsync(WrxDataCommands.SupplyVoltageMillivolts, UInt32, cancellationToken);
        }

        public ValueTask SetDiagnosticsLevelAsync(WrxDiagnosticsLevel value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.DiagnosticsLevel, Enum<WrxDiagnosticsLevel>(Byte), value, cancellationToken);
        }

        public ValueTask SetLocalTimeSecondsAsync(uint value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.LocalTimeSeconds, UInt32, value, cancellationToken);
        }

        public ValueTask SetOutputAlarmDueTimeMinutesAsync(ushort value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.OutputAlarmDueTimeMinutes, UInt16, value, cancellationToken);
        }

        public ValueTask SetOutputAlarmDurationSecondsAsync(uint value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.OutputAlarmDurationSeconds, UInt32, value, cancellationToken);
        }

        public ValueTask SetOutputAlarmScheduleAsync(uint value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.OutputAlarmSchedule, UInt32, value, cancellationToken);
        }

        public ValueTask SetOutputAlarmTypeAsync(WrxAlarmType value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.OutputAlarmType, Enum<WrxAlarmType>(Byte), value, cancellationToken);
        }

        public ValueTask SetOutputStateAsync(WrxOutputState value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.OutputState, Enum<WrxOutputState>(Byte), value, cancellationToken);
        }

        public ValueTask SetOutputStateWithSwitchbackSecondsAsync((WrxOutputState, uint) value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.OutputStateWithSwitchbackSeconds, Tuple(Enum<WrxOutputState>(Byte), UInt32), value, cancellationToken);
        }

        public ValueTask SetResetAsync(uint value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.Reset, UInt32, value, cancellationToken);
        }

        public ValueTask SetSettingsPasswordAsync(string value, CancellationToken cancellationToken = default)
        {
            return SetDataAsync(WrxDataCommands.SettingsPassword, String(32), value, cancellationToken);
        }

        private async ValueTask AuthorizeDataAsync<T>(int command, IVariableSizeDataInfo<T> dataInfo, T value, CancellationToken cancellationToken)
        {
            var dataArray = _byteArrayPool.Rent(dataInfo.MaximumSizeInBytes);

            try
            {
                var data = dataArray.AsMemory();
                var count = dataInfo.Write(data.Span, value);

                using var requestPacket = new WrxPacket(0, WrxPacketAction.AuthorizeDataRequest, command, data[..count]);
                using var responsePacket = await ProcessAsync(requestPacket, cancellationToken).ConfigureAwait(false);
                CheckAndGetData(WrxPacketAction.AuthorizeDataResponse, command, responsePacket);
            }
            finally
            {
                _byteArrayPool.Return(dataArray);
            }
        }

        private ReadOnlyMemory<byte> CheckAndGetData(WrxPacketAction expectedAction, int expectedCommand, WrxPacket packet)
        {
            var (_, action, command, data) = packet;

            if (action != expectedAction)
            {
                throw new InvalidDataException($"The response contains an invalid action. Expects: {expectedAction}");
            }

            if (command == expectedCommand)
            {
                return data;
            }
            else if (command == 0xFF && data.Span[0] == (byte)expectedCommand)
            {
                throw new WrxException((WrxPacketError)data.Span[1]);
            }
            else
            {
                // TODO: Add exception message.
                throw new InvalidDataException();
            }
        }

        private ValueTask<T> GetDataAsync<T>(int command, IFixedSizeDataInfo<T> dataInfo, CancellationToken cancellationToken)
        {
            return GetDataAsync(command, FromFixed(dataInfo), cancellationToken);
        }

        private async ValueTask<T> GetDataAsync<T>(int command, IVariableSizeDataInfo<T> dataInfo, CancellationToken cancellationToken)
        {
            using var requestPacket = new WrxPacket(0, WrxPacketAction.GetDataRequest, command);
            using var responsePacket = await ProcessAsync(requestPacket, cancellationToken).ConfigureAwait(false);
            return dataInfo.Read(CheckAndGetData(WrxPacketAction.GetDataResponse, command, responsePacket).Span);
        }

        private async ValueTask<WrxPacket> ProcessAsync(WrxPacket packet, CancellationToken cancellationToken)
        {
            await _requestWriter.Value.WriteAsync(packet, cancellationToken).ConfigureAwait(false);
            return await _responseReader.Value.ReadAsync(cancellationToken).ConfigureAwait(false);
        }

        private ValueTask SetDataAsync<T>(int command, IFixedSizeDataInfo<T> dataInfo, T value, CancellationToken cancellationToken)
        {
            return SetDataAsync(command, FromFixed(dataInfo), value, cancellationToken);
        }

        private async ValueTask SetDataAsync<T>(int command, IVariableSizeDataInfo<T> dataInfo, T value, CancellationToken cancellationToken)
        {
            var dataArray = _byteArrayPool.Rent(dataInfo.MaximumSizeInBytes);

            try
            {
                var data = dataArray.AsMemory();
                var count = dataInfo.Write(data.Span, value);

                using var requestPacket = new WrxPacket(0, WrxPacketAction.SetDataRequest, command, data[..count]);
                using var responsePacket = await ProcessAsync(requestPacket, cancellationToken).ConfigureAwait(false);
                CheckAndGetData(WrxPacketAction.SetDataResponse, command, responsePacket);
            }
            finally
            {
                _byteArrayPool.Return(dataArray);
            }
        }
        #endregion
    }
}
