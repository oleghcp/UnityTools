using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Tools;
using UnityUtility;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns an index of the first entrance of the specified element or -1 if the element is not found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] self, T item)
        {
            return Array.IndexOf(self, item);
        }

        /// <summary>
        /// Returns an index of the first entrance of an element that matches the specified condition or -1 if the element is not found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] self, Predicate<T> condition)
        {
            return Array.FindIndex(self, condition);
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
        /// Returns the element at the last index.
        /// </summary>
        public static T GetLast<T>(this IList<T> self)
        {
            if (self.Count == 0)
                throw Errors.NoElements();

            return self[self.Count - 1];
        }

        /// <summary>
        /// Returns the element at the last index.
        /// </summary>
        public static ref T GetLast<T>(this T[] self)
        {
            if (self.Length == 0)
                throw Errors.NoElements();

            return ref self[self.Length - 1];
        }

        /// <summary>
        /// Indicates whether the specified collection is null or it's length equals zero.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this ICollection<T> self)
        {
            return self == null || self.Count == 0;
        }

        /// <summary>
        /// Indicates whether the specified collection is not null and contains at least one element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            return !self.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Indicates whether the specified collection is not null and contains at least one element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyData<T>(this IEnumerable<T> self)
        {
            return !self.IsNullOrEmpty();
        }

        /// <summary>
        /// Converts the elements in the IEnumerable'1 collection to strings, inserts the specified separator between the elements, concatenates them, and returns the resulting string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatToString<T>(this IEnumerable<T> self, string separator = "")
        {
            return string.Join(separator, self);
        }

        /// <summary>
        /// Converts the elements in the IEnumerable'1 collection to strings, inserts the specified separator between the elements, concatenates them, and returns the resulting string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatToString(this IEnumerable<string> self, string separator = "")
        {
            return string.Join(separator, self);
        }

        /// <summary>
        /// Converts the elements in the IEnumerable'1 collection to strings, inserts the specified separator between the elements, concatenates them, and returns the resulting string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatToString<T>(this IEnumerable<T> self, string separator, int startIndex, int count)
        {
            return string.Join(separator, self, startIndex, count);
        }

        /// <summary>
        /// Converts the elements in the IEnumerable'1 collection to strings, inserts the specified separator between the elements, concatenates them, and returns the resulting string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatToString(this IEnumerable<string> self, string separator, int startIndex, int count)
        {
            return string.Join(separator, self, startIndex, count);
        }

        /// <summary>
        /// Returns a subarray of the specified length starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this List<T> self, int startIndex, int length)
        {
            T[] subArray = new T[length];
            for (int i = 0; i < length; i++) { subArray[i] = self[i + startIndex]; }
            return subArray;
        }

        /// <summary>
        /// Returns a subarray starting from the specified index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetSubArray<T>(this List<T> self, int startIndex)
        {
            return GetSubArray(self, startIndex, self.Count - startIndex);
        }

        /// <summary>
        /// Returns a subarray of the specified length starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this T[] self, int startIndex, int length)
        {
            T[] subArray = new T[length];
            Array.Copy(self, startIndex, subArray, 0, length);
            return subArray;
        }

        /// <summary>
        /// Returns a subarray starting from the specified index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetSubArray<T>(this T[] self, int startIndex)
        {
            return GetSubArray(self, startIndex, self.Length - startIndex);
        }

        /// <summary>
        /// Returns a copy of the array;
        /// </summary>
        public static T[] GetCopy<T>(this T[] self)
        {
            T[] copy = new T[self.Length];
            Array.Copy(self, copy, self.Length);
            return copy;
        }

        /// <summary>
        /// Sorts the elements in an entire System.Array using the System.IComparable`1 generic interface implementation of each element of the System.Array.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this T[] self) where T : IComparable<T>
        {
            Array.Sort(self);
        }

        /// <summary>
        /// Sorts by comparison.
        /// </summary>
        /// <param name="comparer">Reference to comparing function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this T[] self, Comparison<T> comparer)
        {
            Array.Sort(self, comparer);
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

                T value = self[cur];
                self[cur] = self[last];
                self[last] = value;
            }
        }

        /// <summary>
        /// Finds specified element.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>(this T[] self, T item)
        {
            return Array.Find(self, itm => itm.Equals(item));
        }

        /// <summary>
        /// Finds an element by condition.
        /// </summary>        
        /// <param name="match">Reference to matching function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>(this T[] self, Predicate<T> match)
        {
            return Array.Find(self, match);
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
        /// Reverses the sequence of the elements.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this T[] self)
        {
            Array.Reverse(self);
        }

        /// <summary>
        /// Reverses the sequence of the elements.
        /// </summary>
        /// <param name="index">The starting index of the section to reverse.</param>
        /// <param name="length">The number of elements in the section to reverse.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this T[] self, int index, int length)
        {
            Array.Reverse(self, index, length);
        }

        /// <summary>
        /// Sets elements of the array to their default value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] self)
        {
            Array.Clear(self, 0, self.Length);
        }

        /// <summary>
        /// Sets elements of the array to their default value.
        /// </summary>
        /// <param name="index">The starting index of the range of elements to clear.</param>
		/// <param name="length">The number of elements to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] self, int index, int length)
        {
            Array.Clear(self, index, length);
        }

        /// <summary>
        /// Returns new array contained elements got by converting the array.
        /// </summary>
        /// <param name="converter">Conveting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOut[] GetConverted<TIn, TOut>(this TIn[] self, Converter<TIn, TOut> converter)
        {
            return Array.ConvertAll(self, converter);
        }

        /// <summary>
        /// Returns whether the list contains the specified item.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this IList<T> self, T item)
        {
            return self.Contains(item);
        }

        public static bool Remove<T>(this List<T> self, Predicate<T> match)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (match(self[i]))
                {
                    self.RemoveAt(i);
                    return true;
                }
            }

            return false;
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

            int index = self.Count - 1;
            T item = self[index];
            self.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Adds an element to the collection and returns that element.
        /// </summary>
        public static T AddAndGet<T>(this ICollection<T> self, T newItem)
        {
            self.Add(newItem);
            return newItem;
        }

        /// <summary>
        /// Inserts an element into the list at the specified index and returns that element.
        /// </summary>
        public static T InsertAndGet<T>(this IList<T> self, int index, T newItem)
        {
            self.Insert(index, newItem);
            return newItem;
        }

        /// <summary>
        /// Returns a copy of the list;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> GetCopy<T>(this ICollection<T> self)
        {
            return new List<T>(self);
        }

        /// <summary>
        /// Adds an element to the dictionary and returns that element.
        /// </summary>
        public static TValue AddAndGet<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue newItem)
        {
            self.Add(key, newItem);
            return newItem;
        }

        /// <summary>
        /// Removes the element with the specified key from the dictionary and returns it or default value.
        /// </summary>
        public static TValue PullOut<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        {
            if (self.TryGetValue(key, out TValue value))
                self.Remove(key);
            return value;
        }

        /// <summary>
        /// Returns a read-only System.Collections.ObjectModel.ReadOnlyDictionary`2 wrapper for the current dictionary.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public static (TKey key, TValue value) ToTuple<TKey, TValue>(in this KeyValuePair<TKey, TValue> self)
        {
            return (self.Key, self.Value);
        }
    }
}
