using Swsu.StreetLights.Common;
using Swsu.StreetLights.Common.IO;
using Swsu.StreetLights.Common.Packets;
using Swsu.StreetLights.Protocols.Teleofis.Infrastructure;
using Swsu.StreetLights.Protocols.Teleofis.Packets;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Protocols.Teleofis
{
    public partial class WrxCommandServer : IAsyncDisposable
    {
        #region Fields
        private readonly ArrayPool<byte> _byteArrayPool;

        private readonly IAsyncWrxCommandHandler _commandHandler;

        private readonly Action<object?> _disposePacketCallback;

        private readonly AsyncDisposableArg<IPacketReader<WrxPacket>> _requestReader;

        private readonly AsyncDisposableArg<IPacketWriter<WrxPacket>> _responseWriter;
        #endregion

        #region Constructors
        public WrxCommandServer(
            IAsyncSimpleInputStream<byte> requestStream,
            IAsyncSimpleOutputStream<byte> responseStream,
            IAsyncWrxCommandHandler commandHandler,
            bool doNotDisposeRequestStream = false,
            bool doNotDisposeResponseStream = false) :
            this(
                new WrxPacketReader(requestStream, doNotDisposeRequestStream),
                new WrxPacketWriter(responseStream, doNotDisposeResponseStream),
                commandHandler,
                ArrayPool<byte>.Shared)
        {
        }

        public WrxCommandServer(
            IAsyncSimpleInputStream<byte> requestStream,
            IAsyncSimpleOutputStream<byte> responseStream,
            IAsyncWrxCommandHandler commandHandler,
            ArrayPool<byte> byteArrayPool,
            bool doNotDisposeRequestStream = false,
            bool doNotDisposeResponseStream = false) :
            this(
                new WrxPacketReader(requestStream, doNotDisposeRequestStream),
                new WrxPacketWriter(responseStream, doNotDisposeResponseStream),
                commandHandler,
                byteArrayPool)
        {
        }

        public WrxCommandServer(
            IPacketReader<WrxPacket> requestReader,
            IPacketWriter<WrxPacket> responseWriter,
            IAsyncWrxCommandHandler commandHandler,
            bool doNotDisposeRequestReader = false,
            bool doNotDisposeResponseWriter = false) :
            this(
                requestReader,
                responseWriter,
                commandHandler,
                ArrayPool<byte>.Shared,
                doNotDisposeRequestReader,
                doNotDisposeResponseWriter)
        {
        }

        public WrxCommandServer(
            IPacketReader<WrxPacket> requestReader,
            IPacketWriter<WrxPacket> responseWriter,
            IAsyncWrxCommandHandler commandHandler,
            ArrayPool<byte> byteArrayPool,
            bool doNotDisposeRequestReader = false,
            bool doNotDisposeResponseWriter = false)
        {
            _requestReader = AsyncDisposableArg.Create(requestReader, doNotDisposeRequestReader);
            _responseWriter = AsyncDisposableArg.Create(responseWriter, doNotDisposeResponseWriter);
            _commandHandler = commandHandler;
            _byteArrayPool = byteArrayPool;

            _disposePacketCallback = state =>
            {
                if (state is byte[] array)
                {
                    _byteArrayPool.Return(array);
                }
            };
        }
        #endregion

        #region Methods     
        public async ValueTask ProcessNextAsync(CancellationToken cancellationToken = default)
        {
            using var requestPacket = await _requestReader.Value.ReadAsync(cancellationToken).ConfigureAwait(false);
            using var responsePacket = await ProcessPacketAsync(requestPacket, cancellationToken).ConfigureAwait(false);
            await _responseWriter.Value.WriteAsync(responsePacket, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await _responseWriter.DisposeAsync();
            await _requestReader.DisposeAsync();
        }

        private WrxPacket CreateErrorPacket(WrxPacketAction action, int command, WrxPacketError error)
        {
            var byteArray = _byteArrayPool.Rent(2);
            var bytes = byteArray.AsMemory(..2);

            bytes.Span[0] = (byte)command;
            bytes.Span[1] = (byte)error;

            return new WrxPacket(0, action, 0xFF, bytes, _disposePacketCallback, byteArray);
        }

        private async ValueTask<WrxPacket> ProcessPacketAsync(WrxPacket requestPacket, CancellationToken cancellationToken)
        {
            var (protocol, requestAction, command, requestData) = requestPacket;

            if (!TryGetActionEntry(requestAction, out var actionEntry))
            {
                throw new Exception("Unknown action.");
            }

            var responseAction = actionEntry.ResponseAction;

            if (!TryGetCommandEntry(command, out var commandEntry) || !actionEntry.CanProcessCommand(commandEntry))
            {
                return CreateErrorPacket(responseAction, command, WrxPacketError.CommandNotSupported);
            }

            var errorDataSize = WrxErrorData.GetSize(protocol);
            var responseDataSize = Math.Max(actionEntry.GetMaximumResponseDataSize(commandEntry), errorDataSize);
            var byteArray = _byteArrayPool.Rent(responseDataSize);

            try
            {
                var responseData = byteArray.AsMemory();
                int count;

                try
                {
                    count = await actionEntry.ProcessCommandAsync(commandEntry, _commandHandler, requestData, responseData, cancellationToken).ConfigureAwait(false);
                }
                catch (WrxException ex)
                {
                    count = errorDataSize;
                    new WrxErrorData(command, ex.Error).Write(protocol, responseData.Span);
                    command = 0xFF;
                }

                return new WrxPacket(protocol, responseAction, command, responseData[..count]);
            }
            catch
            {
                _byteArrayPool.Return(byteArray);
                throw;
            }
        }

        private static bool TryGetActionEntry(WrxPacketAction action, out ActionEntry value)
        {
            switch (action)
            {
                case WrxPacketAction.AuthorizeDataRequest:
                    value = AuthorizeActionEntry.Instance;
                    return true;

                case WrxPacketAction.GetDataRequest:
                    value = GetActionEntry.Instance;
                    return true;

                case WrxPacketAction.SetDataRequest:
                    value = SetActionEntry.Instance;
                    return true;

                default:
                    value = default!;
                    return false;
            }
        }

        private static bool TryGetCommandEntry(int command, out CommandEntry value)
        {
            return Commands.TryGetEntry(command, out value);
        }
        #endregion

        #region Nested Types
        private abstract class ActionEntry
        {
            #region Constructors
            private protected ActionEntry()
            {
            }
            #endregion

            #region Properties
            internal abstract WrxPacketAction ResponseAction
            {
                get;
            }
            #endregion

            #region Methods
            internal abstract bool CanProcessCommand(CommandEntry commandEntry);

            internal abstract int GetMaximumResponseDataSize(CommandEntry commandEntry);

            internal abstract ValueTask<int> ProcessCommandAsync(CommandEntry commandEntry, IAsyncWrxCommandHandler commandHandler, ReadOnlyMemory<byte> requestData, Memory<byte> responseData, CancellationToken cancellationToken);
            #endregion
        }

        private sealed class AuthorizeActionEntry : ActionEntry
        {
            #region Fields
            internal static readonly AuthorizeActionEntry Instance = new AuthorizeActionEntry();
            #endregion

            #region Constructors
            private AuthorizeActionEntry()
            {
            }
            #endregion

            #region Properties
            internal override WrxPacketAction ResponseAction
            {
                get
                {
                    return WrxPacketAction.AuthorizeDataResponse;
                }
            }
            #endregion

            #region Methods
            internal override bool CanProcessCommand(CommandEntry commandEntry)
            {
                return commandEntry.CanAuthorizeData;
            }

            internal override int GetMaximumResponseDataSize(CommandEntry commandEntry)
            {
                return 0;
            }

            internal async override ValueTask<int> ProcessCommandAsync(CommandEntry commandEntry, IAsyncWrxCommandHandler commandHandler, ReadOnlyMemory<byte> requestData, Memory<byte> responseData, CancellationToken cancellationToken)
            {
                await commandEntry.AuthorizeDataAsync(commandHandler, requestData, cancellationToken).ConfigureAwait(false);
                return 0;
            }
            #endregion
        }

        private abstract class CommandEntry
        {
            #region Constructors
            private protected CommandEntry()
            {
            }
            #endregion

            #region Properties
            internal abstract int MaximumDataSize
            {
                get;
            }

            internal abstract bool CanAuthorizeData
            {
                get;
            }

            internal abstract bool CanGetData
            {
                get;
            }

            internal abstract bool CanSetData
            {
                get;
            }
            #endregion

            #region Methods
            internal abstract ValueTask AuthorizeDataAsync(IAsyncWrxCommandHandler handler, ReadOnlyMemory<byte> data, CancellationToken cancellationToken);

            internal abstract ValueTask<int> GetDataAsync(IAsyncWrxCommandHandler handler, Memory<byte> data, CancellationToken cancellationToken);

            internal abstract ValueTask SetDataAsync(IAsyncWrxCommandHandler handler, ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
            #endregion
        }

        private class CommandEntry<T> : CommandEntry
        {
            #region Fields
            private readonly IVariableSizeDataInfo<T> _dataInfo;

            private readonly AuthorizeDataCallback? _onAuthorizeData;

            private readonly GetDataCallback? _onGetData;

            private readonly SetDataCallback? _onSetData;
            #endregion

            #region Constructors
            internal CommandEntry(
                IVariableSizeDataInfo<T> dataInfo,
                GetDataCallback? onGetData = default,
                SetDataCallback? onSetData = default,
                AuthorizeDataCallback? onAuthorizeData = default)
            {
                _dataInfo = dataInfo;
                _onGetData = onGetData;
                _onSetData = onSetData;
                _onAuthorizeData = onAuthorizeData;
            }

            internal CommandEntry(
                IFixedSizeDataInfo<T> dataInfo,
                GetDataCallback? onGetData = default,
                SetDataCallback? onSetData = default,
                AuthorizeDataCallback? onAuthorizeData = default) :
                this(VariableSizeDataInfo.FromFixed(dataInfo), onGetData, onSetData, onAuthorizeData)
            {
            }
            #endregion

            #region Properties
            internal override int MaximumDataSize
            {
                get
                {
                    return _dataInfo.MaximumSizeInBytes;
                }
            }

            internal override bool CanAuthorizeData
            {
                get
                {
                    return _onAuthorizeData != null;
                }
            }

            internal override bool CanGetData
            {
                get
                {
                    return _onGetData != null;
                }
            }

            internal override bool CanSetData
            {
                get
                {
                    return _onSetData != null;
                }
            }
            #endregion

            #region Methods
            internal override ValueTask AuthorizeDataAsync(IAsyncWrxCommandHandler handler, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            {
                if (_onAuthorizeData == null)
                {
                    throw new InvalidOperationException();
                }

                var value = _dataInfo.Read(data.Span);
                return _onAuthorizeData(handler, value, cancellationToken);
            }

            internal override async ValueTask<int> GetDataAsync(IAsyncWrxCommandHandler handler, Memory<byte> data, CancellationToken cancellationToken)
            {
                if (_onGetData == null)
                {
                    throw new InvalidOperationException();
                }

                var value = await _onGetData(handler, cancellationToken).ConfigureAwait(false);
                return _dataInfo.Write(data.Span, value);
            }

            internal override ValueTask SetDataAsync(IAsyncWrxCommandHandler handler, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            {
                if (_onSetData == null)
                {
                    throw new InvalidOperationException();
                }

                var value = _dataInfo.Read(data.Span);
                return _onSetData(handler, value, cancellationToken);
            }
            #endregion

            #region Nested Types
            internal delegate ValueTask AuthorizeDataCallback(IAsyncWrxCommandHandler handler, T value, CancellationToken cancellationToken);

            internal delegate ValueTask<T> GetDataCallback(IAsyncWrxCommandHandler handler, CancellationToken cancellationToken);

            internal delegate ValueTask SetDataCallback(IAsyncWrxCommandHandler handler, T value, CancellationToken cancellationToken);
            #endregion
        }

        private sealed class GetActionEntry : ActionEntry
        {
            #region Fields
            internal static readonly GetActionEntry Instance = new GetActionEntry();
            #endregion

            #region Constructors
            private GetActionEntry()
            {
            }
            #endregion

            #region Properties
            internal override WrxPacketAction ResponseAction
            {
                get
                {
                    return WrxPacketAction.GetDataResponse;
                }
            }
            #endregion

            #region Methods
            internal override bool CanProcessCommand(CommandEntry commandEntry)
            {
                return commandEntry.CanGetData;
            }

            internal override int GetMaximumResponseDataSize(CommandEntry commandEntry)
            {
                return commandEntry.MaximumDataSize;
            }

            internal async override ValueTask<int> ProcessCommandAsync(CommandEntry commandEntry, IAsyncWrxCommandHandler commandHandler, ReadOnlyMemory<byte> requestData, Memory<byte> responseData, CancellationToken cancellationToken)
            {
                return await commandEntry.GetDataAsync(commandHandler, responseData, cancellationToken).ConfigureAwait(false);
            }
            #endregion
        }

        //private readonly struct ProcessingResult
        //{
        //    #region Fields
        //    private readonly WrxPacketError _error;
        //    #endregion

        //    #region Constructors
        //    internal ProcessingResult(ProcessingResultTag tag, WrxPacketError error = default)
        //    {
        //        Tag = tag;
        //        _error = error;
        //    }
        //    #endregion

        //    #region Properties
        //    internal WrxPacketError Error
        //    {
        //        get
        //        {
        //            if (Tag != ProcessingResultTag.Failure)
        //            {
        //                throw new InvalidOperationException();
        //            }

        //            return _error;
        //        }
        //    }

        //    internal ProcessingResultTag Tag
        //    {
        //        get;
        //    }
        //    #endregion
        //}

        //private readonly struct ProcessingResult<T>
        //{
        //    #region Fields
        //    private readonly WrxPacketError _error;

        //    private readonly T _value;
        //    #endregion

        //    #region Constructors
        //    internal ProcessingResult(ProcessingResultTag tag, T value = default, WrxPacketError error = default)
        //    {
        //        Tag = tag;
        //        _value = value;
        //        _error = error;
        //    }
        //    #endregion

        //    #region Properties
        //    internal WrxPacketError Error
        //    {
        //        get
        //        {
        //            if (Tag != ProcessingResultTag.Failure)
        //            {
        //                throw new InvalidOperationException();
        //            }

        //            return _error;
        //        }
        //    }

        //    internal ProcessingResultTag Tag
        //    {
        //        get;
        //    }

        //    internal T Value
        //    {
        //        get
        //        {
        //            if (Tag != ProcessingResultTag.Success)
        //            {
        //                throw new InvalidOperationException();
        //            }

        //            return _value;
        //        }
        //    }
        //    #endregion

        //    #region Operators
        //    public static implicit operator ProcessingResult<T>(T value)
        //    {
        //        return new ProcessingResult<T>(ProcessingResultTag.Success, value, default);
        //    }

        //    public static implicit operator ProcessingResult<T>(WrxPacketError value)
        //    {
        //        return new ProcessingResult<T>(ProcessingResultTag.Failure, default!, value);
        //    }
        //    #endregion
        //}

        //private enum ProcessingResultTag
        //{
        //    Success,
        //    Failure
        //}

        private sealed class SetActionEntry : ActionEntry
        {
            #region Fields
            internal static readonly SetActionEntry Instance = new SetActionEntry();
            #endregion

            #region Constructors
            private SetActionEntry()
            {
            }
            #endregion

            #region Properties
            internal override WrxPacketAction ResponseAction
            {
                get
                {
                    return WrxPacketAction.SetDataResponse;
                }
            }
            #endregion

            #region Methods
            internal override bool CanProcessCommand(CommandEntry commandEntry)
            {
                return commandEntry.CanSetData;
            }

            internal override int GetMaximumResponseDataSize(CommandEntry commandEntry)
            {
                return 0;
            }

            internal async override ValueTask<int> ProcessCommandAsync(CommandEntry commandEntry, IAsyncWrxCommandHandler commandHandler, ReadOnlyMemory<byte> requestData, Memory<byte> responseData, CancellationToken cancellationToken)
            {
                await commandEntry.SetDataAsync(commandHandler, requestData, cancellationToken).ConfigureAwait(false);
                return 0;
            }
            #endregion
        }
        #endregion
    }
}
