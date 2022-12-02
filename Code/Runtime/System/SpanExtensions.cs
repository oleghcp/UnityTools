#if !UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using UnityUtility;
using UnityUtility.CSharp;

namespace System
{
    public static class SpanExtensions
    {
        public static void CopyTo<T>(this in Span<T> self, Span<T> destination) where T : unmanaged, IEquatable<T>
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
        }

        public static bool TryCopyTo<T>(this in Span<T> self, Span<T> destination) where T : unmanaged, IEquatable<T>
        {
            if ((uint)self.Length <= (uint)destination.Length)
            {
                for (int i = 0; i < self.Length; i++)
                {
                    destination[i] = self[i];
                }
                return true;
            }
            return false;
        }

        public static int IndexOf<T>(this in Span<T> self, T item) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (item.Equals(self[i]))
                    return i;
            }

            return -1;
        }

        public static unsafe void Sort<T>(this in Span<T> self) where T : unmanaged, IComparable<T>
        {
            if (self.IsEmpty)
                return;

            SpanUtility.QuickSort(self, 0, self.Length - 1);
        }

        public static unsafe void Sort<T>(this in Span<T> self, Comparison<T> comparer) where T : unmanaged
        {
            if (self.IsEmpty)
                return;

            SpanUtility.QuickSort(self, 0, self.Length - 1, comparer);
        }

        public static unsafe void Sort<T, TComparer>(this in Span<T> self, TComparer comparer)
            where T : unmanaged
            where TComparer : IComparer<T>
        {
            if (self.IsEmpty)
                return;

            SpanUtility.QuickSort(self, 0, self.Length - 1, comparer);
        }

        public static bool Contains<T>(this in Span<T> self, T item) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (item.Equals(self[i]))
                    return true;
            }

            return false;
        }

        public static void Reverse<T>(this in Span<T> self) where T : unmanaged
        {
            self.Reverse(0, self.Length);
        }
    }
}
#endif
