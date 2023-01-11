using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityUtility.CSharp.Collections.Iterators;
using UnityUtility.Tools;

namespace UnityUtility.CSharp.Collections
{
    public static class CollectionExtensions
    {
        public static void DisplaceLeft<T>(this IList<T> self)
        {
            if (self.Count <= 1)
                return;

            T tmp = self[0];

            for (int i = 1; i < self.Count; i++)
            {
                self[i - 1] = self[i];
            }

            self[self.Count - 1] = tmp;
        }

        public static void DisplaceRight<T>(this IList<T> self)
        {
            if (self.Count <= 1)
                return;

            T tmp = self.FromEnd(0);

            for (int i = self.Count - 2; i >= 0; i--)
            {
                self[i + 1] = self[i];
            }

            self[0] = tmp;
        }

        public static void Swap<T>(this IList<T> self, int i, int j)
        {
            Helper.Swap(self, i, j);
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this List<TSource> self, Func<TSource, TKey> keySelector)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;
            self.Sort((itm1, itm2) => comparer.Compare(keySelector(itm1), keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> keySelector)
        {
            if (self.Count == 0)
                return;

            Comparer<TKey> comparer = Comparer<TKey>.Default;
            CollectionUtility.QuickSort(self, 0, self.Count - 1, (itm1, itm2) => comparer.Compare(keySelector(itm1), keySelector(itm2)));
        }

        /// <summary>
        /// Returns an index of the first entrance of an element that matches the specified condition or -1 if the element is not found.
        /// </summary>
        public static int IndexOf<T>(this IList<T> self, Predicate<T> condition)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        public static int IndexOfMin<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Min(self, selector, out _, out _);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        public static int IndexOfMax<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Max(self, selector, out _, out _);
        }

        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        public static int IndexOfMin<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector, out TKey min)
        {
            return CollectionUtility.Min(self, selector, out _, out min);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        public static int IndexOfMax<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector, out TKey max)
        {
            return CollectionUtility.Max(self, selector, out _, out max);
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector)
        {
            CollectionUtility.Min(self, selector, out TSource res, out _);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector)
        {
            CollectionUtility.Max(self, selector, out TSource res, out _);
            return res;
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector, out TKey min)
        {
            CollectionUtility.Min(self, selector, out TSource res, out min);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector, out TKey max)
        {
            CollectionUtility.Max(self, selector, out TSource res, out max);
            return res;
        }

        /// <summary>
        /// Indicates whether the specified collection is null or it's length equals zero.
        /// </summary>        
        public static bool IsNullOrEmpty<T>(this ICollection<T> self)
        {
            return self == null || self.Count == 0;
        }

        /// <summary>
        /// Indicates whether the specified collection is not null and contains at least one element.
        /// </summary>
        public static bool HasAnyData<T>(this ICollection<T> self)
        {
            return self != null && self.Count > 0;
        }

