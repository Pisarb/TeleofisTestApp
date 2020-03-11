using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Net
{
    public static class SocketExtensions
    {
        #region Methods
        public async static ValueTask<Socket> AcceptAsync(this Socket socket, CancellationToken cancellationToken = default)
        {
            return await FromAsyncEventArgs(
                socket,
                (s, e, a) => s.AcceptAsync(e),
                (e) => e.AcceptSocket,
                (e) => { },
                Unit.Default,
                cancellationToken);
        }

        public async static ValueTask ConnectAsync(this Socket socket, EndPoint remoteEP, CancellationToken cancellationToken = default)
        {
            await FromAsyncEventArgs(
                socket,
                (s, e, a) =>
                {
                    e.RemoteEndPoint = a.RemoteEP;
                    return s.ConnectAsync(e);
                },
                (s) => Unit.Default,
                (e) => Socket.CancelConnectAsync(e),
                new { RemoteEP = remoteEP },
                cancellationToken);
        }

        internal static async ValueTask<TResult> FromAsyncEventArgs<TArg, TResult>(
            Socket socket,
            Func<Socket, SocketAsyncEventArgs, TArg, bool> startCallback,
            Func<SocketAsyncEventArgs, TResult> endCallback,
            Action<SocketAsyncEventArgs> cancelCallback,
            TArg arg,
            CancellationToken cancellationToken)
        {
            // TODO: Use pooling for SocketAsyncEventArgs instances.
            using var e = new CustomSocketAsyncEventArgs<TResult>(endCallback, cancelCallback, cancellationToken);

            if (!startCallback(socket, e, arg))
            {
                e.SetTask(e);
            }

            using (cancellationToken.Register(Cancel<TResult>, e))
            {
                return await e.TaskHandle.Task;
            }
        }

        private static void Cancel<TResult>(object state)
        {
            var e = (CustomSocketAsyncEventArgs<TResult>)state;
            e.OnCancel();
            e.TaskHandle.TrySetCanceled(e.CancellationToken);
        }

        private static ValueTaskHandle<TResult> CreateTaskHandle<TResult>()
        {
            return new ValueTaskHandle<TResult>(new MultiValueTaskSource<TResult>(), 0);
        }
        #endregion

        #region Nested Types
        private class CustomSocketAsyncEventArgs<TResult> : SocketAsyncEventArgs
        {
            #region Fields
            internal readonly Action<SocketAsyncEventArgs> CancelCallback;

            internal readonly CancellationToken CancellationToken;

            internal readonly Func<SocketAsyncEventArgs, TResult> EndCallback;

            internal readonly ValueTaskHandle<TResult> TaskHandle;
            #endregion

            #region Constructors
            internal CustomSocketAsyncEventArgs(
                Func<SocketAsyncEventArgs, TResult> endCallback,
                Action<SocketAsyncEventArgs> cancelCallback,
                CancellationToken cancellationToken)
            {
                EndCallback = endCallback;
                CancelCallback = cancelCallback;
                CancellationToken = cancellationToken;

                TaskHandle = CreateTaskHandle<TResult>();
            }
            #endregion

            #region Methods
            internal void SetTask(SocketAsyncEventArgs e)
            {
                if (SocketError == SocketError.Success)
                {
                    TaskHandle.TrySetSucceeded(EndCallback(this));
                }
                else
                {
                    TaskHandle.TrySetFaulted(new SocketException((int)SocketError));
                }
            }

            internal void OnCancel()
            {
                CancelCallback(this);
            }

            protected override void OnCompleted(SocketAsyncEventArgs e)
            {
                base.OnCompleted(e);
                SetTask(e);
            }
            #endregion
        }
        #endregion
    }
}
