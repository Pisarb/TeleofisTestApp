using System.Collections.Generic;

namespace Swsu.StreetLights.Common
{
    public static class DictionaryHelpers
    {
        #region Methods
        public static bool DictionaryEquals<TKey, TValue>(
            IReadOnlyDictionary<TKey, TValue> dictionaryA,
            IReadOnlyDictionary<TKey, TValue> dictionaryB)
        {
            return DictionaryEquals(dictionaryA, dictionaryB, EqualityComparer<TValue>.Default);
        }

        public static bool DictionaryEquals<TKey, TValue>(
            IReadOnlyDictionary<TKey, TValue> dictionaryA,
            IReadOnlyDictionary<TKey, TValue> dictionaryB,
            IEqualityComparer<TValue> valueComparer)
        {
            foreach (var (key, valueA) in dictionaryA)
            {
                if (!dictionaryB.TryGetValue(key, out var valueB) || !valueComparer.Equals(valueA, valueB))
                {
                    return false;
                }
            }

            foreach (var (key, valueB) in dictionaryB)
            {
                if (!dictionaryA.TryGetValue(key, out var valueA) || !valueComparer.Equals(valueA, valueB))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
