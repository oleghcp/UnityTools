using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public static class CollectionExtensionsForValueTypes
    {
        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        public static int IndexOfMin<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            return CollectionUtility.ValueTypes.Min(self, selector, out _);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        public static int IndexOfMax<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            return CollectionUtility.ValueTypes.Max(self, selector, out _);
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            CollectionUtility.ValueTypes.Min(self, selector, out TSource res);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            CollectionUtility.ValueTypes.Max(self, selector, out TSource res);
            return res;
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector) where TKey : struct, IComparable<TKey>
        {
            Array.Sort(self, (itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this List<TSource> self, Func<TSource, TKey> keySelector) where TKey : struct, IComparable<TKey>
        {
            self.Sort((itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> keySelector) where TKey : struct, IComparable<TKey>
        {
            CollectionUtility.QuickSort(self, 0, self.Count - 1, (itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }
    }

    public static class ArrayExtensionsForEnums
    {
        /// <summary>
        /// Sorts by selected key.
        /// </summary>
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector) where TKey : Enum
        {
            Array.Sort(self, (itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this List<TSource> self, Func<TSource, TKey> keySelector) where TKey : Enum
        {
            self.Sort((itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> keySelector) where TKey : Enum
        {
            CollectionUtility.QuickSort(self, 0, self.Count - 1, (itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }
    }
}