        /// <summary>
        /// Indicates whether the specified collection is null or it's length equals zero.
        /// </summary>    
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> self)
        {
            if (self == null)
                return true;

            if (self is ICollection<T> collection)
                return collection.Count == 0;

            if (self is IReadOnlyCollection<T> readOnlyColl)
                return readOnlyColl.Count == 0;

            return !self.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Indicates whether the specified collection is not null and contains at least one element.
        /// </summary>
        public static bool HasAnyData<T>(this IEnumerable<T> self)
        {
            return !self.IsNullOrEmpty();
        }

#if !UNITY_2021_2_OR_NEWER
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
        {
            return new HashSet<T>(self);
        }
#endif

        /// <summary>
        /// Returns the element at the specified index from the end of a collection.
        /// </summary>
        public static T FromEnd<T>(this IList<T> self, int reverseIndex)
        {
            return self[self.Count - (reverseIndex + 1)];
        }

        /// <summary>
        /// Returns a subarray of the specified length starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this IList<T> self, int startIndex, int length)
        {
            T[] subArray = new T[length];
            for (int i = 0; i < length; i++) { subArray[i] = self[i + startIndex]; }
            return subArray;
        }

        /// <summary>
        /// Returns a subarray starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this IList<T> self, int startIndex)
        {
            return GetSubArray(self, startIndex, self.Count - startIndex);
        }

        /// <summary>
        /// Enumerates collection from the specified index.
        /// </summary>
        public static EnumerableQuery<T> Enumerate<T>(this IList<T> self, int startIndex, int length)
        {
            return new EnumerableQuery<T>(self, startIndex, length);
        }

        /// <summary>
        /// Enumerates collection from the specified index.
        /// </summary>
        public static EnumerableQuery<T> Enumerate<T>(this IList<T> self, int startIndex)
        {
            return new EnumerableQuery<T>(self, startIndex);
        }

        /// <summary>
        /// Copies all the elements of the current collection to the specified Span`1.
        /// </summary>
        public static void CopyTo<T>(this IList<T> self, Span<T> target) where T : unmanaged
        {
            int count = Math.Min(self.Count, target.Length);
            for (int i = 0; i < count; i++)
            {
                target[i] = self[i];
            }
        }

        /// <summary>
        /// Copies all the elements of the current collection to the specified Span`1.
        /// </summary>
        /// <param name="index">The starting index of the target span.</param>
        public static void CopyTo<T>(this IList<T> self, Span<T> target, int index) where T : unmanaged
        {
            if ((uint)index >= (uint)target.Length)
                throw Errors.IndexOutOfRange();

            int count = Math.Min(self.Count, target.Length - index);
            for (int i = 0; i < count; i++)
            {
                target[i + index] = self[i];
            }
        }

        /// <summary>
        /// Shuffles the elements of an entire collection.
        /// </summary>
        public static void Shuffle<T>(this IList<T> self, IRng generator)
        {
            int last = self.Count;

            while (last > 1)
            {
                int cur = generator.Next(last--);
                self.Swap(cur, last);
            }
        }

        public static T GetRandomItem<T>(this IList<T> self, IRng generator)
        {
            if (self.Count == 0)
                throw Errors.NoElements();

            return self[generator.Next(self.Count)];
        }

        public static T GetRandomItem<T>(this ICollection<T> self, IRng generator)
        {
            int index = generator.Next(self.Count);
            int count = 0;
            foreach (T item in self)
            {
                if (index == count)
                    return item;

                count++;
            }

            throw Errors.NoElements();
        }

        public static T PullOutRandomItem<T>(this IList<T> self, IRng generator)
        {
            if (self.Count == 0)
                throw Errors.NoElements();

            return self.PullOut(generator.Next(self.Count));
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IList`1.
        /// </summary>
        public static void ForEach<T>(this IList<T> self, Action<T> action)
        {
            for (int i = 0; i < self.Count; i++)
                action(self[i]);
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable`1.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
                action(item);
        }

        /// <summary>
        /// Returns whether array contains the specified item.
        /// </summary>
        public static bool Contains<T>(this IList<T> self, Predicate<T> condition)
        {
            return self.IndexOf(condition) >= 0;
        }

        /// <summary>
        /// Removes the element at the specified index of the lsit and returns that element.
        /// </summary>
        public static T PullOut<T>(this IList<T> self, int index)
        {
            T item = self[index];
            self.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Removes the last element of the lsit and returns that element.
        /// </summary>
        public static T Pop<T>(this IList<T> self)
        {
            if (self.Count == 0)
                throw Errors.NoElements();

            return self.PullOut(self.Count - 1);
        }

        /// <summary>
        /// Adds an element to the collection and returns that element.
        /// </summary>
        public static T Place<T>(this ICollection<T> self, T newItem)
        {
            self.Add(newItem);
            return newItem;
        }

        /// <summary>
        /// Inserts an element into the list at the specified index and returns that element.
        /// </summary>
        public static T Push<T>(this IList<T> self, int index, T newItem)
        {
            self.Insert(index, newItem);
            return newItem;
        }

        /// <summary>
        /// Inserts an element into the list at the specified index. Expands the list if the index is out of bounds.
        /// </summary>
        public static void ForceInsert<T>(this IList<T> self, int index, T newItem)
        {
            while (index > self.Count) { self.Add(default); }
            self.Insert(index, newItem);
        }

        /// <summary>
        /// Inserts an element into the list at the specified index and returns that element. Expands the list if the index is out of bounds.
        /// </summary>
        public static T ForcePush<T>(this IList<T> self, int index, T newItem)
        {
            self.ForceInsert(index, newItem);
            return newItem;
        }

        /// <summary>
        /// Adds an element to the dictionary and returns that element.
        /// </summary>
        public static TValue Place<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue newItem)
        {
            self.Add(key, newItem);
            return newItem;
        }

#if !UNITY_2021_2_OR_NEWER
        /// <summary>
        /// Removes the element with the specified key from the dictionary and returns it or default value.
        /// </summary>
        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, out TValue value)
        {
            if (self.TryGetValue(key, out value))
            {
                self.Remove(key);
                return true;
            }

            return false;
        }
#endif

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

#if !UNITY_2021_2_OR_NEWER
        public static void Deconstruct<TKey, TValue>(this in KeyValuePair<TKey, TValue> self, out TKey key, out TValue value)
        {
            key = self.Key;
            value = self.Value;
        }

        public static bool TryPeek<T>(this Stack<T> self, out T item)
        {
            if (self.Count > 0)
            {
                item = self.Peek();
                return true;
            }

            item = default;
            return false;
        }

        public static bool TryPop<T>(this Stack<T> self, out T item)
        {
            if (self.Count > 0)
            {
                item = self.Pop();
                return true;
            }

            item = default;
            return false;
        }

        public static bool TryPeek<T>(this Queue<T> self, out T item)
        {
            if (self.Count > 0)
            {
                item = self.Peek();
                return true;
            }

            item = default;
            return false;
        }

        public static bool TryDequeue<T>(this Queue<T> self, out T item)
        {
            if (self.Count > 0)
            {
                item = self.Dequeue();
                return true;
            }

            item = default;
            return false;
        }
#endif
    }
}
