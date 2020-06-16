using Tools;
using UnityUtility;

namespace System.Collections.Generic
{
    internal static class CollectionHelper
    {
        public static void Shuffle<T>(IList<T> collection, IRng generator)
        {
            int last = collection.Count;

            while (last > 1)
            {
                int cur = generator.Next(last--);

                T value = collection[cur];
                collection[cur] = collection[last];
                collection[last] = value;
            }
        }
    }

    internal static class CollectionHelperForValueType
    {
        public static int Min<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : struct, IComparable<TKey>
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            int index = -1;
            TKey minKey = default;
            result = default;

            bool nonFirstIteration = false;

            int i = 0;
            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (key.CompareTo(minKey) < 0)
                    {
                        minKey = key;
                        result = item;
                        index = i;
                    }
                }
                else
                {
                    minKey = keySelector(item);
                    nonFirstIteration = true;
                }

                ++i;
            }

            if (nonFirstIteration)
                return index;

            throw Errors.NoElements();
        }

        public static int Max<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : struct, IComparable<TKey>
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            int index = -1;
            TKey maxKey = default;
            result = default;

            bool nonFirstIteration = false;

            int i = 0;
            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (key.CompareTo(maxKey) > 0)
                    {
                        maxKey = key;
                        result = item;
                        index = i;
                    }
                }
                else
                {
                    maxKey = keySelector(item);
                    nonFirstIteration = true;
                }

                ++i;
            }

            if (nonFirstIteration)
                return index;

            throw Errors.NoElements();
        }
    }

    internal static class CollectionHelperForRefType
    {
        public static int Min<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : class, IComparable<TKey>
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            int index = -1;
            TKey minKey = default;
            result = default;

            bool nonFirstIteration = false;

            int i = 0;
            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (Helper.Compare(key, minKey) < 0)
                    {
                        minKey = key;
                        result = item;
                        index = i;
                    }
                }
                else
                {
                    minKey = keySelector(item);
                    nonFirstIteration = true;
                }

                ++i;
            }

            if (nonFirstIteration)
                return index;

            throw Errors.NoElements();
        }

        public static int Max<TSource, TKey>(IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : class, IComparable<TKey>
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            int index = -1;
            TKey maxKey = default;
            result = default;

            bool nonFirstIteration = false;

            int i = 0;
            foreach (var item in collection)
            {
                if (nonFirstIteration)
                {
                    TKey key = keySelector(item);

                    if (Helper.Compare(key, maxKey) > 0)
                    {
                        maxKey = key;
                        result = item;
                        index = i;
                    }
                }
                else
                {
                    maxKey = keySelector(item);
                    nonFirstIteration = true;
                }

                ++i;
            }

            if (nonFirstIteration)
                return index;

            throw Errors.NoElements();
        }
    }
}
