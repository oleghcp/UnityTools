using System.Collections.Generic;
using OlegHcp.CSharp;

namespace System
{
    public static class SpanExtensions
    {
        public static unsafe void Sort<T>(this Span<T> self) where T : unmanaged, IComparable<T>
        {
            SortUtility.Sort(self, 0, self.Length - 1, Comparer<T>.Default);
        }

        public static unsafe void Sort<T>(this Span<T> self, Comparison<T> comparison) where T : unmanaged
        {
            SortUtility.Sort(self, 0, self.Length - 1, new SortUtility.DefaultComparer<T> { Comparison = comparison });
        }

        public static unsafe void Sort<T, TComparer>(this Span<T> self, TComparer comparer)
            where T : unmanaged
            where TComparer : IComparer<T>
        {
            SortUtility.Sort(self, 0, self.Length - 1, comparer);
        }

        public static bool Contains<T>(this Span<T> self, T item) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (item.Equals(self[i]))
                    return true;
            }

            return false;
        }
    }
}
