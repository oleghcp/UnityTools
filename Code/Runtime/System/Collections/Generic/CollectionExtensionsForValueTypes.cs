using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public static class CollectionExtensionsForValueTypes
    {
        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        public static int IndexOfMin<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            return CollectionHelper.ValueMin(collection, selector, out _);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        public static int IndexOfMax<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            return CollectionHelper.ValueMax(collection, selector, out _);
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            CollectionHelper.ValueMin(collection, selector, out TSource res);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IList<TSource> collection, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            if (collection.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            CollectionHelper.ValueMax(collection, selector, out TSource res);
            return res;
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this TSource[] array, Func<TSource, TKey> keySelector) where TKey : struct, IComparable<TKey>
        {
            Array.Sort(array, (itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }

        /// <summary>
        /// Sorts by selected key.
        /// </summary>                
        /// <param name="keySelector">Reference to selecting function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<TSource, TKey>(this List<TSource> list, Func<TSource, TKey> keySelector) where TKey : struct, IComparable<TKey>
        {
            list.Sort((itm1, itm2) => keySelector(itm1).CompareTo(keySelector(itm2)));
        }
    }
}
