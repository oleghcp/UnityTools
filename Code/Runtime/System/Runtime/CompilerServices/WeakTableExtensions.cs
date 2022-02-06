namespace System.Runtime.CompilerServices
{
    public static class WeakTableExtensions
    {
        public static TValue Place<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> self, TKey key, TValue value) where TKey : class where TValue : class
        {
            self.Add(key, value);
            return value;
        }

        public static TValue GetOrCreate<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> self, TKey key) where TKey : class where TValue : class, new()
        {
            if (self.TryGetValue(key, out TValue value))
                return value;

            return self.Place(key, new TValue());
        }
    }
}
