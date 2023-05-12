using System.Collections.Generic;
using UnityUtility.CSharp;
using UnityUtility.CSharp.Collections;

namespace System
{
    public static class SpanExtensions
    {
#if !UNITY_2021_2_OR_NEWER
        public static void CopyTo<T>(this in Span<T> self, Span<T> destination) where T : IEquatable<T>
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
        }

        public static bool TryCopyTo<T>(this in Span<T> self, Span<T> destination) where T : IEquatable<T>
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

        public static int IndexOf<T>(this in Span<T> self, T item) where T : IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (item.Equals(self[i]))
                    return i;
            }

            return -1;
        }

        public static void Reverse<T>(this in Span<T> self)
        {
            self.Reverse(0, self.Length);
        }
#endif

        public static unsafe void Sort<T>(this in Span<T> self) where T : IComparable<T>
        {
            if (self.Length < CollectionUtility.QUICK_SORT_MIN_SIZE)
            {
                SpanUtility.SelectionSort(self);
                return;
            }

            SpanUtility.QuickSort(self, 0, self.Length - 1);
        }

        public static unsafe void Sort<T>(this in Span<T> self, Comparison<T> comparison)
        {
            if (self.Length < CollectionUtility.QUICK_SORT_MIN_SIZE)
            {
                SpanUtility.SelectionSort(self, comparison);
                return;
            }

            SpanUtility.QuickSort(self, 0, self.Length - 1, comparison);
        }

        public static unsafe void Sort<T, TComparer>(this in Span<T> self, TComparer comparer) where TComparer : IComparer<T>
        {
            if (self.Length < CollectionUtility.QUICK_SORT_MIN_SIZE)
            {
                SpanUtility.SelectionSort(self, comparer);
                return;
            }

            SpanUtility.QuickSort(self, 0, self.Length - 1, comparer);
        }

        public static bool Contains<T>(this in Span<T> self, T item) where T : IEquatable<T>
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
