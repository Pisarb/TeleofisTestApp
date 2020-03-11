using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common
{
    // TODO: Rename to Rendezvous.
    // TODO: Reify MatchAsync as End{1,2}.CallAsync.
    // TODO: Implement End{1,2}.ShutdownAsync, throwing PeerShuttedDownException on peer end.
    public class MatchingQueue<TArg1, TArg2, TResult1, TResult2>
    {
        #region Fields
        private readonly Queue<Entry<TArg1, TResult1>> _entries1 = new Queue<Entry<TArg1, TResult1>>();

        private readonly Queue<Entry<TArg2, TResult2>> _entries2 = new Queue<Entry<TArg2, TResult2>>();

        private readonly object _lock = new object();

        private readonly Func<TArg1, TArg2, CancellationToken, CancellationToken, (TResult1, TResult2)> _matcher;
        #endregion

        #region Constructors
        // TODO: Remove 'matcher' parameter.
        public MatchingQueue(Func<TArg1, TArg2, CancellationToken, CancellationToken, (TResult1, TResult2)> matcher)
        {
            _matcher = matcher;
        }
        #endregion

        #region Methods
        // TODO: Add optional callback to clear 'arg' in case of cancellation.
        public ValueTask<TResult1> MatchAsync(TArg1 arg, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                return MatchAsync(arg, _entries1, _entries2, cancellationToken, (arg1, arg2, cancellationToken1, cancellationToken2) =>
                {
                    var (result1, result2) = _matcher(arg1, arg2, cancellationToken1, cancellationToken2);
                    return (result1, result2);
                });
            }
        }

        public ValueTask<TResult2> MatchAsync(TArg2 arg, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                return MatchAsync(arg, _entries2, _entries1, cancellationToken, (arg2, arg1, cancellationToken2, cancellationToken1) =>
                {
                    var (result1, result2) = _matcher(arg1, arg2, cancellationToken1, cancellationToken2);
                    return (result2, result1);
                });
            }
        }

        private static ValueTask<TResult> MatchAsync<TArg, TOtherArg, TResult, TOtherResult>(
            TArg arg,
            Queue<Entry<TArg, TResult>> entries,
            Queue<Entry<TOtherArg, TOtherResult>> otherEntries,
            CancellationToken cancellationToken,
            Func<TArg, TOtherArg, CancellationToken, CancellationToken, (TResult, TOtherResult)> matcher)
        {
            while (otherEntries.TryDequeue(out var otherEntry))
            {
                if (otherEntry.TryExtract(out var otherArg, out var otherTaskHandle, out var otherCancellationToken))
                {
                    try
                    {
                        var (result, otherResult) = matcher(arg, otherArg, cancellationToken, otherCancellationToken);
                        otherTaskHandle.SetSucceeded(otherResult);
                        return new ValueTask<TResult>(result);
                    }
                    catch (Exception ex)
                    {
                        otherTaskHandle.SetFaulted(ex);
                        throw;
                    }
                }
            }

            var taskHandle = CreateTaskHandle<TResult>();
            entries.Enqueue(new Entry<TArg, TResult>(arg, taskHandle, cancellationToken));
            return taskHandle.Task;
        }

        private static ValueTaskHandle<TResult> CreateTaskHandle<TResult>()
        {
            return new ValueTaskHandle<TResult>(new MultiValueTaskSource<TResult>(), 1);
        }
        #endregion

        #region Nested Types
        private class Entry<TArg, TResult>
        {
            #region Fields
            private Content? _content;
            #endregion

            #region Constructors
            internal Entry(TArg arg, ValueTaskHandle<TResult> taskHandle, CancellationToken cancellationToken)
            {
                _content = new Content(arg, taskHandle, cancellationToken.Register(OnCanceled, this));
            }
            #endregion

            #region Methods
            public bool TryExtract(out TArg arg, out ValueTaskHandle<TResult> taskHandle, out CancellationToken cancellationToken)
            {
                var content = Interlocked.Exchange(ref _content, null);

                if (content != null)
                {
                    content.Return(out arg, out taskHandle, out cancellationToken);
                    return true;
                }
                else
                {
                    (arg, taskHandle, cancellationToken) = (default!, default, default);
                    return false;
                }
            }

            private static void OnCanceled(object state)
            {
                var entry = (Entry<TArg, TResult>)state;

                if (entry.TryExtract(out var arg, out var taskHandle, out var cancellationToken))
                {
                    taskHandle.SetCanceled(cancellationToken);
                }
            }
            #endregion

            #region Nested Types
            private class Content
            {
                #region Fields
                private readonly TArg _arg;

                private readonly CancellationTokenRegistration _cancellationTokenRegistration;

                private readonly ValueTaskHandle<TResult> _taskHandle;
                #endregion

                #region Constructors
                internal Content(TArg arg, ValueTaskHandle<TResult> taskHandle, CancellationTokenRegistration cancellationTokenRegistration)
                {
                    _arg = arg;
                    _taskHandle = taskHandle;
                    _cancellationTokenRegistration = cancellationTokenRegistration;
                }
                #endregion

                #region Methods
                public void Return(out TArg arg, out ValueTaskHandle<TResult> taskHandle, out CancellationToken cancellationToken)
                {
                    taskHandle = _taskHandle;
                    arg = _arg;
                    cancellationToken = _cancellationTokenRegistration.Token;

                    _cancellationTokenRegistration.Dispose();
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
