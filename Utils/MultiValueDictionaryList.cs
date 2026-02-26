using System.Collections.Generic;

namespace UniSignal.Utils
{
    /// <summary>
    /// Dictionary that stores multiple values per key using a List.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    internal class MultiValueDictionaryList<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        /// <summary>
        /// Adds the specified value under the specified key
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (!TryGetValue(key, out var container))
            {
                container = new List<TValue>(4);
                base.Add(key, container);
            }

            container.Add(value);
        }

        /// <summary>
        /// Adds specified key with no values
        /// </summary>
        public void Add(TKey key)
        {
            if (!ContainsKey(key))
            {
                base.Add(key, new List<TValue>(4));
            }
        }

        /// <summary>
        /// Removes the specified value for the specified key.
        /// Returns true if the key was removed (no more values left).
        /// </summary>
        public bool Remove(TKey key, TValue value)
        {
            if (!TryGetValue(key, out var container))
                return false;

            container.Remove(value);
            if (container.Count == 0)
            {
                Remove(key);
                return true;
            }

            return false;
        }
    }
}
