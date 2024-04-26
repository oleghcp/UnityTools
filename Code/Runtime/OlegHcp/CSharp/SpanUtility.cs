using System;
using System.Collections.Generic;

namespace OlegHcp.CSharp
{
    internal static class SpanUtility
    {
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
                    span.Swap(i++, j--);
            }

            Sort(span, left, j, comparer);
            Sort(span, i, right, comparer);
        }
    }
}
