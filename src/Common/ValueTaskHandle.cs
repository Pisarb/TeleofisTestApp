using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swsu.StreetLights.Common
{
    public readonly struct ValueTaskHandle<TResult>
    {
        #region Constructors
        public ValueTaskHandle(MultiValueTaskSource<TResult> source, short token)
        {
            Source = source;
            Token = token;
        }
        #endregion

        #region Properties
        // TODO: Replace with some kind of ValueTaskFactory.
        public MultiValueTaskSource<TResult> Source
        {
            get;
        }

        public ValueTask<TResult> Task
        {
            get
            {
                return new ValueTask<TResult>(Source, Token);
            }
        }

        public short Token
        {
            get;
        }
        #endregion

        #region Methods
        public void SetCanceled(CancellationToken cancellationToken)
        {
            Source.SetCanceled(Token, cancellationToken);
        }

        public void SetFaulted(Exception exception)
        {
            Source.SetFaulted(Token, exception);
        }

        public void SetSucceeded(TResult result)
        {
            Source.SetSucceeded(Token, result);
        }

        public bool TrySetCanceled(CancellationToken cancellationToken)
        {
            return Source.TrySetCanceled(Token, cancellationToken);
        }

        public bool TrySetFaulted(Exception exception)
        {
            return Source.TrySetFaulted(Token, exception);
        }

        public bool TrySetSucceeded(TResult result)
        {
            return Source.TrySetSucceeded(Token, result);
        }
        #endregion
    }
}
