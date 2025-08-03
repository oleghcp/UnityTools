using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections.ReadOnly;
using OlegHcp.Tools;

namespace OlegHcp.CSharp.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector)
        {
            return self.GetWithMin(keySelector, out _);
        }

        /// <summary>
        /// Returns an element with the minimum parameter value.
        /// </summary>
        public static TSource GetWithMin<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector, out TKey minKey)
        {
            if (keySelector == null)
                throw ThrowErrors.NullParameter(nameof(keySelector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            TSource result = default;
            minKey = default;
            bool nonFirstIteration = false;

            foreach (var item in self)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (comparer.Compare(key, minKey) < 0)
                    {
                        minKey = key;
                        result = item;
                    }
                }
                else
                {
                    minKey = keySelector(item);
                    result = item;
                    nonFirstIteration = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector)
        {
            return self.GetWithMax(keySelector, out _);
        }

        /// <summary>
        /// Returns an element with the maximum parameter value.
        /// </summary>
        public static TSource GetWithMax<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector, out TKey maxKey)
        {
            if (keySelector == null)
                throw ThrowErrors.NullParameter(nameof(keySelector));

            Comparer<TKey> comparer = Comparer<TKey>.Default;

            maxKey = default;
            TSource result = default;
            bool nonFirstIteration = false;

            foreach (var item in self)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (comparer.Compare(key, maxKey) > 0)
                    {
                        maxKey = key;
                        result = item;
                    }
                }
                else
                {
                    maxKey = keySelector(item);
                    result = item;
                    nonFirstIteration = true;
                }
            }

            return result;
        }

        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> self, T newElement)
        {
            if (self == null)
                throw new NullReferenceException();

            foreach (T item in self)
            {
                yield return item;
            }

            yield return newElement;
        }

        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> self, T newElement1, T newElement2)
        {
            if (self == null)
                throw new NullReferenceException();

            foreach (T item in self)
            {
                yield return item;
            }

            yield return newElement1;
            yield return newElement2;
        }

        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> self, T newElement1, T newElement2, T newElement3)
        {
            if (self == null)
                throw new NullReferenceException();

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
            if (self == null)
                throw new NullReferenceException();

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
            if (self == null)
                throw new NullReferenceException();

            yield return newElement;

            foreach (T item in self)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> self, T newElement1, T newElement2)
        {
            if (self == null)
                throw new NullReferenceException();

            yield return newElement1;
            yield return newElement2;

            foreach (T item in self)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> self, T newElement1, T newElement2, T newElement3)
        {
            if (self == null)
                throw new NullReferenceException();

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
            if (self == null)
                throw new NullReferenceException();

            for (int i = 0; i < newElements.Length; i++)
            {
                yield return newElements[i];
            }

            foreach (T item in self)
            {
                yield return item;
            }
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

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable`1.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (action == null)
                throw ThrowErrors.NullParameter(nameof(action));

            if (self is IList<T> list)
            {
                list.ForEach(action);
                return;
            }

            if (self is IReadOnlyList<T> roList)
            {
                roList.ForEach_(action);
                return;
            }

            foreach (var item in self)
            {
                action(item);
            }
        }

        public static void AddTo<T>(this IEnumerable<T> self, List<T> targetCollection)
        {
            targetCollection.AddRange(self);
        }

        public static void AddTo<T>(this IEnumerable<T> self, ICollection<T> targetCollection)
        {
            foreach (T item in self)
            {
                targetCollection.Add(item);
            }
        }

        public static void AddTo<T>(this IEnumerable<T> self, ISet<T> targetCollection)
        {
            targetCollection.UnionWith(self);
        }

        public static void AddTo<T>(this IEnumerable<T> self, Queue<T> targetCollection)
        {
            foreach (T item in self)
            {
                targetCollection.Enqueue(item);
            }
        }

        public static void AddTo<T>(this IEnumerable<T> self, Stack<T> targetCollection)
        {
            foreach (T item in self)
            {
                targetCollection.Push(item);
            }
        }

        public static void AddTo<TKey, TValue>(this IEnumerable<TValue> self, IDictionary<TKey, TValue> targetCollection, Func<TValue, TKey> keySelector)
        {
            foreach (TValue item in self)
            {
                targetCollection.Add(keySelector(item), item);
            }
        }

#if !UNITY_2021_2_OR_NEWER
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
        {
            return new HashSet<T>(self);
        }
#endif
    }
}
