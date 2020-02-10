namespace System.Collections.Generic
{
    internal static class CollectionHelper
    {
        public static int ValueMin<TSource, TKey>(IList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : IComparable<TKey>
        {
            int index = 0;
            result = collection[index];
            TKey min = keySelector(result);

            int count = collection.Count;

            for (int i = 1; i < count; i++)
            {
                TKey key = keySelector(collection[i]);

                if (key.CompareTo(min) < 0)
                {
                    min = key;
                    result = collection[i];
                    index = i;
                }
            }

            return index;
        }

        public static int ValueMax<TSource, TKey>(IList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : IComparable<TKey>
        {
            int index = 0;
            result = collection[index];
            TKey max = keySelector(result);

            int count = collection.Count;

            for (int i = 1; i < count; i++)
            {
                TKey key = keySelector(collection[i]);

                if (key.CompareTo(max) > 0)
                {
                    max = key;
                    result = collection[i];
                    index = i;
                }
            }

            return index;
        }

        public static int RefMin<TSource, TKey>(IList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : IComparable<TKey>
        {
            int index = 0;
            result = collection[index];
            TKey min = keySelector(result);

            int count = collection.Count;

            for (int i = 1; i < count; i++)
            {
                TKey key = keySelector(collection[i]);

                if (Compare(key, min) < 0)
                {
                    min = key;
                    result = collection[i];
                    index = i;
                }
            }

            return index;
        }

        public static int RefMax<TSource, TKey>(IList<TSource> collection, Func<TSource, TKey> keySelector, out TSource result) where TKey : IComparable<TKey>
        {
            int index = 0;
            result = collection[index];
            TKey max = keySelector(result);

            int count = collection.Count;

            for (int i = 1; i < count; i++)
            {
                TKey key = keySelector(collection[i]);

                if (Compare(key, max) > 0)
                {
                    max = key;
                    result = collection[i];
                    index = i;
                }
            }

            return index;
        }

        public static int Compare<T>(T a, T b) where T : IComparable<T>
        {
            if (a != null)
                return a.CompareTo(b);
            if (b != null)
                return -b.CompareTo(a);
            return 0;
        }
    }
}
