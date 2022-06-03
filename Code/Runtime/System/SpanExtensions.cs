#if UNITY_2018_3_OR_NEWER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility;
using UnityUtilityTools;

namespace System
{
    public static class SpanExtensions
    {
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

        public static void CopyTo<T>(this in Span<T> self, T[] destination) where T : unmanaged
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
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

        public static int IndexOf<T>(this in Span<T> self, Predicate<T> condition) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

#if !UNITY_2021_2_OR_NEWER
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

            UnsafeArrayUtility.QuickSort(self.Ptr, 0, self.Length - 1);
        }

        public static unsafe void Sort<T>(this in Span<T> self, Comparison<T> comparer) where T : unmanaged
        {
            if (self.IsEmpty)
                return;

            UnsafeArrayUtility.QuickSort(self.Ptr, 0, self.Length - 1, comparer);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this in Span<T> self) where T : unmanaged
        {
            Reverse(self, 0, self.Length);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this in Span<T> self, IRng generator) where T : unmanaged
        {
#if UNITY_2021_2_OR_NEWER
            SpanUtility.Shuffle(self, generator);
#else
            unsafe
	        {
		        UnsafeArrayUtility.Shuffle(self.Ptr, self.Length, generator); 
	        }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this in Span<T> self) where T : unmanaged
        {
#if UNITY_2021_2_OR_NEWER
            SpanUtility.Shuffle(self);
#else
            unsafe
	        {
		        UnsafeArrayUtility.Shuffle(self.Ptr, self.Length); 
	        }
#endif
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

        public static bool Contains<T>(this in Span<T> self, Predicate<T> condition) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>(this in Span<T> self) where T : unmanaged, IComparable<T>
        {
#if UNITY_2021_2_OR_NEWER
            return SpanUtility.Min(self);
#else
            unsafe
	        {
		        return UnsafeArrayUtility.Min(self.Ptr, self.Length); 
	        }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>(this in Span<T> self) where T : unmanaged, IComparable<T>
        {
#if UNITY_2021_2_OR_NEWER
            return SpanUtility.Max(self);
#else
            unsafe
	        {
		        return UnsafeArrayUtility.Max(self.Ptr, self.Length); 
	        }
#endif
        }

        public static void Reverse<T>(this in Span<T> self, int startIndex, int length) where T : unmanaged
        {
            int backIndex = startIndex + length - 1;
            length /= 2;

            for (int i = 0; i < length; i++)
            {
                Helper.Swap(ref self[startIndex + i], ref self[backIndex - i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(this in Span<T> self, int i, int j) where T : unmanaged
        {
            Helper.Swap(ref self[i], ref self[j]);
        }
    }
}
#endif
