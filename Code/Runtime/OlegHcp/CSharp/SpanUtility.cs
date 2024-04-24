using System;
using System.Collections.Generic;
using OlegHcp.Tools;

namespace OlegHcp.CSharp
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

        public static T Min<T>(in Span<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw ThrowErrors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) < 0)
                    num = span[i];
            }

            return num;
        }

        public static T Max<T>(in Span<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw ThrowErrors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) > 0)
                    num = span[i];
            }

            return num;
        }

#if UNITY_2021_2_OR_NEWER
        public static T Min<T>(in ReadOnlySpan<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw ThrowErrors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) < 0)
                    num = span[i];
            }

            return num;
        }

        public static T Max<T>(in ReadOnlySpan<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw ThrowErrors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) > 0)
                    num = span[i];
            }

            return num;
        }
#endif

        #region Sort
        public static void Sort<T, TComparer>(Span<T> span, int left, int right, TComparer comparer)
            where T : unmanaged
            where TComparer : IComparer<T>
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
                    Helper.Swap(ref span[i++], ref span[j--]);
            }

            Sort(span, left, j, comparer);
            Sort(span, i, right, comparer);
        }
        #endregion
    }
}
