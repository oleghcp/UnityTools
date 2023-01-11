﻿using System;
using System.Collections.Generic;
using UnityUtility.CSharp.Collections.Iterators;
using UnityUtility.Tools;

namespace UnityUtility.CSharp.Collections.ReadOnly
{
    public static class ReadOnlyCollectionExtensions
    {
        public static T[] GetSubArray_<T>(this IReadOnlyList<T> self, int startIndex, int length)
        {
            T[] subArray = new T[length];
            for (int i = 0; i < length; i++) { subArray[i] = self[i + startIndex]; }
            return subArray;
        }

        public static T[] GetSubArray_<T>(this IReadOnlyList<T> self, int startIndex)
        {
            return GetSubArray_(self, startIndex, self.Count - startIndex);
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

        public static int IndexOfMin_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Min(self, selector, out _, out _);
        }

        public static int IndexOfMax_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> selector)
        {
            return CollectionUtility.Max(self, selector, out _, out _);
        }

        public static int IndexOfMin_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> selector, out TKey min)
        {
            return CollectionUtility.Min(self, selector, out _, out min);
        }

        public static int IndexOfMax_<TSource, TKey>(this IReadOnlyList<TSource> self, Func<TSource, TKey> selector, out TKey max)
        {
            return CollectionUtility.Max(self, selector, out _, out max);
        }

        public static T GetRandomItem_<T>(this IReadOnlyList<T> self, IRng generator)
        {
            if (self.Count == 0)
                throw Errors.NoElements();

            return self[generator.Next(self.Count)];
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

            throw Errors.NoElements();
        }

        public static bool IsNullOrEmpty_<T>(this IReadOnlyCollection<T> self)
        {
            return self == null || self.Count == 0;
        }

        public static bool HasAnyData_<T>(this IReadOnlyCollection<T> self)
        {
            return self != null && self.Count > 0;
        }

        public static T FromEnd_<T>(this IReadOnlyList<T> self, int reverseIndex)
        {
            return self[self.Count - (reverseIndex + 1)];
        }

        public static EnumerableQuery_<T> Enumerate_<T>(this IReadOnlyList<T> self, int startIndex, int length)
        {
            return new EnumerableQuery_<T>(self, startIndex, length);
        }

        public static EnumerableQuery_<T> Enumerate_<T>(this IReadOnlyList<T> self, int startIndex)
        {
            return new EnumerableQuery_<T>(self, startIndex);
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
                throw Errors.IndexOutOfRange();

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

        public static bool Contains_<T>(this IReadOnlyList<T> self, Predicate<T> condition)
        {
            return self.IndexOf_(condition) >= 0;
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
