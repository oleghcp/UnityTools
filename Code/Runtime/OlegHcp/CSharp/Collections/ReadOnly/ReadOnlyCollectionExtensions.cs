using System;
using System.Collections.Generic;
using OlegHcp.Tools;

namespace OlegHcp.CSharp.Collections.ReadOnly
{
    public static class ReadOnlyCollectionExtensions
    {
        public static T[] GetSubArray_<T>(this IReadOnlyList<T> self, int startIndex, int length)
        {
            if ((uint)startIndex >= (uint)self.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (length == 0)
                return Array.Empty<T>();

            T[] subArray = new T[length];
            for (int i = 0; i < length; i++)
            {
                subArray[i] = self[i + startIndex];
            }
            return subArray;
        }

        public static T[] GetSubArray_<T>(this IReadOnlyList<T> self, int startIndex)
        {
            return GetSubArray_(self, startIndex, self.Count - startIndex);
        }

        public static T Find_<T>(this IReadOnlyList<T> self, Predicate<T> match)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (match(self[i]))
                    return self[i];
            }

            return default;
        }

        public static T FindLast_<T>(this IReadOnlyList<T> self, Predicate<T> match)
        {
            for (int i = self.Count - 1; i >= 0; i--)
            {
                if (match(self[i]))
                    return self[i];
            }

            return default;
        }

        public static int IndexOf_<T>(this IReadOnlyList<T> self, T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < self.Count; i++)
            {
                if (comparer.Equals(self[i], item))
                    return i;
            }

            return -1;
        }

        public static int IndexOf_<T>(this IReadOnlyList<T> self, Predicate<T> condition)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        public static int LatIndexOf_<T>(this IReadOnlyList<T> self, Predicate<T> condition)
        {
            for (int i = self.Count - 1; i >= 0; i--)
            {
                if (condition(self[i]))
                    return i;
            }

            return -1;
        }

        public static int IndexOfMin_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> keySelector)
        {
            return self.IndexOfMin_(keySelector, out _);
        }

        public static int IndexOfMin_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> keySelector, out TKey minKey)
        {
            if (keySelector == null)
                throw ThrowErrors.NullParameter(nameof(keySelector));

            if (self.Count <= 0)
                throw ThrowErrors.NoElements();

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = 0;
            minKey = keySelector(self[index]);

            for (int i = 1; i < self.Count; i++)
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

        public static int IndexOfMax_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> keySelector)
        {
            return self.IndexOfMax_(keySelector, out _);
        }

        public static int IndexOfMax_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> keySelector, out TKey maxKey)
        {
            if (keySelector == null)
                throw ThrowErrors.NullParameter(nameof(keySelector));

            if (self.Count <= 0)
                throw ThrowErrors.NoElements();

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            int index = 0;
            maxKey = keySelector(self[index]);

            for (int i = 1; i < self.Count; i++)
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

        public static T GetRandomItem_<T>(this IReadOnlyList<T> self, IRng generator)
        {
            if (self.Count == 0)
                throw ThrowErrors.NoElements();

            return self[generator.Next(self.Count)];
        }

        public static T GetRandomItem_<T>(this IReadOnlyList<T> self)
        {
            return GetRandomItem_(self, RandomNumberGenerator.Default);
        }

        public static T GetRandomItem_<T>(this IReadOnlyCollection<T> self, IRng generator)
        {
            int index = generator.Next(self.Count);
            int count = 0;
            foreach (T item in self)
            {
                if (index == count)
                    return item;

                count++;
            }

            throw ThrowErrors.NoElements();
        }

        public static T GetRandomItem_<T>(this IReadOnlyCollection<T> self)
        {
            return GetRandomItem_(self, RandomNumberGenerator.Default);
        }

        public static bool IsNullOrEmpty_<T>(this IReadOnlyCollection<T> self)
        {
            return self == null || self.Count == 0;
        }

        public static bool HasAnyData_<T>(this IReadOnlyCollection<T> self)
        {
            return self != null && self.Count > 0;
        }

        public static T FromEnd_<T>(this IReadOnlyList<T> self, int index)
        {
            return self[self.Count - (index + 1)];
        }

        public static ReadOnlySegment<T> Slice_<T>(this IReadOnlyList<T> self, int startIndex, int length)
        {
            return new ReadOnlySegment<T>(self, startIndex, length);
        }

        public static ReadOnlySegment<T> Slice_<T>(this IReadOnlyList<T> self, int startIndex)
        {
            return new ReadOnlySegment<T>(self, startIndex);
        }

        public static void CopyTo_<T>(this IReadOnlyList<T> self, Span<T> target) where T : unmanaged
        {
            int count = Math.Min(self.Count, target.Length);
            for (int i = 0; i < count; i++)
            {
                target[i] = self[i];
            }
        }

        public static void CopyTo_<T>(this IReadOnlyList<T> self, Span<T> target, int index) where T : unmanaged
        {
            if ((uint)index >= (uint)target.Length)
                throw ThrowErrors.IndexOutOfRange();

            int count = Math.Min(self.Count, target.Length - index);
            for (int i = 0; i < count; i++)
            {
                target[i + index] = self[i];
            }
        }

        public static void ForEach_<T>(this IReadOnlyList<T> self, Action<T> action)
        {
            for (int i = 0; i < self.Count; i++)
                action(self[i]);
        }

        public static bool Contains_<T>(this IReadOnlyList<T> self, T item)
        {
            return IndexOf_(self, item) >= 0;
        }

        public static bool Contains_<T>(this IReadOnlyList<T> self, Predicate<T> condition)
        {
            return IndexOf_(self, condition) >= 0;
        }

        public static void ForEach_<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self, Action<KeyValuePair<TKey, TValue>> action)
        {
            foreach (var item in self)
                action(item);
        }

        public static void ForEach_<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self, Action<TKey, TValue> action)
        {
            foreach (var item in self)
                action(item.Key, item.Value);
        }
    }
}
