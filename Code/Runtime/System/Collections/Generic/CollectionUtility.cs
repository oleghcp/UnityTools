using UnityUtility;
using UnityUtilityTools;

namespace System.Collections.Generic
{
    internal static class CollectionUtility
    {
        public static void Shuffle<T>(IList<T> collection, IRng generator)
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

        public static void Shuffle<T>(IList<T> collection)
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

        public static T GetRandomItem<T>(IList<T> collection, IRng generator)
        {
            int index = generator.Next(collection.Count);
            return collection[index];
        }

        public static T GetRandomItem<T>(IList<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection[index];
        }

        public static void QuickSort<T>(IList<T> array, int left, int right, Comparison<T> comparison)
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

        public static class ValueTypes
        {
            public static int Min<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : struct, IComparable<TKey>
            {
                if (collection == null)
                    throw new ArgumentNullException(nameof(collection));

                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                int index = 0;
                TKey minKey = default;
                result = default;

                bool nonFirstIteration = false;

                int i = 0;
                foreach (var item in collection)
                {
                    if (nonFirstIteration)
                    {
                        TKey key = keySelector(item);

                        if (key.CompareTo(minKey) < 0)
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
                        nonFirstIteration = true;
                    }

                    ++i;
                }

                if (nonFirstIteration)
                    return index;

                throw Errors.NoElements();
            }

            public static int Max<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : struct, IComparable<TKey>
            {
                if (collection == null)
                    throw new ArgumentNullException(nameof(collection));

                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                int index = 0;
                TKey maxKey = default;
                result = default;

                bool nonFirstIteration = false;

                int i = 0;
                foreach (var item in collection)
                {
                    if (nonFirstIteration)
                    {
                        TKey key = keySelector(item);

                        if (key.CompareTo(maxKey) > 0)
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
                        nonFirstIteration = true;
                    }

                    ++i;
                }

                if (nonFirstIteration)
                    return index;

                throw Errors.NoElements();
            }
        }

        public static class RefTypes
        {
            public static int Min<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : class, IComparable<TKey>
            {
                if (collection == null)
                    throw new ArgumentNullException(nameof(collection));

                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                int index = 0;
                TKey minKey = default;
                result = default;

                bool nonFirstIteration = false;

                int i = 0;
                foreach (var item in collection)
                {
                    if (nonFirstIteration)
                    {
                        TKey key = keySelector(item);

                        if (Helper.Compare(key, minKey) < 0)
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
                        nonFirstIteration = true;
                    }

                    ++i;
                }

                if (nonFirstIteration)
                    return index;

                throw Errors.NoElements();
            }

            public static int Max<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : class, IComparable<TKey>
            {
                if (collection == null)
                    throw new ArgumentNullException(nameof(collection));

                if (keySelector == null)
                    throw new ArgumentNullException(nameof(keySelector));

                int index = 0;
                TKey maxKey = default;
                result = default;

                bool nonFirstIteration = false;

                int i = 0;
                foreach (var item in collection)
                {
                    if (nonFirstIteration)
                    {
                        TKey key = keySelector(item);

                        if (Helper.Compare(key, maxKey) > 0)
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
                        nonFirstIteration = true;
                    }

                    ++i;
                }

                if (nonFirstIteration)
                    return index;

                throw Errors.NoElements();
            }
        }
    }
}
