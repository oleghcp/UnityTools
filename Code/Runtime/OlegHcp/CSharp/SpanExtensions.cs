using System;
using System.Collections.Generic;
using OlegHcp.Tools;

namespace OlegHcp.CSharp
{
    public static class SpanExtensions
    {
        public static void Sort<T, TKey>(this Span<T> self, Func<T, TKey> keySelector) where T : unmanaged
        {
            var keyComparer = new SortUtility.KeyComparerA<T, TKey>
            {
                KeySelector = keySelector,
                Comparer = Comparer<TKey>.Default,
            };

            SortUtility.Sort(self, 0, self.Length - 1, keyComparer);
        }

        public static void SortDescending<T>(this Span<T> self) where T : unmanaged
        {
            SortUtility.Sort(self, 0, self.Length - 1, new SortUtility.DescendingComparer<T> { Comparer = Comparer<T>.Default, });
        }

        public static void SortDescending<T, TKey>(this Span<T> self, Func<T, TKey> keySelector) where T : unmanaged
        {
            var keyComparer = new SortUtility.DescendingKeyComparer<T, TKey>
            {
                KeySelector = keySelector,
                Comparer = Comparer<TKey>.Default,
            };

            SortUtility.Sort(self, 0, self.Length - 1, keyComparer);
        }

        public static int Sum(this Span<int> self)
        {
            int sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        public static float Sum(this Span<float> self)
        {
            float sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        public static void CopyTo<T>(this Span<T> self, T[] destination) where T : unmanaged
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
        }

        public static List<T> ToList<T>(this Span<T> self) where T : unmanaged
        {
            List<T> dest = new List<T>(self.Length * 2);

            for (int i = 0; i < self.Length; i++)
            {
                dest.Add(self[i]);
            }

            return dest;
        }

        public static int IndexOf<T>(this Span<T> self, Predicate<T> condition) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        public static void Shuffle<T>(this Span<T> self, IRng generator) where T : unmanaged
        {
            int last = self.Length;

            while (last > 1)
            {
                int cur = generator.Next(last--);
                self.Swap(cur, last);
            }
        }

        public static void Shuffle<T>(this Span<T> self) where T : unmanaged
        {
            self.Shuffle(RandomNumberGenerator.Default);
        }

        public static T Find<T>(this Span<T> self, Predicate<T> match) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (match(self[i]))
                    return self[i];
            }

            return default;
        }

