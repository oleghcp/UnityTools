using System;
using System.Collections.Generic;
using UnityUtility.CSharp.Collections;
using UnityUtility.Rng;
using UnityUtility.Tools;

namespace UnityUtility.CSharp
{
    public static class SpanExtensions
    {
        public static void Sort<T, TKey>(this Span<T> self, Func<T, TKey> selector) where TKey : IComparable<TKey>
        {
            if (self.Length < CollectionUtility.QUICK_SORT_MIN_SIZE)
            {
                SpanUtility.SelectionSort(self, selector);
                return;
            }

            SpanUtility.QuickSort(self, 0, self.Length - 1, selector);
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

        public static void CopyTo<T>(this in Span<T> self, T[] destination)
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
        }

        public static List<T> ToList<T>(this in Span<T> self)
        {
            List<T> dest = new List<T>(self.Length * 2);

            for (int i = 0; i < self.Length; i++)
            {
                dest.Add(self[i]);
            }

            return dest;
        }

        public static int IndexOf<T>(this in Span<T> self, Predicate<T> condition)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        public static void Shuffle<T>(this Span<T> self, IRng generator)
        {
            SpanUtility.Shuffle(self, generator);
        }

        public static void Shuffle<T>(this Span<T> self)
        {
            SpanUtility.Shuffle(self, RandomNumberGenerator.Default);
        }

        public static T Find<T>(this in Span<T> self, Predicate<T> match)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (match(self[i]))
                    return self[i];
            }

            return default;
        }

        public static void ForEach<T>(this in Span<T> self, Action<T> action)
        {
            for (int i = 0; i < self.Length; i++)
                action(self[i]);
        }

        public static bool Contains<T>(this in Span<T> self, Predicate<T> condition) where T : IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return true;
            }

            return false;
        }

        public static T Min<T>(this in Span<T> self) where T : IComparable<T>
        {
            return SpanUtility.Min(self);
        }

        public static T Max<T>(this in Span<T> self) where T : IComparable<T>
        {
            return SpanUtility.Max(self);
        }

        public static void Reverse<T>(this Span<T> self, int startIndex, int length)
        {
            int backIndex = startIndex + length - 1;
            length /= 2;

            for (int i = 0; i < length; i++)
            {
                Helper.Swap(ref self[startIndex + i], ref self[backIndex - i]);
            }
        }

        public static void Swap<T>(this Span<T> self, int i, int j)
        {
            Helper.Swap(ref self[i], ref self[j]);
        }

        public static void Fill<T>(this Span<T> self, T value, int startIndex, int count)
        {
            if ((uint)startIndex >= (uint)self.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + count > self.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            for (int i = startIndex; i < count + startIndex; i++)
            {
                self[i] = value;
            }
        }

        public static void Fill<T>(this Span<T> self, Func<int, T> factory)
        {
            for (int i = 0; i < self.Length; i++)
            {
                self[i] = factory(i);
            }
        }

        public static void Fill<T>(this Span<T> self, Func<int, T> factory, int startIndex, int count)
        {
            if ((uint)startIndex >= (uint)self.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + count > self.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            for (int i = startIndex; i < count + startIndex; i++)
            {
                self[i] = factory(i);
            }
        }

        public static T GetRandomItem<T>(this in Span<T> self, IRng generator)
        {
            if (self.Length == 0)
                throw ThrowErrors.NoElements();

            return self[generator.Next(self.Length)];
        }

        public static T GetRandomItem<T>(this in Span<T> self)
        {
            return GetRandomItem(self, RandomNumberGenerator.Default);
        }

#if UNITY_2021_2_OR_NEWER
        public static int Sum(this in ReadOnlySpan<int> self)
        {
            int sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        public static float Sum(this in ReadOnlySpan<float> self)
        {
            float sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        public static void CopyTo<T>(this in ReadOnlySpan<T> self, T[] destination)
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
        }

        public static List<T> ToList<T>(this in ReadOnlySpan<T> self)
        {
            List<T> dest = new List<T>(self.Length * 2);

            for (int i = 0; i < self.Length; i++)
            {
                dest.Add(self[i]);
            }

            return dest;
        }

        public static int IndexOf<T>(this in ReadOnlySpan<T> self, Predicate<T> condition)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        public static T Find<T>(this in ReadOnlySpan<T> self, Predicate<T> match)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (match(self[i]))
                    return self[i];
            }

            return default;
        }

        public static void ForEach<T>(this in ReadOnlySpan<T> self, Action<T> action)
        {
            for (int i = 0; i < self.Length; i++)
                action(self[i]);
        }

        public static bool Contains<T>(this in ReadOnlySpan<T> self, Predicate<T> condition) where T : IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return true;
            }

            return false;
        }

        public static T Min<T>(this in ReadOnlySpan<T> self) where T : IComparable<T>
        {
            return SpanUtility.Min(self);
        }

        public static T Max<T>(this in ReadOnlySpan<T> self) where T : IComparable<T>
        {
            return SpanUtility.Max(self);
        }

        public static T GetRandomItem<T>(this in ReadOnlySpan<T> self, IRng generator)
        {
            if (self.Length == 0)
                throw ThrowErrors.NoElements();

            return self[generator.Next(self.Length)];
        }

        public static T GetRandomItem<T>(this in ReadOnlySpan<T> self)
        {
            return GetRandomItem(self, RandomNumberGenerator.Default);
        }
#endif
    }
}
