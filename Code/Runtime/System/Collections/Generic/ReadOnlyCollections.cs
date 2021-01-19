using System.Runtime.CompilerServices;
using UnityUtility;

namespace System.Collections.Generic
{
    public static class ReadOnlyCollections
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(IReadOnlyList<T> collection, T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < collection.Count; i++)
            {
                if (comparer.Equals(collection[i], item))
                    return i;
            }

            return -1;
        }

        public static int IndexOf<T>(IReadOnlyList<T> collection, Predicate<T> condition)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (condition(collection[i]))
                    return i;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMinRef<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector) where TKey : class, IComparable<TKey>
        {
            return CollectionUtility.RefTypes.Min(collection, selector, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMaxRef<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector) where TKey : class, IComparable<TKey>
        {
            return CollectionUtility.RefTypes.Max(collection, selector, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMinValue<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            return CollectionUtility.ValueTypes.Min(collection, selector, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMaxValue<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector) where TKey : struct, IComparable<TKey>
        {
            return CollectionUtility.ValueTypes.Max(collection, selector, out _);
        }

        public static T GetRandomItem<T>(IReadOnlyList<T> collection, IRng generator)
        {
            int index = generator.Next(collection.Count);
            return collection[index];
        }

        public static T GetRandomItem<T>(IReadOnlyList<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection[index];
        }


    }
}
