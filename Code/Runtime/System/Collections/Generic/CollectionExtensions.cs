using UU;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Shuffles the elements of an entire collection.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int last = list.Count;

            while (last > 1)
            {
                int cur = Rnd.Random(last--);

                T value = list[cur];
                list[cur] = list[last];
                list[last] = value;
            }
        }

        /// <summary>
        /// Returns an index of the first entrance of the specified element or -1 if the element is not found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] array, T item)
        {
            return Array.IndexOf(array, item);
        }

        /// <summary>
        /// Returns an index of the first entrance of an element that matches the specified condition or -1 if the element is not found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] array, Predicate<T> condition)
        {
            return Array.FindIndex(array, condition);
        }

        /// <summary>
        /// Returns an index of the first entrance of an element that matches the specified condition or -1 if the element is not found.
        /// </summary>
        public static int IndexOf<T>(this IList<T> list, Predicate<T> condition)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (condition(list[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the element at the last index.
        /// </summary>
        public static T GetLast<T>(this IList<T> collection)
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            return collection[collection.Count - 1];
        }

        /// <summary>
        /// Returns the element at the last index.
        /// </summary>
        public static ref T GetLast<T>(this T[] array)
        {
            if (array.Length == 0)
                throw new InvalidOperationException("Collection is empty.");

            return ref array[array.Length - 1];
        }

        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        public static int IndexOfMin<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            if (typeof(TKey).IsValueType)
                return CollectionHelper.ValueMin(collection, selector, out _);
            else
                return CollectionHelper.RefMin(collection, selector, out _);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        public static int IndexOfMax<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            if (typeof(TKey).IsValueType)
                return CollectionHelper.ValueMax(collection, selector, out _);
            else
                return CollectionHelper.RefMax(collection, selector, out _);
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            TSource res;

            if (typeof(TKey).IsValueType)
                CollectionHelper.ValueMin(collection, selector, out res);
            else
                CollectionHelper.RefMin(collection, selector, out res);

            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            TSource res;

            if (typeof(TKey).IsValueType)
                CollectionHelper.ValueMax(collection, selector, out res);
            else
                CollectionHelper.RefMax(collection, selector, out res);

            return res;
        }

        /// <summary>
        /// Returns a subarray of the specified length starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this List<T> list, int startIndex, int length)
        {
            T[] subArray = new T[length];
            for (int i = 0; i < length; i++) { subArray[i] = list[i + startIndex]; }
            return subArray;
        }

        /// <summary>
        /// Returns a subarray starting from the specified index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetSubArray<T>(this List<T> list, int startIndex)
        {
            return GetSubArray(list, startIndex, list.Count - startIndex);
        }

        /// <summary>
        /// Returns a subarray of the specified length starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this T[] array, int startIndex, int length)
        {
            T[] subArray = new T[length];
            Array.Copy(array, startIndex, subArray, 0, length);
            return subArray;
        }

        /// <summary>
        /// Returns a subarray starting from the specified index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetSubArray<T>(this T[] array, int startIndex)
        {
            return GetSubArray(array, startIndex, array.Length - startIndex);
        }

        /// <summary>
        /// Returns a copy of the array;
        /// </summary>
        public static T[] GetCopy<T>(this T[] array)
        {
            T[] copy = new T[array.Length];
            Array.Copy(array, copy, array.Length);
            return copy;
        }

        /// <summary>
        /// Sorts the elements in an entire System.Array using the System.IComparable`1 generic interface implementation of each element of the System.Array.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this T[] array) where T : IComparable<T>
        {
            Array.Sort(array);
        }

        /// <summary>
        /// Sorts by comparison.
        /// </summary>
        /// <param name="comparer">Reference to comparing function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this T[] array, Comparison<T> comparer)
        {
            Array.Sort(array, comparer);
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this TSource[] array, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            if (typeof(TKey).IsValueType)
                Array.Sort(array, (itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
            else
                Array.Sort(array, (itm1, itm2) => CollectionHelper.Compare(keySelector(itm1), keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this List<TSource> list, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            if (typeof(TKey).IsValueType)
                list.Sort((itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
            else
                list.Sort((itm1, itm2) => CollectionHelper.Compare(keySelector(itm1), keySelector(itm2)));
        }

        /// <summary>
        /// Finds specified element.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>(this T[] array, T item)
        {
            return Array.Find(array, itm => itm.Equals(item));
        }

        /// <summary>
        /// Finds an element by condition.
        /// </summary>        
        /// <param name="match">Reference to matching function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>(this T[] array, Predicate<T> match)
        {
            return Array.Find(array, match);
        }

        /// <summary>
        /// Reverses the sequence of the elements.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this T[] array)
        {
            Array.Reverse(array);
        }

        /// <summary>
        /// Reverses the sequence of the elements.
        /// </summary>
        /// <param name="index">The starting index of the section to reverse.</param>
        /// <param name="length">The number of elements in the section to reverse.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this T[] array, int index, int length)
        {
            Array.Reverse(array, index, length);
        }

        /// <summary>
        /// Sets elements of the array to their default value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] array)
        {
            Array.Clear(array, 0, array.Length);
        }

        /// <summary>
        /// Sets elements of the array to their default value.
        /// </summary>
        /// <param name="index">The starting index of the range of elements to clear.</param>
		/// <param name="length">The number of elements to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] array, int index, int length)
        {
            Array.Clear(array, index, length);
        }

        /// <summary>
        /// Returns new array contained elements got by converting the array.
        /// </summary>
        /// <param name="converter">Conveting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOut[] GetConverted<TIn, TOut>(this TIn[] array, Converter<TIn, TOut> converter)
        {
            return Array.ConvertAll(array, converter);
        }

        /// <summary>
        /// Returns whether the list contains the specified item.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this IList<T> array, T item)
        {
            return array.Contains(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the lsit and returns that element.
        /// </summary>
        public static T PullOut<T>(this IList<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Removes the last element of the lsit and returns that element.
        /// </summary>
        public static T Pop<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            int index = list.Count - 1;
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Adds an element to the collection and returns that element.
        /// </summary>
        public static T AddNGet<T>(this ICollection<T> list, T newItem)
        {
            list.Add(newItem);
            return newItem;
        }

        /// <summary>
        /// Inserts an element into the list at the specified index and returns that element.
        /// </summary>
        public static T InsertNGet<T>(this IList<T> list, int index, T newItem)
        {
            list.Insert(index, newItem);
            return newItem;
        }

        /// <summary>
        /// Returns a copy of the list;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> GetCopy<T>(this ICollection<T> list)
        {
            return new List<T>(list);
        }

        /// <summary>
        /// Adds an element to the dictionary and returns that element.
        /// </summary>
        public static TValue AddNGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue newItem)
        {
            dict.Add(key, newItem);
            return newItem;
        }

        /// <summary>
        /// Removes the element with the specified key from the dictionary and returns it or default value.
        /// </summary>
        public static TValue PullOut<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.TryGetValue(key, out TValue value))
                dict.Remove(key);
            return value;
        }

        /// <summary>
        /// Returns a read-only System.Collections.ObjectModel.ReadOnlyDictionary`2 wrapper for the current dictionary.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dict);
        }
    }
}
