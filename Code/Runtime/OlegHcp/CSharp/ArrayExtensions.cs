using System;
using System.Collections.Generic;
using OlegHcp.Tools;

namespace OlegHcp.CSharp
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Assigns the given value of type T to the elements of the specified array.
        /// </summary>
        public static void Fill<T>(this T[] self, T value)
        {
#if UNITY_2021_2_OR_NEWER
            Array.Fill(self, value);
#else
            for (int i = 0; i < self.Length; i++)
            {
                self[i] = value;
            }
#endif
        }

        /// <summary>
        /// Assigns the given value of type T to the elements of the specified array which are within the range of startIndex (inclusive) and the next count number of indices.
        /// </summary>
        public static void Fill<T>(this T[] self, T value, int startIndex, int count)
        {
#if UNITY_2021_2_OR_NEWER
            Array.Fill(self, value, startIndex, count);
#else
            if ((uint)startIndex >= (uint)self.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + count > self.Length)
                throw ThrowErrors.SegmentOutOfRange();

            for (int i = startIndex; i < count + startIndex; i++)
            {
                self[i] = value;
            }
#endif
        }

        /// <summary>
        /// Assigns the given value of type T to the elements of the specified array.
        /// </summary>
        public static void Fill<T>(this T[] self, Func<int, T> factory)
        {
            for (int i = 0; i < self.Length; i++)
            {
                self[i] = factory(i);
            }
        }

        /// <summary>
        /// Assigns the given value of type T to the elements of the specified array which are within the range of startIndex (inclusive) and the next count number of indices.
        /// </summary>
        public static void Fill<T>(this T[] self, Func<int, T> factory, int startIndex, int count)
        {
            if ((uint)startIndex >= (uint)self.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + count > self.Length)
                throw ThrowErrors.SegmentOutOfRange();

            for (int i = startIndex; i < count + startIndex; i++)
            {
                self[i] = factory(i);
            }
        }

        public static T[] ToArray<T>(this Array self)
        {
            return (T[])self;
        }

        public static void Swap<T>(this T[] self, int i, int j)
        {
            (self[i], self[j]) = (self[j], self[i]);
        }

        /// <summary>
        /// Sorts the elements in an entire System.Array using the System.IComparable`1 generic interface implementation of each element of the System.Array.
        /// </summary>
        public static void Sort<T>(this T[] self) where T : IComparable<T>
        {
            Array.Sort(self);
        }

        /// <summary>
        /// Sorts using the specified comparer.
        /// </summary>
        public static void Sort<T>(this T[] self, IComparer<T> comparer)
        {
            Array.Sort(self, comparer);
        }

        /// <summary>
        /// Sorts by comparison.
        /// </summary>
        /// <param name="comparison">Reference to comparing function.</param>
        public static void Sort<T>(this T[] self, Comparison<T> comparison)
        {
            Array.Sort(self, comparison);
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector)
        {
            var keyComparer = new SortUtility.KeyComparerA<TSource, TKey>
            {
                KeySelector = keySelector,
                Comparer = Comparer<TKey>.Default,
            };

            SortUtility.Sort(self, 0, self.Length - 1, keyComparer);
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector, Comparison<TKey> comparison)
        {
            var keyComparer = new SortUtility.KeyComparerB<TSource, TKey>
            {
                KeySelector = keySelector,
                Comparison = comparison,
            };

            SortUtility.Sort(self, 0, self.Length - 1, keyComparer);
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void Sort<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            var keyComparer = new SortUtility.KeyComparerA<TSource, TKey>
            {
                KeySelector = keySelector,
                Comparer = comparer,
            };

            SortUtility.Sort(self, 0, self.Length - 1, keyComparer);
        }

        public static void SortDescending<T>(this T[] self)
        {
            SortUtility.Sort(self, 0, self.Length - 1, new SortUtility.DescendingComparer<T> { Comparer = Comparer<T>.Default });
        }

        /// <summary>
        /// Sorts by selected key in descending order.
        /// </summary>
        /// <param name="keySelector">Reference to selecting function.</param>
        public static void SortDescending<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector)
        {
            var keyComparer = new SortUtility.DescendingKeyComparer<TSource, TKey>
            {
                KeySelector = keySelector,
                Comparer = Comparer<TKey>.Default,
            };

            SortUtility.Sort(self, 0, self.Length - 1, keyComparer);
        }

        /// <summary>
        /// Returns an index of the first entrance of the specified element or -1 if the element is not found.
        /// </summary>
        public static int IndexOf<T>(this T[] self, T item)
        {
            return (self as IList<T>).IndexOf(item);
        }

        /// <summary>
        /// Returns an index of the first entrance of an element that matches the specified condition or -1 if the element is not found.
        /// </summary>
        public static int IndexOf<T>(this T[] self, Predicate<T> condition)
        {
            return Array.FindIndex(self, condition);
        }

        public static int LastIndexOf<T>(this T[] self, Predicate<T> condition)
        {
            return Array.FindLastIndex(self, condition);
        }

        /// <summary>
        /// Returns the element at the specified index from the end of a collection.
        /// </summary>
        public static ref T FromEnd<T>(this T[] self, int index)
        {
            return ref self[self.Length - (index + 1)];
        }

        /// <summary>
        /// Returns a subarray of the specified length starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this T[] self, int startIndex, int length)
        {
            if ((uint)startIndex >= (uint)self.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (length == 0)
                return Array.Empty<T>();

            T[] subArray = new T[length];
            Array.Copy(self, startIndex, subArray, 0, length);
            return subArray;
        }

        /// <summary>
        /// Returns a subarray starting from the specified index.
        /// </summary>
        public static T[] GetSubArray<T>(this T[] self, int startIndex)
        {
            return GetSubArray(self, startIndex, self.Length - startIndex);
        }

        /// <summary>
        /// Returns a copy of the array.
        /// </summary>
        public static T[] GetCopy<T>(this T[] self)
        {
            if (self.Length == 0)
                return self;

            T[] copy = new T[self.Length];
            self.CopyTo(copy, 0);
            return copy;
        }

        /// <summary>
        /// Finds an element by condition.
        /// </summary>        
        /// <param name="match">Reference to matching function.</param>
        public static T Find<T>(this T[] self, Predicate<T> match)
        {
            return Array.Find(self, match);
        }

        public static T FindLast<T>(this T[] self, Predicate<T> match)
        {
            return Array.FindLast(self, match);
        }

        /// <summary>
        /// Performs the specified action on each element of the specified array.
        /// </summary>
        public static void ForEach<T>(this T[] self, Action<T> action)
        {
            Array.ForEach(self, action);
        }

        /// <summary>
        /// Reverses the sequence of the elements.
        /// </summary>
        public static void Reverse<T>(this T[] self)
        {
            Array.Reverse(self);
        }

        /// <summary>
        /// Reverses the sequence of the elements.
        /// </summary>
        /// <param name="index">The starting index of the section to reverse.</param>
        /// <param name="length">The number of elements in the section to reverse.</param>
        public static void Reverse<T>(this T[] self, int index, int length)
        {
            Array.Reverse(self, index, length);
        }

        /// <summary>
        /// Sets elements of the array to their default value.
        /// </summary>
        public static void Clear<T>(this T[] self)
        {
            Array.Clear(self, 0, self.Length);
        }

        /// <summary>
        /// Sets elements of the array to their default value.
        /// </summary>
        /// <param name="index">The starting index of the range of elements to clear.</param>
        /// <param name="length">The number of elements to clear.</param>
        public static void Clear<T>(this T[] self, int index, int length)
        {
            Array.Clear(self, index, length);
        }

        /// <summary>
        /// Returns new array contained elements got by converting the array.
        /// </summary>
        /// <param name="converter">Converting function.</param>
        public static TOut[] GetConverted<TIn, TOut>(this TIn[] self, Converter<TIn, TOut> converter)
        {
            return Array.ConvertAll(self, converter);
        }

        /// <summary>
        /// Returns whether array contains the specified item.
        /// </summary>
        public static bool Contains<T>(this T[] self, T item)
        {
            return (self as IList<T>).Contains(item);
        }

#if UNITY_2021_2_OR_NEWER
        /// <summary>
        /// Returns ArraySegment with the specified range.
        /// </summary>
        public static ArraySegment<T> Slice<T>(this T[] self, int startIndex, int length)
        {
            return new ArraySegment<T>(self, startIndex, length);
        }

        /// <summary>
        /// Returns ArraySegment with the specified range.
        /// </summary>
        public static ArraySegment<T> Slice<T>(this T[] self, int startIndex)
        {
            return new ArraySegment<T>(self, startIndex, self.Length - startIndex);
        }

        /// <summary>
        /// Enumerates array within the specified range.
        /// </summary>
        [Obsolete("This method is deprecated. Use Slice instead.")]
        public static ArraySegment<T> Enumerate<T>(this T[] self, int startIndex, int length)
        {
            return new ArraySegment<T>(self, startIndex, length);
        }

        /// <summary>
        /// Enumerates array within the specified range.
        /// </summary>
        [Obsolete("This method is deprecated. Use Slice instead.")]
        public static ArraySegment<T> Enumerate<T>(this T[] self, int startIndex)
        {
            return new ArraySegment<T>(self, startIndex, self.Length - startIndex);
        }
#else
        /// <summary>
        /// Enumerates array within the specified range.
        /// </summary>
        public static Collections.Iterators.ArrayEnumerableQuery<T> Enumerate<T>(this T[] self, int startIndex, int length)
        {
            return new Collections.Iterators.ArrayEnumerableQuery<T>(self, startIndex, length);
        }

        /// <summary>
        /// Enumerates array within the specified range.
        /// </summary>
        public static Collections.Iterators.ArrayEnumerableQuery<T> Enumerate<T>(this T[] self, int startIndex)
        {
            return new Collections.Iterators.ArrayEnumerableQuery<T>(self, startIndex);
        }
#endif
    }
}
