using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Swsu.StreetLights.Common
{
    public static class EnumerableHelpers
    {
        #region Methods
        /// <summary>
        /// Внимание! Обе исходные последовательности должны быть упорядочены по возрастанию ключей.
        /// </summary>
        public static IEnumerable<SynchronizationAction<TKey, TOldValue, TNewValue>> Synchronize<TKey, TOldValue, TNewValue>(
            IEnumerable<KeyValuePair<TKey, TOldValue>> oldKeyValuePairs,
            IEnumerable<KeyValuePair<TKey, TNewValue>> newKeyValuePairs)
        {
            return Synchronize(oldKeyValuePairs, newKeyValuePairs, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Внимание! Обе исходные последовательности должны быть упорядочены по возрастанию ключей.
        /// </summary>
        public static IEnumerable<SynchronizationAction<TKey, TOldValue, TNewValue>> Synchronize<TKey, TOldValue, TNewValue>(
            IEnumerable<KeyValuePair<TKey, TOldValue>> oldKeyValuePairs,
            IEnumerable<KeyValuePair<TKey, TNewValue>> newKeyValuePairs,
            IComparer<TKey> keyComparer)
        {
            using var oldE = oldKeyValuePairs.GetEnumerator();
            using var newE = newKeyValuePairs.GetEnumerator();

            var haveOld = oldE.MoveNext();
            var haveNew = newE.MoveNext();

            switch ((haveOld, haveNew))
            {
                case (true, true):
                    goto Merge;

                case (true, false):
                    goto RemoveOld;

                case (false, true):
                    goto AddNew;

                case (false, false):
                    goto Done;
            }

        Merge:
            Debug.Assert(haveOld);
            Debug.Assert(haveNew);

            for (; ; )
            {
                var (oldKey, oldValue) = oldE.Current;
                var (newKey, newValue) = newE.Current;

                switch (Math.Sign(keyComparer.Compare(oldKey, newKey)))
                {
                    case 0:
                        yield return SynchronizationAction.Create(
                            SynchronizationActionKind.Replace,
                            Untee(oldKey, newKey),
                            oldValue,
                            newValue);

                        haveOld = oldE.MoveNext();
                        haveNew = newE.MoveNext();
                        break;

                    case -1: // oldKey < newKey
                        yield return SynchronizationAction.Create(
                            SynchronizationActionKind.Remove,
                            oldKey,
                            oldValue,
                            default(TNewValue)!);

                        haveOld = oldE.MoveNext();
                        break;

                    case +1: // newKey < oldKey
                        yield return SynchronizationAction.Create(
                            SynchronizationActionKind.Add,
                            newKey,
                            default(TOldValue)!,
                            newValue);

                        haveNew = newE.MoveNext();
                        break;

                    default:
                        throw new ShouldNeverHappenException();
                }

                switch ((haveOld, haveNew))
                {
                    case (true, true):
                        continue;

                    case (true, false):
                        goto RemoveOld;

                    case (false, true):
                        goto AddNew;

                    case (false, false):
                        goto Done;
                }
            }

        RemoveOld:
            Debug.Assert(haveOld);
            Debug.Assert(!haveNew);

            for (; ; )
            {
                var (oldKey, oldValue) = oldE.Current;

                yield return SynchronizationAction.Create(
                    SynchronizationActionKind.Remove,
                    oldKey,
                    oldValue,
                    default(TNewValue)!);

                haveOld = oldE.MoveNext();

                if (!haveOld)
                {
                    goto Done;
                }
            }

        AddNew:
            Debug.Assert(!haveOld);
            Debug.Assert(haveNew);

            for (; ; )
            {
                var (newKey, newValue) = newE.Current;

                yield return SynchronizationAction.Create(
                    SynchronizationActionKind.Add,
                    newKey,
                    default(TOldValue)!,
                    newValue);

                haveNew = newE.MoveNext();

                if (!haveNew)
                {
                    goto Done;
                }
            }

        Done:
            Debug.Assert(!haveOld);
            Debug.Assert(!haveNew);

        }

        private static T Untee<T>(T value1, T value2)
        {
            return Untee(value1, value2, EqualityComparer<T>.Default);
        }

        private static T Untee<T>(T value1, T value2, IEqualityComparer<T> comparer)
        {
            if (comparer.Equals(value1, value2))
            {
                return value1;
            }
            else
            {
                throw new ArgumentException("Not equals.", nameof(value2));
            }
        }
        #endregion
    }
}
