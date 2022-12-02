using System.Collections.Generic;
using UnityUtility;
using UnityUtility.Tools;

namespace System
{
    internal static class SpanUtility
    {
        public static void Shuffle<T>(Span<T> span, IRng generator) where T : unmanaged
        {
            int last = span.Length;

            while (last > 1)
            {
                int cur = generator.Next(last--);
                Helper.Swap(ref span[cur], ref span[last]);
            }
        }

        public static void Shuffle<T>(Span<T> span) where T : unmanaged
        {
            int last = span.Length;

            while (last > 1)
            {
                int cur = UnityEngine.Random.Range(0, last--);
                Helper.Swap(ref span[cur], ref span[last]);
            }
        }

        public static T Min<T>(Span<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw Errors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) < 0)
                    num = span[i];
            }

            return num;
        }

        public static T Max<T>(Span<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw Errors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) > 0)
                    num = span[i];
            }

            return num;
        }

        public static unsafe void QuickSort<T>(Span<T> span, int left, int right) where T : unmanaged, IComparable<T>
        {
            int i = left, j = right;
            T pivot = span[(left + right) / 2];

            while (i < j)
            {
                while (span[i].CompareTo(pivot) < 0) { i++; }
                while (span[j].CompareTo(pivot) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref span[i++], ref span[j--]);
            }

            if (left < j)
                QuickSort(span, left, j);

            if (i < right)
                QuickSort(span, i, right);
        }

        public static unsafe void QuickSort<T>(Span<T> span, int left, int right, Comparison<T> comparer) where T : unmanaged
        {
            int i = left, j = right;
            T pivot = span[(left + right) / 2];

            while (i < j)
            {
                while (comparer(span[i], pivot) < 0) { i++; }
                while (comparer(span[j], pivot) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref span[i++], ref span[j--]);
            }

            if (left < j)
                QuickSort(span, left, j, comparer);

            if (i < right)
                QuickSort(span, i, right, comparer);
        }

        public static unsafe void QuickSort<T, TComparer>(Span<T> span, int left, int right, TComparer comparer)
            where T : unmanaged
            where TComparer : IComparer<T>
        {
            int i = left, j = right;
            T pivot = span[(left + right) / 2];

            while (i < j)
            {
                while (comparer.Compare(span[i], pivot) < 0) { i++; }
                while (comparer.Compare(span[j], pivot) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref span[i++], ref span[j--]);
            }

            if (left < j)
                QuickSort(span, left, j, comparer);

            if (i < right)
                QuickSort(span, i, right, comparer);
        }

        public static unsafe void QuickSort<T, TKey>(Span<T> span, int left, int right, Func<T, TKey> selector)
            where T : unmanaged
            where TKey : IComparable<TKey>
        {
            int i = left, j = right;
            TKey pivotKey = selector(span[(left + right) / 2]);

            while (i < j)
            {
                while (selector(span[i]).CompareTo(pivotKey) < 0) { i++; }
                while (selector(span[j]).CompareTo(pivotKey) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref span[i++], ref span[j--]);
            }

            if (left < j)
                QuickSort(span, left, j, selector);

            if (i < right)
                QuickSort(span, i, right, selector);
        }
    }
}
