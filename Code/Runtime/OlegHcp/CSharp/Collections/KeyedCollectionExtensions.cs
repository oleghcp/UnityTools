using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OlegHcp.CSharp.Collections
{
    public static class KeyedCollectionExtensions
    {
        /// <summary>
        /// Adds an element to the dictionary and returns that element.
        /// </summary>
        public static TValue Place<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue newItem)
        {
            self.Add(key, newItem);
            return newItem;
        }

        /// <summary>
        /// Returns a read-only System.Collections.ObjectModel.ReadOnlyDictionary`2 wrapper for the current dictionary.
        /// </summary>        
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> self)
        {
            return new ReadOnlyDictionary<TKey, TValue>(self);
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IDictionary`2.
        /// </summary>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> self, Action<KeyValuePair<TKey, TValue>> action)
        {
            foreach (var item in self)
                action(item);
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IDictionary`2.
        /// </summary>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> self, Action<TKey, TValue> action)
        {
            foreach (var item in self)
                action(item.Key, item.Value);
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> self, in KeyValuePair<TKey, TValue> keyValuePair)
        {
            self.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> self, in (TKey key, TValue value) pair)
        {
            self.Add(pair.key, pair.value);
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        {
            self.TryGetValue(key, out TValue value);
            return value;
        }

        public static TValue GetOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key) where TValue : new()
        {
            if (!self.TryGetValue(key, out TValue value))
                self.Add(key, value = new TValue());
            return value;
        }

        public static TValue GetOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TKey, TValue> creator)
        {
            if (!self.TryGetValue(key, out TValue value))
                self.Add(key, value = creator(key));
            return value;
        }
    }
}
