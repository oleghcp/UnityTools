using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityUtility;

namespace System.Collections.Generic
{
    public static class CollectionUtility
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(ReadOnlyCollection<T> collection, T item)
        {
            return collection.IndexOf(item);
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
            return Min(collection, selector, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMax<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> selector)
        {
            return Max(collection, selector, out _);
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

        ///////////////
        // Internals //
        ///////////////

        internal static void Shuffle<T>(IList<T> collection, IRng generator)
        {
            int last = collection.Count;

            while (last > 1)
            {
                int cur = generator.Next(last--);

                T value = collection[cur];
                collection[cur] = collection[last];
                collection[last] = value;
            }
        }

        internal static void Shuffle<T>(IList<T> collection)
        {
            int last = collection.Count;

            while (last > 1)
            {
                int cur = UnityEngine.Random.Range(0, last--);

                T value = collection[cur];
                collection[cur] = collection[last];
                collection[last] = value;
            }
        }

        internal static T GetRandomItem<T>(IList<T> collection, IRng generator)
        {
            int index = generator.Next(collection.Count);
            return collection[index];
        }

        internal static T GetRandomItem<T>(IList<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection[index];
        }

        internal static T PullOutRandomItem<T>(IList<T> collection, IRng generator)
        {
            int index = generator.Next(collection.Count);
            return collection.PullOut(index);
        }

        internal static T PullOutRandomItem<T>(IList<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection.PullOut(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void QuickSort<T>(IList<T> array, int left, int right, IComparer<T> comparer)
        {
            QuickSort(array, left, right, comparer.Compare);
        }

        internal static void QuickSort<T>(IList<T> array, int left, int right, Comparison<T> comparison)
        {
            int i = left, j = right;
            T pivot = array[(left + right) / 2];

            while (i < j)
            {
                while (comparison(array[i], pivot) < 0) { i++; }
                while (comparison(array[j], pivot) > 0) { j--; }

                if (i <= j)
                {
                    T tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;

                    i++;
                    j--;
                }
            }

            if (left < j)
                QuickSort(array, left, j, comparison);

            if (i < right)
                QuickSort(array, i, right, comparison);
        }

        internal static int Min<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            TKey minKey = default;
            result = default;

            bool nonFirstIteration = false;

            int i = 0;
            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (comparer.Compare(key, minKey) < 0)
                    {
                        minKey = key;
                        result = item;
                        index = i;
                    }
                }
                else
                {
                    minKey = keySelector(item);
                    result = item;
                    index = i;
                    nonFirstIteration = true;
                }

                ++i;
            }

            return index;
        }

        internal static int Max<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            TKey maxKey = default;
            result = default;

            bool nonFirstIteration = false;

            int i = 0;
            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (comparer.Compare(key, maxKey) > 0)
                    {
                        maxKey = key;
                        result = item;
                        index = i;
                    }
                }
                else
                {
                    maxKey = keySelector(item);
                    result = item;
                    index = i;
                    nonFirstIteration = true;
                }

                ++i;
            }

            return index;
        }
    }
}
