using System;
using System.Collections.Generic;

namespace Swsu.StreetLights.Common
{
    public static class SynchronizationAction
    {
        #region Methods
        public static SynchronizationAction<TKey, TOldValue, TNewValue> Create<TKey, TOldValue, TNewValue>(SynchronizationActionKind kind, TKey key, TOldValue oldValue, TNewValue newValue)
        {
            return new SynchronizationAction<TKey, TOldValue, TNewValue>(kind, key, oldValue, newValue);
        }
        #endregion
    }

    public readonly struct SynchronizationAction<TKey, TOldValue, TNewValue> : IEquatable<SynchronizationAction<TKey, TOldValue, TNewValue>>
    {
        #region Constructors
        public SynchronizationAction(SynchronizationActionKind kind, TKey key, TOldValue oldValue, TNewValue newValue)
        {
            switch (kind)
            {
                case SynchronizationActionKind.Add:
                case SynchronizationActionKind.Remove:
                case SynchronizationActionKind.Replace:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }

            Kind = kind;
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
        #endregion

        #region Properties
        public TKey Key
        {
            get;
        }

        public SynchronizationActionKind Kind
        {
            get;
        }

        /// <summary>
        /// Равно <c>default</c>, если действие состоит в удалении элемента.
        /// </summary>
        public TNewValue NewValue
        {
            get;
        }

        /// <summary>
        /// Равно <c>default</c>, если действие состоит в добавлении элемента.
        /// </summary>
        public TOldValue OldValue
        {
            get;
        }
        #endregion

        #region Methods
        public void Deconstruct(out SynchronizationActionKind kind, out TKey key, out TOldValue oldValue, out TNewValue newValue)
        {
            kind = Kind;
            key = Key;
            oldValue = OldValue;
            newValue = NewValue;
        }

        public override bool Equals(object obj)
        {
            return obj is SynchronizationAction<TKey, TOldValue, TNewValue> other
                && Equals(other);
        }

        public bool Equals(
            object obj,
            IEqualityComparer<TKey> keyComparer,
            IEqualityComparer<TOldValue> oldValueComparer,
            IEqualityComparer<TNewValue> newValueComparer)
        {
            return obj is SynchronizationAction<TKey, TOldValue, TNewValue> other
                && Equals(other, keyComparer, oldValueComparer, newValueComparer);
        }

        public bool Equals(SynchronizationAction<TKey, TOldValue, TNewValue> other)
        {
            return Equals(other, EqualityComparer<TKey>.Default, EqualityComparer<TOldValue>.Default, EqualityComparer<TNewValue>.Default);
        }

        public bool Equals(
            SynchronizationAction<TKey, TOldValue, TNewValue> other,
            IEqualityComparer<TKey> keyComparer,
            IEqualityComparer<TOldValue> oldValueComparer,
            IEqualityComparer<TNewValue> newValueComparer)
        {
            return Kind.Equals(other.Kind)
                && keyComparer.Equals(Key, other.Key)
                && oldValueComparer.Equals(OldValue, other.OldValue)
                && newValueComparer.Equals(NewValue, other.NewValue);
        }

        public override int GetHashCode()
        {
            return GetHashCode(EqualityComparer<TKey>.Default, EqualityComparer<TOldValue>.Default, EqualityComparer<TNewValue>.Default);
        }

        public int GetHashCode(
            IEqualityComparer<TKey> keyComparer,
            IEqualityComparer<TOldValue> oldValueComparer,
            IEqualityComparer<TNewValue> newValueComparer)
        {
            return HashCode.Combine(
                Kind,
                keyComparer.GetHashCode(Key),
                oldValueComparer.GetHashCode(OldValue),
                newValueComparer.GetHashCode(NewValue));
        }

        public override string ToString()
        {
            return Kind switch
            {
                SynchronizationActionKind.NotInitialized => "NotInitialized",
                SynchronizationActionKind.Add => "Add(" + Key + ", " + NewValue + ")",
                SynchronizationActionKind.Remove => "Remove(" + Key + ", " + OldValue + ")",
                SynchronizationActionKind.Replace => "Replace(" + Key + ", " + OldValue + ", " + NewValue + ")",
                _ => throw new ShouldNeverHappenException(),
            };
        }
        #endregion
    }
}
