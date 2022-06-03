using System.Runtime.CompilerServices;
using UnityUtility;

namespace System.Collections.Generic
{
    public static class ReadOnlyListUtility
    {
        public static T[] GetSubArray<T>(IReadOnlyList<T> self, int startIndex, int length)
        {
            T[] subArray = new T[length];
            for (int i = 0; i < length; i++) { subArray[i] = self[i + startIndex]; }
            return subArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetSubArray<T>(IReadOnlyList<T> self, int startIndex)
        {
            return GetSubArray(self, startIndex, self.Count - startIndex);
        }

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
        public static int IndexOfMin<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Min(collection, selector, out _, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMax<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Max(collection, selector, out _, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMin<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector, out TKey min)
        {
            return CollectionUtility.Min(collection, selector, out _, out min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMax<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector, out TKey max)
        {
            return CollectionUtility.Max(collection, selector, out _, out max);
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
