using System;
using System.Collections.Generic;

namespace OlegHcp.CSharp.Collections
{
    internal static class CollectionUtility
    {
        #region Sort
        public static void Sort<T>(IList<T> collection, int left, int right) where T : IComparable<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = collection[left];

            while (i <= j)
            {
                while (collection[i].CompareTo(pivot) < 0) { i++; }
                while (collection[j].CompareTo(pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            Sort(collection, left, j);
            Sort(collection, i, right);
        }

        public static void Sort<T, TComp>(IList<T> collection, int left, int right, TComp comparer) where TComp : IComparer<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = collection[left];

            while (i <= j)
            {
                while (comparer.Compare(collection[i], pivot) < 0) { i++; }
                while (comparer.Compare(collection[j], pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            Sort(collection, left, j, comparer);
            Sort(collection, i, right, comparer);
        }

        public static void Sort<T>(IList<T> collection, int left, int right, Comparison<T> comparison)
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = collection[left];

            while (i <= j)
            {
                while (comparison(collection[i], pivot) < 0) { i++; }
                while (comparison(collection[j], pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            Sort(collection, left, j, comparison);
            Sort(collection, i, right, comparison);
        }
        #endregion

        #region Min/max selection
        public static void Min<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey minKey)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;

            minKey = default;
            result = default;

            bool nonFirstIteration = false;

            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (comparer.Compare(key, minKey) < 0)
                    {
                        minKey = key;
                        result = item;
                    }
                }
                else
                {
                    minKey = keySelector(item);
                    result = item;
                    nonFirstIteration = true;
                }
            }
        }

        public static void Max<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey maxKey)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;

            maxKey = default;
            result = default;

            bool nonFirstIteration = false;

            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (comparer.Compare(key, maxKey) > 0)
                    {
                        maxKey = key;
                        result = item;
                    }
                }
                else
                {
                    maxKey = keySelector(item);
                    result = item;
                    nonFirstIteration = true;
                }
            }
        }

        public static int Min<TSource, TKey>(IList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey minKey)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            minKey = default;
            result = default;

            bool nonFirstIteration = false;

            for (int i = 0; i < collection.Count; i++)
            {
                TSource item = collection[i];

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
            }

            return index;
        }

        public static int Max<TSource, TKey>(IList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey maxKey)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            maxKey = default;
            result = default;

            bool nonFirstIteration = false;

            for (int i = 0; i < collection.Count; i++)
            {
                TSource item = collection[i];

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
            }

            return index;
        }

        public static int Min<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey minKey)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            minKey = default;
            result = default;

            bool nonFirstIteration = false;

            for (int i = 0; i < collection.Count; i++)
            {
                TSource item = collection[i];

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
            }

            return index;
        }

        public static int Max<TSource, TKey>(IReadOnlyList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result, out TKey maxKey)
        {
            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = -1;
            maxKey = default;
            result = default;

            bool nonFirstIteration = false;

            for (int i = 0; i < collection.Count; i++)
            {
                TSource item = collection[i];

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
            }

            return index;
        }
        #endregion

        #region Comparers
        public struct KeyComparerA<TSource, TKey> : IComparer<TSource>
        {
            public Func<TSource, TKey> KeySelector;
            public IComparer<TKey> Comparer;

            public int Compare(TSource x, TSource y)
            {
                return Comparer.Compare(KeySelector(x), KeySelector(y));
            }
        }

        public struct KeyComparerB<TSource, TKey> : IComparer<TSource>
        {
            public Func<TSource, TKey> KeySelector;
            public Comparison<TKey> Comparison;

            public int Compare(TSource x, TSource y)
            {
                return Comparison(KeySelector(x), KeySelector(y));
            }
        }

        public struct DescendingComparer<T> : IComparer<T>
        {
            public IComparer<T> Comparer;

            public int Compare(T x, T y)
            {
                return -Comparer.Compare(x, y);
            }
        }

        public struct DescendingKeyComparer<TSource, TKey> : IComparer<TSource>
        {
            public Func<TSource, TKey> KeySelector;
            public IComparer<TKey> Comparer;

            public int Compare(TSource x, TSource y)
            {
                return -Comparer.Compare(KeySelector(x), KeySelector(y));
            }
        }
        #endregion
    }
}
