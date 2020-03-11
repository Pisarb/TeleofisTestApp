using System;
using System.Threading;

namespace Swsu.StreetLights.Common.Lending
{
    public static class ObjectLendingExtensions
    {
        #region Methods
        public static IObjectLending<TResult> Wrap<TSource, TResult>(this IObjectLending<TSource> source, Func<TSource, TResult> wrapper, bool doNotDisposeSource = false)
        {
            return new WrapImpl<TSource, TResult>(source, wrapper, doNotDisposeSource);
        }
        #endregion

        #region Nested Types
        private class WrapImpl<TSource, TResult> : IObjectLending<TResult>
        {
            #region Fields
            private readonly DisposableArg<IObjectLending<TSource>> _source;
            #endregion

            #region Constructors
            internal WrapImpl(IObjectLending<TSource> source, Func<TSource, TResult> wrapper, bool doNotDisposeSource) :
                this(DisposableArg.Create(source, doNotDisposeSource), wrapper)
            {
            }

            internal WrapImpl(DisposableArg<IObjectLending<TSource>> source, Func<TSource, TResult> wrapper)
            {
                _source = source;
                Object = wrapper(source.Value.Object);
            }
            #endregion

            #region Properties
            public TResult Object
            {
                get;
            }

            public bool IsMarkedAsBroken
            {
                get
                {
                    return _source.Value.IsMarkedAsBroken;
                }
            }

            public CancellationToken StoppingToken
            {
                get
                {
                    return _source.Value.StoppingToken;
                }
            }
            #endregion

            #region Methods
            public void Dispose()
            {
                DisposableHelpers.Dispose(Object);
                _source.Dispose();
            }

            public void MarkAsBroken(Exception? exception = null)
            {
                _source.Value.MarkAsBroken(exception);
            }
            #endregion
        }
        #endregion
    }
}
