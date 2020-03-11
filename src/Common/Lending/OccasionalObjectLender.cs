using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common.Lending
{
    /// <summary>
    /// Предоставляет доступ к объектам, указанным в методе <see cref="UseAsync(T, CancellationToken)"/>
    /// вплоть до остановки вызова этого метода.
    /// </summary>
    public class OccasionalObjectLender<T> : IAsyncObjectLender<T>
    {
        #region Fields
        private readonly MatchingQueue<ValueTask<Unit>, LendingData, LendingData, ValueTask<Unit>> _matchingQueue;
        #endregion

        #region Constructors
        public OccasionalObjectLender()
        {
            _matchingQueue = new MatchingQueue<ValueTask<Unit>, LendingData, LendingData, ValueTask<Unit>>(Match);
        }
        #endregion

        #region Methods
        public async ValueTask<IObjectLending<T>> LendAsync(CancellationToken cancellationToken = default)
        {
            var lendingDisposedTaskHandle = CreateTaskHandle<Unit>();
            var lendingData = await _matchingQueue.MatchAsync(lendingDisposedTaskHandle.Task, cancellationToken).ConfigureAwait(false);
            var lending = new Lending(lendingData, lendingDisposedTaskHandle);
            return lending;
        }

        /// <summary>
        /// Делает объект <paramref name="obj"/> доступным для использования.
        /// </summary>
        /// <param name="obj">Объект, предоставляемый в пользование.</param>
        /// <param name="stoppingToken">Сигнал, указывающий пользователю объекта, что он должен прекратить использование объекта.</param>
        /// <exception cref="OperationCanceledException">Сработал сигнал <paramref name="stoppingToken"/>.</exception>
        /// <exception cref="ObjectBrokenException">Пользователь отметил объект, как сломанный.</exception>
        public async ValueTask UseAsync(T obj, CancellationToken stoppingToken)
        {
            var lendingData = new LendingData(obj, stoppingToken);

            for (; ; )
            {
                stoppingToken.ThrowIfCancellationRequested();
                var lendingDisposedTask = await _matchingQueue.MatchAsync(lendingData, stoppingToken).ConfigureAwait(false);

                // Если пользователь объекта отметил его как сломанный,
                // то следующая строка выбросит исключение ObjectBrokenException.
                await lendingDisposedTask.ConfigureAwait(false);
            }
        }

        private ValueTaskHandle<TResult> CreateTaskHandle<TResult>()
        {
            // TODO: Reuse value task sources created earlier.
            return new ValueTaskHandle<TResult>(new MultiValueTaskSource<TResult>(), 0);
        }

        private (LendingData, ValueTask<Unit>) Match(ValueTask<Unit> disposedTask, LendingData data, CancellationToken _3, CancellationToken _4)
        {
            return (data, disposedTask);
        }
        #endregion

        #region Nested Types
        private class Lending : IObjectLending<T>
        {
            #region Fields
            private BreakageMark? _breakageMark;

            private readonly LendingData _data;

            private int _disposedFlag = 0;

            private readonly ValueTaskHandle<Unit> _disposedTaskHandle;
            #endregion

            #region Constructors
            internal Lending(LendingData data, ValueTaskHandle<Unit> disposedTaskHandle)
            {
                _data = data;
                _disposedTaskHandle = disposedTaskHandle;
            }
            #endregion

            #region Properties
            public bool IsMarkedAsBroken
            {
                get
                {
                    return _breakageMark != null;
                }
            }

            public T Object
            {
                get
                {
                    return _data.Object;
                }
            }

            public CancellationToken StoppingToken
            {
                get
                {
                    return _data.StoppingToken;
                }
            }
            #endregion

            #region Methods  
            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposedFlag, 1) != 0)
                {
                    return;
                }

                if (_breakageMark != null)
                {
                    _disposedTaskHandle.SetFaulted(_breakageMark.GetException());
                }
                else
                {
                    _disposedTaskHandle.SetSucceeded(default);
                }
            }

            public void MarkAsBroken(Exception? exception = null)
            {
                if (_breakageMark != null)
                {
                    throw new InvalidOperationException();
                }

                if (Interlocked.CompareExchange(ref _breakageMark, new BreakageMark(exception), null) != null)
                {
                    throw new InvalidOperationException();
                }
            }
            #endregion

            #region Nested Type
            private class BreakageMark
            {
                #region Fields
                private readonly ExceptionDispatchInfo? _exception;
                #endregion

                #region Constructors
                internal BreakageMark(Exception? exception)
                {
                    _exception = exception != null ? ExceptionDispatchInfo.Capture(exception) : null;
                }
                #endregion

                #region Methods
                internal ObjectBrokenException GetException()
                {
                    if (_exception != null)
                    {
                        try
                        {
                            _exception.Throw();
                            throw new ShouldNeverHappenException();
                        }
                        catch (Exception ex)
                        {
                            return new ObjectBrokenException(ex);
                        }
                    }
                    else
                    {
                        return new ObjectBrokenException();
                    }
                }
                #endregion
            }
            #endregion
        }

        private class LendingData
        {
            #region Constructors
            internal LendingData(T obj, CancellationToken stoppingToken)
            {
                Object = obj;
                StoppingToken = stoppingToken;
            }
            #endregion

            #region Properties
            internal T Object
            {
                get;
            }

            internal CancellationToken StoppingToken
            {
                get;
            }
            #endregion
        }
        #endregion
    }
}
