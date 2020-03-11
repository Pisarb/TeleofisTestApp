using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Swsu.StreetLights.Common
{
    public class MultiValueTaskSource<TResult> : IValueTaskSource<TResult>
    {
        // TODO: Reduce heap usage!!!

        #region Fields
        private readonly ConcurrentDictionary<short, Entry> _entryByToken = new ConcurrentDictionary<short, Entry>();
        #endregion

        #region Constructors
        public MultiValueTaskSource()
        {
        }
        #endregion

        #region Methods
        public TResult GetResult(short token)
        {
            return GetEntry(token).GetResult();
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return GetEntry(token).Status;
        }

        public ValueTask<TResult> GetTask(short token)
        {
            return new ValueTask<TResult>(this, token);
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            GetEntry(token).OnCompleted(continuation, state, flags);
        }

        public void SetCanceled(short token, CancellationToken cancellationToken)
        {
            GetEntry(token).SetCanceled(cancellationToken);
        }

        public void SetFaulted(short token, Exception exception)
        {
            GetEntry(token).SetFaulted(exception);
        }

        public void SetSucceeded(short token, TResult result)
        {
            GetEntry(token).SetSucceeded(result);
        }

        public bool TrySetCanceled(short token, CancellationToken cancellationToken)
        {
            return GetEntry(token).TrySetCanceled(cancellationToken);
        }

        public bool TrySetFaulted(short token, Exception exception)
        {
            return GetEntry(token).TrySetFaulted(exception);
        }

        public bool TrySetSucceeded(short token, TResult result)
        {
            return GetEntry(token).TrySetSucceeded(result);
        }

        private Entry GetEntry(short token)
        {
            return _entryByToken.GetOrAdd(token, _1 => new Entry());
        }
        #endregion

        #region Nested Types
        private class Entry
        {
            #region Fields
            private readonly ReaderWriterLockSlim _lock;

            private State _state;
            #endregion

            #region Constructors
            public Entry()
            {
                _lock = new ReaderWriterLockSlim();
                _state = new PendingState();
            }
            #endregion

            #region Properties
            internal ValueTaskSourceStatus Status
            {
                get
                {
                    _lock.EnterReadLock();

                    try
                    {
                        return _state.Status;
                    }
                    finally
                    {
                        _lock.ExitReadLock();
                    }
                }
            }
            #endregion

            #region Methods
            internal TResult GetResult()
            {
                _lock.EnterReadLock();

                try
                {
                    return _state.GetResult();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            internal void OnCompleted(Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags)
            {
                Action<Action<object>, object> postscript;

                _lock.EnterWriteLock();

                try
                {
                    _state.OnCompleted(out postscript, continuation, state, flags);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                postscript(continuation, state);
            }

            internal void SetCanceled(CancellationToken cancellationToken)
            {
                Action postscript;

                _lock.EnterWriteLock();

                try
                {
                    _state.SetCanceled(out _state, out postscript, cancellationToken);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                postscript();
            }

            internal void SetFaulted(Exception exception)
            {
                Action postscript;

                _lock.EnterWriteLock();

                try
                {
                    _state.SetFaulted(out _state, out postscript, exception);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                postscript();
            }

            internal void SetSucceeded(TResult result)
            {
                Action postscript;

                _lock.EnterWriteLock();

                try
                {
                    _state.SetSucceeded(out _state, out postscript, result);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                postscript();
            }

            internal bool TrySetCanceled(CancellationToken cancellationToken)
            {
                Func<bool> postscript;

                _lock.EnterWriteLock();

                try
                {
                    _state.TrySetCanceled(out _state, out postscript, cancellationToken);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                return postscript();
            }

            internal bool TrySetFaulted(Exception exception)
            {
                Func<bool> postscript;

                _lock.EnterWriteLock();

                try
                {
                    _state.TrySetFaulted(out _state, out postscript, exception);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                return postscript();
            }

            internal bool TrySetSucceeded(TResult result)
            {
                Func<bool> postscript;

                _lock.EnterWriteLock();

                try
                {
                    _state.TrySetSucceeded(out _state, out postscript, result);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                return postscript();
            }
            #endregion

            #region Nested Types
            private class CanceledState : FinalState
            {
                #region Fields
                private readonly CancellationToken _cancellationToken;
                #endregion

                #region Constructors
                internal CanceledState(CancellationToken cancellationToken)
                {
                    _cancellationToken = cancellationToken;
                }
                #endregion

                #region Properties
                internal override ValueTaskSourceStatus Status
                {
                    get
                    {
                        return ValueTaskSourceStatus.Canceled;
                    }
                }
                #endregion

                #region Methods
                internal override TResult GetResult()
                {
                    throw new OperationCanceledException(_cancellationToken);
                }
                #endregion
            }

            private class PendingState : State
            {
                #region Fields
                private readonly Queue<Callback> _callbacks = new Queue<Callback>();
                #endregion

                #region Constructors
                internal PendingState()
                {
                }
                #endregion

                #region Properties
                internal override ValueTaskSourceStatus Status
                {
                    get
                    {
                        return ValueTaskSourceStatus.Pending;
                    }
                }
                #endregion

                #region Methods
                internal override TResult GetResult()
                {
                    throw new InvalidOperationException();
                }

                internal override void OnCompleted(out Action<Action<object>, object> postscript, Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags)
                {
                    _callbacks.Enqueue(new Callback(continuation, state, flags));
                    postscript = (c, s) => { };
                }

                internal override void SetCanceled(out State state, out Action postscript, CancellationToken cancellationToken)
                {
                    state = new CanceledState(cancellationToken);
                    postscript = Postscript;
                }

                internal override void SetFaulted(out State state, out Action postscript, Exception exception)
                {
                    state = new FaultedState(exception);
                    postscript = Postscript;
                }

                internal override void SetSucceeded(out State state, out Action postscript, TResult result)
                {
                    state = new SucceededState(result);
                    postscript = Postscript;
                }

                internal override void TrySetCanceled(out State state, out Func<bool> postscript, CancellationToken cancellationToken)
                {
                    state = new CanceledState(cancellationToken);
                    postscript = TryPostscript;
                }

                internal override void TrySetFaulted(out State state, out Func<bool> postscript, Exception exception)
                {
                    state = new FaultedState(exception);
                    postscript = TryPostscript;
                }

                internal override void TrySetSucceeded(out State state, out Func<bool> postscript, TResult result)
                {
                    state = new SucceededState(result);
                    postscript = TryPostscript;
                }

                private void DoCallbacks()
                {
                    while (_callbacks.TryDequeue(out var callback))
                    {
                        var (continuation, state, _) = callback;
                        continuation(state);
                    }
                }

                private void Postscript()
                {
                    DoCallbacks();
                }

                private bool TryPostscript()
                {
                    DoCallbacks();
                    return true;
                }
                #endregion

                #region Nested Types
                private readonly struct Callback
                {
                    #region Fields
                    internal readonly Action<object> Continuation;

                    internal readonly ValueTaskSourceOnCompletedFlags Flags;

                    internal readonly object State;
                    #endregion

                    #region Constructors
                    internal Callback(Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags)
                    {
                        Continuation = continuation;
                        State = state;
                        Flags = flags;
                    }
                    #endregion

                    #region Methods
                    internal void Deconstruct(out Action<object> continuation, out object state, out ValueTaskSourceOnCompletedFlags flags)
                    {
                        continuation = Continuation;
                        state = State;
                        flags = Flags;
                    }
                    #endregion
                }
                #endregion
            }

            private class FaultedState : FinalState
            {
                #region Fields
                private readonly ExceptionDispatchInfo _exceptionDispatchInfo;
                #endregion

                #region Constructors
                internal FaultedState(Exception exception)
                {
                    _exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                #endregion

                #region Properties
                internal override ValueTaskSourceStatus Status
                {
                    get
                    {
                        return ValueTaskSourceStatus.Faulted;
                    }
                }
                #endregion

                #region Methods
                internal override TResult GetResult()
                {
                    _exceptionDispatchInfo.Throw();
                    throw new ShouldNeverHappenException();
                }
                #endregion
            }

            private abstract class FinalState : State
            {
                #region Constructors
                private protected FinalState()
                {
                }
                #endregion

                #region Methods
                internal sealed override void OnCompleted(out Action<Action<object>, object> postscript, Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags)
                {
                    postscript = (c, s) => c(s);
                }

                internal sealed override void SetCanceled(out State state, out Action postscript, CancellationToken cancellationToken)
                {
                    state = this;
                    postscript = () => throw new InvalidOperationException();
                }

                internal sealed override void SetFaulted(out State state, out Action postscript, Exception exception)
                {
                    state = this;
                    postscript = () => throw new InvalidOperationException();
                }

                internal sealed override void SetSucceeded(out State state, out Action postscript, TResult result)
                {
                    state = this;
                    postscript = () => throw new InvalidOperationException();
                }

                internal sealed override void TrySetCanceled(out State state, out Func<bool> postscript, CancellationToken cancellationToken)
                {
                    state = this;
                    postscript = () => false;
                }

                internal sealed override void TrySetFaulted(out State state, out Func<bool> postscript, Exception exception)
                {
                    state = this;
                    postscript = () => false;
                }

                internal sealed override void TrySetSucceeded(out State state, out Func<bool> postscript, TResult result)
                {
                    state = this;
                    postscript = () => false;
                }
                #endregion
            }

            private abstract class State
            {
                #region Constructors
                private protected State()
                {
                }
                #endregion

                #region Properties
                internal abstract ValueTaskSourceStatus Status
                {
                    get;
                }
                #endregion

                #region Methods
                internal abstract TResult GetResult();

                internal abstract void OnCompleted(out Action<Action<object>, object> postscript, Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags);

                internal abstract void SetCanceled(out State state, out Action postscript, CancellationToken cancellationToken);

                internal abstract void SetFaulted(out State state, out Action postscript, Exception exception);

                internal abstract void SetSucceeded(out State state, out Action postscript, TResult result);

                internal abstract void TrySetCanceled(out State state, out Func<bool> postscript, CancellationToken cancellationToken);

                internal abstract void TrySetFaulted(out State state, out Func<bool> postscript, Exception exception);

                internal abstract void TrySetSucceeded(out State state, out Func<bool> postscript, TResult result);
                #endregion
            }

            private class SucceededState : FinalState
            {
                #region Fields
                private readonly TResult _result;
                #endregion

                #region Constructors
                internal SucceededState(TResult result)
                {
                    _result = result;
                }
                #endregion

                #region Properties
                internal override ValueTaskSourceStatus Status
                {
                    get
                    {
                        return ValueTaskSourceStatus.Succeeded;
                    }
                }
                #endregion

                #region Methods
                internal override TResult GetResult()
                {
                    return _result;
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
