using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.CSharp
{
    internal static class SortUtility
    {
        public static void Sort<T, TComp>(T[] array, int left, int right, TComp comparer) where TComp : IComparer<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = array[left];

            while (i <= j)
            {
                while (comparer.Compare(array[i], pivot) < 0) { i++; }
                while (comparer.Compare(array[j], pivot) > 0) { j--; }

                if (i <= j)
                    array.Swap(i++, j--);
            }

            Sort(array, left, j, comparer);
            Sort(array, i, right, comparer);
        }

        public static void Sort<T, TComp>(Span<T> span, int left, int right, TComp comparer)
            where T : unmanaged
            where TComp : IComparer<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = span[left];

            while (i <= j)
            {
                while (comparer.Compare(span[i], pivot) < 0) { i++; }
                while (comparer.Compare(span[j], pivot) > 0) { j--; }

                if (i <= j)
                    span.Swap(i++, j--);
            }

            Sort(span, left, j, comparer);
            Sort(span, i, right, comparer);
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

        #region Comparers
        public struct DefaultComparer<T> : IComparer<T>
        {
            public Comparison<T> Comparison;

            public int Compare(T x, T y) => Comparison(x, y);
        }

        public struct DescendingComparer<T> : IComparer<T>
        {
            public IComparer<T> Comparer;

            public int Compare(T x, T y) => -Comparer.Compare(x, y);
        }

        public struct KeyComparerA<TSource, TKey> : IComparer<TSource>
        {
            public Func<TSource, TKey> KeySelector;
            public IComparer<TKey> Comparer;

            public int Compare(TSource x, TSource y) => Comparer.Compare(KeySelector(x), KeySelector(y));
        }

        public struct KeyComparerB<TSource, TKey> : IComparer<TSource>
        {
            public Func<TSource, TKey> KeySelector;
            public Comparison<TKey> Comparison;

            public int Compare(TSource x, TSource y) => Comparison(KeySelector(x), KeySelector(y));
        }

        public struct DescendingKeyComparer<TSource, TKey> : IComparer<TSource>
        {
            public Func<TSource, TKey> KeySelector;
            public IComparer<TKey> Comparer;

            public int Compare(TSource x, TSource y) => -Comparer.Compare(KeySelector(x), KeySelector(y));
        }
        #endregion
    }
}
