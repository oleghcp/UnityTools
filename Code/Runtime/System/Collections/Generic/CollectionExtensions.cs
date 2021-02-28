using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityUtility;
using UnityUtilityTools;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
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
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;
            Array.Sort(self, (itm1, itm2) => comparer.Compare(keySelector(itm1), keySelector(itm2)));
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> keySelector)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;
            CollectionUtility.QuickSort(self, 0, self.Count - 1, (itm1, itm2) => comparer.Compare(keySelector(itm1), keySelector(itm2)));
        }

        /// <summary>
        /// Returns an index of the first entrance of the specified element or -1 if the element is not found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] self, T item)
        {
            return (self as IList<T>).IndexOf(item);
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
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMin<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Min(self, selector, out _);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMax<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Max(self, selector, out _);
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector)
        {
            CollectionUtility.Min(self, selector, out TSource res);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector)
        {
            CollectionUtility.Max(self, selector, out TSource res);
            return res;
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

            if (self is IReadOnlyCollection<T> readOnlyColl)
                return readOnlyColl.Count == 0;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
        {
            return new HashSet<T>(self);
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
        /// Returns the element at the specified index from the end of a collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FromEnd<T>(this IList<T> self, int reverseIndex)
        {
            return self[self.Count - (reverseIndex + 1)];
        }

        /// <summary>
        /// Returns the element at the specified index from the end of a collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T FromEnd<T>(this T[] self, int reverseIndex)
        {
            return ref self[self.Length - (reverseIndex + 1)];
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetSubArray<T>(this IList<T> self, int startIndex)
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
        /// Enumerates collection from the specified index.
        /// </summary>
        public static IEnumerable<T> Enumerate<T>(this IList<T> self, int startIndex, int length)
        {
            if (startIndex < 0 || startIndex >= self.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + length > self.Count)
                throw new ArgumentOutOfRangeException(nameof(length));

            for (int i = startIndex; i < length; i++)
            {
                yield return self[i];
            }
        }

        /// <summary>
        /// Enumerates collection from the specified index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Enumerate<T>(this IList<T> self, int startIndex)
        {
            return Enumerate(self, startIndex, self.Count - startIndex);
        }

        /// <summary>
        /// Enumerates collection from the specified index from the end.
        /// </summary>
        public static IEnumerable<T> EnumerateBack<T>(this IList<T> self, int startReverseIndex, int length)
        {
            if (startReverseIndex < 0 || startReverseIndex >= self.Count)
                throw new ArgumentOutOfRangeException(nameof(startReverseIndex), "Index was out of range.");

            if (startReverseIndex + length > self.Count)
                throw new ArgumentOutOfRangeException(nameof(length));

            for (int i = startReverseIndex; i < length; i++)
            {
                yield return self.FromEnd(i);
            }
        }

        /// <summary>
        /// Enumerates collection from the specified index from the end.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> EnumerateBack<T>(this IList<T> self, int startReverseIndex)
        {
            return Enumerate(self, startReverseIndex, self.Count - startReverseIndex);
        }

        /// <summary>
        /// Returns a copy of the array.
        /// </summary>
        public static T[] GetCopy<T>(this T[] self)
        {
            T[] copy = new T[self.Length];
            self.CopyTo(copy, 0);
            return copy;
        }

        /// <summary>
        /// Copies all the elements of the current collection to the specified Span`1.
        /// </summary>
        public static void CopyTo<T>(this IList<T> self, Span<T> target) where T : unmanaged
        {
            int length = Math.Min(self.Count, target.Length);
            for (int i = 0; i < length; i++)
            {
                target[i] = self[i];
            }
        }

        /// <summary>
        /// Copies all the elements of the current collection to the specified Span`1.
        /// </summary>
        public static void CopyTo<T>(this IList<T> self, Span<T> target, int index) where T : unmanaged
        {
            if (index < 0 || index >= self.Count)
                throw Errors.IndexOutOfRange();

            int length = Math.Min(self.Count - index, target.Length);
            for (int i = 0; i < length; i++)
            {
                target[i] = self[i + index];
            }
        }

        /// <summary>
        /// Shuffles the elements of an entire collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IList<T> self, IRng generator)
        {
            CollectionUtility.Shuffle(self, generator);
        }

        /// <summary>
        /// Shuffles the elements of an entire collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IList<T> self)
        {
            CollectionUtility.Shuffle(self);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomItem<T>(this IList<T> self, IRng generator)
        {
            return CollectionUtility.GetRandomItem(self, generator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomItem<T>(this IList<T> self)
        {
            return CollectionUtility.GetRandomItem(self);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PullOutRandomItem<T>(this IList<T> self, IRng generator)
        {
            return CollectionUtility.PullOutRandomItem(self, generator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PullOutRandomItem<T>(this IList<T> self)
        {
            return CollectionUtility.PullOutRandomItem(self);
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
        /// Performs the specified action on each element of the specified array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this T[] self, Action<T> action)
        {
            Array.ForEach(self, action);
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
        /// Returns whether array contains the specified item.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this T[] self, T item)
        {
            return (self as IList).Contains(item);
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

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        {
            self.TryGetValue(key, out TValue value);
            return value;
        }

        public static (TKey key, TValue value) ToTuple<TKey, TValue>(in this KeyValuePair<TKey, TValue> self)
        {
            return (self.Key, self.Value);
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
    }
}
