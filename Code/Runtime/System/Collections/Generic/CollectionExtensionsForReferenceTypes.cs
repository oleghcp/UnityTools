using System.Runtime.CompilerServices;
using UnityUtilityTools;

namespace System.Collections.Generic
{
    public static class CollectionExtensionsForReferenceTypes
    {
        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMin<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector) where TKey : class, IComparable<TKey>
        {
            return CollectionUtility.RefTypes.Min(self, selector, out _);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMax<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> selector) where TKey : class, IComparable<TKey>
        {
            return CollectionUtility.RefTypes.Max(self, selector, out _);
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector) where TKey : class, IComparable<TKey>
        {
            CollectionUtility.RefTypes.Min(self, selector, out TSource res);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector) where TKey : class, IComparable<TKey>
        {
            CollectionUtility.RefTypes.Max(self, selector, out TSource res);
            return res;
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this TSource[] self, Func<TSource, TKey> keySelector) where TKey : class, IComparable<TKey>
        {
            Array.Sort(self, (itm1, itm2) => Helper.Compare(keySelector(itm1), keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this List<TSource> self, Func<TSource, TKey> keySelector) where TKey : class, IComparable<TKey>
        {
            self.Sort((itm1, itm2) => Helper.Compare(keySelector(itm1), keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this IList<TSource> self, Func<TSource, TKey> keySelector) where TKey : class, IComparable<TKey>
        {
            CollectionUtility.QuickSort(self, 0, self.Count - 1, (itm1, itm2) => Helper.Compare(keySelector(itm1), keySelector(itm2)));
        }
    }
}
