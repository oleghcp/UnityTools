#if UNITY_2018_3_OR_NEWER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility;

namespace System
{
    public static class SpanExtensions
    {
        public static T[] ToArray<T>(this in Span<T> self) where T : unmanaged
        {
            if (self == null)
                throw new ArgumentNullException();

            T[] newArray = new T[self.Length];

            for (int i = 0; i < self.Length; i++)
            {
                newArray[i] = self[i];
            }

            return newArray;
        }

        public static List<T> ToList<T>(this in Span<T> self) where T : unmanaged
        {
            List<T> dest = new List<T>(self.Length * 2);

            for (int i = 0; i < self.Length; i++)
            {
                dest.Add(self[i]);
            }

            return dest;
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

        public static int IndexOf<T>(this in Span<T> self, Predicate<T> condition) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sort<T>(this in Span<T> self) where T : unmanaged, IComparable<T>
        {
            UnsafeArrayUtility.QuickSort(self.Ptr, 0, self.Length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sort<T>(this in Span<T> self, Comparison<T> comparer) where T : unmanaged
        {
            UnsafeArrayUtility.QuickSort(self.Ptr, 0, self.Length - 1, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Shuffle<T>(this in Span<T> self, IRng generator) where T : unmanaged
        {
            UnsafeArrayUtility.Shuffle(self.Ptr, self.Length, generator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Shuffle<T>(this in Span<T> self) where T : unmanaged
        {
            UnsafeArrayUtility.Shuffle(self.Ptr, self.Length);
        }

        public static T Find<T>(this in Span<T> self, Predicate<T> match) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (match(self[i]))
                    return self[i];
            }

            return default;
        }

        public static void ForEach<T>(this in Span<T> self, Action<T> action) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
                action(self[i]);
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

        public static bool Contains<T>(this in Span<T> self, Predicate<T> condition) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return true;
            }

            return false;
        }

        public static int Sum(this in Span<int> self)
        {
            int sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        public static float Sum(this in Span<float> self)
        {
            float sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T Min<T>(this in Span<T> self) where T : unmanaged, IComparable<T>
        {
            return UnsafeArrayUtility.Min(self.Ptr, self.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T Max<T>(this in Span<T> self) where T : unmanaged, IComparable<T>
        {
            return UnsafeArrayUtility.Max(self.Ptr, self.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this in Span<T> self) where T : unmanaged
        {
            Reverse(self, 0, self.Length);
        }

        public static void Reverse<T>(this in Span<T> self, int startIndex, int length) where T : unmanaged
        {
            int backIndex = startIndex + length - 1;
            length /= 2;

            for (int i = 0; i < length; i++)
            {
                T tmp = self[startIndex + i];
                self[startIndex + i] = self[backIndex - i];
                self[backIndex - i] = tmp;
            }
        }
    }
}
#endif