        public static void ForEach<T>(this Span<T> self, Action<T> action) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
                action(self[i]);
        }

        public static bool Contains<T>(this Span<T> self, Predicate<T> condition) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return true;
            }

            return false;
        }

        public static T Min<T>(this Span<T> self) where T : unmanaged
        {
            if (self.Length <= 0)
                throw ThrowErrors.NoElements();

            Comparer<T> comparer = Comparer<T>.Default;
            T num = self[0];

            for (int i = 1; i < self.Length; i++)
            {
                if (comparer.Compare(self[i], num) < 0)
                    num = self[i];
            }

            return num;
        }

        public static T Max<T>(this Span<T> self) where T : unmanaged
        {
            if (self.Length <= 0)
                throw ThrowErrors.NoElements();

            Comparer<T> comparer = Comparer<T>.Default;
            T num = self[0];

            for (int i = 1; i < self.Length; i++)
            {
                if (comparer.Compare(self[i], num) > 0)
                    num = self[i];
            }

            return num;
        }

        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        public static int IndexOfMin<TSource, TKey>(this Span<TSource> self, Func<TSource, TKey> keySelector) where TSource : unmanaged
        {
            return self.IndexOfMin(keySelector, out _);
        }

        /// <summary>
        /// Returns an index of an element with the minimum parameter value.
        /// </summary>
        public static int IndexOfMin<TSource, TKey>(this Span<TSource> self, Func<TSource, TKey> keySelector, out TKey minKey) where TSource : unmanaged
        {
            if (keySelector == null)
                throw ThrowErrors.NullParameter(nameof(keySelector));

            if (self.Length <= 0)
                throw ThrowErrors.NoElements();

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = 0;
            minKey = keySelector(self[index]);

            for (int i = 1; i < self.Length; i++)
            {
                TSource item = self[i];
                TKey key = keySelector(item);

                if (comparer.Compare(key, minKey) < 0)
                {
                    minKey = key;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        public static int IndexOfMax<TSource, TKey>(this Span<TSource> self, Func<TSource, TKey> keySelector) where TSource : unmanaged
        {
            return self.IndexOfMax(keySelector, out _);
        }

        /// <summary>
        /// Returns an index of an element with the maximum parameter value.
        /// </summary>
        public static int IndexOfMax<TSource, TKey>(this Span<TSource> self, Func<TSource, TKey> keySelector, out TKey maxKey) where TSource : unmanaged
        {
            if (keySelector == null)
                throw ThrowErrors.NullParameter(nameof(keySelector));

            if (self.Length <= 0)
                throw ThrowErrors.NoElements();

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = 0;
            maxKey = keySelector(self[index]);

            for (int i = 1; i < self.Length; i++)
            {
                TSource item = self[i];
                TKey key = keySelector(item);

                if (comparer.Compare(key, maxKey) > 0)
                {
                    maxKey = key;
                    index = i;
                }
            }

            return index;
        }

        public static void Reverse<T>(this Span<T> self, int startIndex, int length) where T : unmanaged
        {
            int backIndex = startIndex + length - 1;
            length /= 2;

            for (int i = 0; i < length; i++)
            {
                self.Swap(startIndex + i, backIndex - i);
            }
        }

        public static void Swap<T>(this Span<T> self, int i, int j) where T : unmanaged
        {
            (self[i], self[j]) = (self[j], self[i]);
        }

        public static void Fill<T>(this Span<T> self, T value, int startIndex, int count) where T : unmanaged
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

        public static void Fill<T>(this Span<T> self, Func<int, T> factory) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                self[i] = factory(i);
            }
        }

        public static void Fill<T>(this Span<T> self, Func<int, T> factory, int startIndex, int count) where T : unmanaged
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

        public static T GetRandomItem<T>(this Span<T> self, IRng generator) where T : unmanaged
        {
            if (self.Length == 0)
                throw ThrowErrors.NoElements();

            return self[generator.Next(self.Length)];
        }

        public static T GetRandomItem<T>(this Span<T> self) where T : unmanaged
        {
            return GetRandomItem(self, RandomNumberGenerator.Default);
        }

#if UNITY_2021_2_OR_NEWER
        public static int Sum(this ReadOnlySpan<int> self)
        {
            int sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        public static float Sum(this ReadOnlySpan<float> self)
        {
            float sum = 0;

            for (int i = 0; i < self.Length; i++)
            {
                sum += self[i];
            }

            return sum;
        }

        public static void CopyTo<T>(this ReadOnlySpan<T> self, T[] destination) where T : unmanaged
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
        }

        public static List<T> ToList<T>(this ReadOnlySpan<T> self) where T : unmanaged
        {
            List<T> dest = new List<T>(self.Length * 2);

            for (int i = 0; i < self.Length; i++)
            {
                dest.Add(self[i]);
            }

            return dest;
        }

        public static int IndexOf<T>(this ReadOnlySpan<T> self, Predicate<T> condition) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        public static T Find<T>(this ReadOnlySpan<T> self, Predicate<T> match) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (match(self[i]))
                    return self[i];
            }

            return default;
        }

        public static void ForEach<T>(this ReadOnlySpan<T> self, Action<T> action) where T : unmanaged
        {
            for (int i = 0; i < self.Length; i++)
                action(self[i]);
        }

        public static bool Contains<T>(this ReadOnlySpan<T> self, Predicate<T> condition) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (condition(self[i]))
                    return true;
            }

            return false;
        }

        public static T Min<T>(this ReadOnlySpan<T> self) where T : unmanaged, IComparable<T>
        {
            if (self.Length <= 0)
                throw ThrowErrors.NoElements();

            T num = self[0];

            for (int i = 1; i < self.Length; i++)
            {
                if (self[i].CompareTo(num) < 0)
                    num = self[i];
            }

            return num;
        }

        public static T Max<T>(this ReadOnlySpan<T> self) where T : unmanaged, IComparable<T>
        {
            if (self.Length <= 0)
                throw ThrowErrors.NoElements();

            T num = self[0];

            for (int i = 1; i < self.Length; i++)
            {
                if (self[i].CompareTo(num) > 0)
                    num = self[i];
            }

            return num;
        }

        public static T GetRandomItem<T>(this ReadOnlySpan<T> self, IRng generator) where T : unmanaged
        {
            if (self.Length == 0)
                throw ThrowErrors.NoElements();

            return self[generator.Next(self.Length)];
        }

        public static T GetRandomItem<T>(this ReadOnlySpan<T> self) where T : unmanaged
        {
            return GetRandomItem(self, RandomNumberGenerator.Default);
        }
#endif
    }
}
