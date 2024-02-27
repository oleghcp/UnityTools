﻿using System;
using System.Collections.Generic;

namespace OlegHcp.CSharp.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector)
        {
            CollectionUtility.Min(self, selector, out TSource res, out _);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector)
        {
            CollectionUtility.Max(self, selector, out TSource res, out _);
            return res;
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector, out TKey min)
        {
            CollectionUtility.Min(self, selector, out TSource res, out min);
            return res;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> selector, out TKey max)
        {
            CollectionUtility.Max(self, selector, out TSource res, out max);
            return res;
        }

        /// <summary>
        /// Indicates whether the specified collection is null or it's length equals zero.
        /// </summary>    
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> self)
        {
            if (self == null)
                return true;

            if (self is ICollection<T> collection)
                return collection.Count == 0;

            if (self is IReadOnlyCollection<T> readOnlyColl)
                return readOnlyColl.Count == 0;

            return !self.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Indicates whether the specified collection is not null and contains at least one element.
        /// </summary>
        public static bool HasAnyData<T>(this IEnumerable<T> self)
        {
            return !self.IsNullOrEmpty();
        }

#if !UNITY_2021_2_OR_NEWER
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
        {
            return new HashSet<T>(self);
        }
#endif

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable`1.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
                action(item);
        }

        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> self, T newElement)
        {
            foreach (T item in self)
            {
                yield return item;
            }

            yield return newElement;
        }

        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> self, T newElement1, T newElement2)
        {
            foreach (T item in self)
            {
                yield return item;
            }

            yield return newElement1;
            yield return newElement2;
        }

        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> self, T newElement1, T newElement2, T newElement3)
        {
            foreach (T item in self)
            {
                yield return item;
            }

            yield return newElement1;
            yield return newElement2;
            yield return newElement3;
        }

        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> self, params T[] newElements)
        {
            foreach (T item in self)
            {
                yield return item;
            }

            for (int i = 0; i < newElements.Length; i++)
            {
                yield return newElements[i];
            }
        }

        public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> self, T newElement)
        {
            yield return newElement;

            foreach (T item in self)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> self, T newElement1, T newElement2)
        {
            yield return newElement1;
            yield return newElement2;

            foreach (T item in self)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> self, T newElement1, T newElement2, T newElement3)
        {
            yield return newElement1;
            yield return newElement2;
            yield return newElement3;

            foreach (T item in self)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> self, params T[] newElements)
        {
            for (int i = 0; i < newElements.Length; i++)
            {
                yield return newElements[i];
            }

            foreach (T item in self)
            {
                yield return item;
            }
        }
    }
}
