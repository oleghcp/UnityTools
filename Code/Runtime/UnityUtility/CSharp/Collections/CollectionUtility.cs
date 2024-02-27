using System;
using System.Collections.Generic;

namespace OlegHcp.CSharp.Collections
{
    internal static class CollectionUtility
    {
        public const int QUICK_SORT_MIN_SIZE = 5;

        #region Sort
        public static void QuickSort<T>(IList<T> collection, int left, int right) where T : IComparable<T>
        {
            int i = left;
            int j = right;
            T pivot = collection[(left + right) / 2];

            while (i < j)
            {
                while (collection[i].CompareTo(pivot) < 0) { i++; }
                while (collection[j].CompareTo(pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            if (left < j)
                QuickSort(collection, left, j);

            if (i < right)
                QuickSort(collection, i, right);
        }

        public static void QuickSort<T>(IList<T> collection, int left, int right, IComparer<T> comparer)
        {
            int i = left;
            int j = right;
            T pivot = collection[(left + right) / 2];

            while (i < j)
            {
                while (comparer.Compare(collection[i], pivot) < 0) { i++; }
                while (comparer.Compare(collection[j], pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            if (left < j)
                QuickSort(collection, left, j, comparer);

            if (i < right)
                QuickSort(collection, i, right, comparer);
        }

        public static void QuickSort<T>(IList<T> collection, int left, int right, Comparison<T> comparison)
        {
            int i = left;
            int j = right;
            T pivot = collection[(left + right) / 2];

            while (i < j)
            {
                while (comparison(collection[i], pivot) < 0) { i++; }
                while (comparison(collection[j], pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            if (left < j)
                QuickSort(collection, left, j, comparison);

            if (i < right)
                QuickSort(collection, i, right, comparison);
        }

        public static void SelectionSort<T>(IList<T> collection) where T : IComparable<T>
        {
            int count = collection.Count - 1;

            for (int i = 0; i < count; i++)
            {
                int indexOfMin = i;

                for (int j = i + 1; j < collection.Count; j++)
                {
                    if (collection[indexOfMin].CompareTo(collection[j]) > 0)
                        indexOfMin = j;
                }

                if (indexOfMin != i)
                    collection.Swap(i, indexOfMin);
            }
        }

        public static void SelectionSort<T>(IList<T> collection, IComparer<T> comparer)
        {
            int count = collection.Count - 1;

            for (int i = 0; i < count; i++)
            {
                int indexOfMin = i;

                for (int j = i + 1; j < collection.Count; j++)
                {
                    if (comparer.Compare(collection[indexOfMin], collection[j]) > 0)
                        indexOfMin = j;
                }

                if (indexOfMin != i)
                    collection.Swap(i, indexOfMin);
            }
        }

        public static void SelectionSort<T>(IList<T> collection, Comparison<T> comparison)
        {
            int count = collection.Count - 1;

            for (int i = 0; i < count; i++)
            {
                int indexOfMin = i;

                for (int j = i + 1; j < collection.Count; j++)
                {
                    if (comparison(collection[indexOfMin], collection[j]) > 0)
                        indexOfMin = j;
                }

                if (indexOfMin != i)
                    collection.Swap(i, indexOfMin);
            }
        }
        #endregion

        public static int Min<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey minKey)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            minKey = default;
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

        public static int Max<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey maxKey)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            maxKey = default;
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
