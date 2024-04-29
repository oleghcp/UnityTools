#if !UNITY_2021_2_OR_NEWER
namespace System
{
    public struct HashCode
    {
        public static int Combine<T1, T2>(T1 value0, T2 value1)
        {
            return value0.GetHashCode() ^ (value1.GetHashCode() << 2);
        }

        public static int Combine<T1, T2, T3>(T1 value0, T2 value1, T3 value2)
        {
            return value0.GetHashCode() ^ (value1.GetHashCode() << 2) ^ (value2.GetHashCode() >> 2);
        }

        public static int Combine<T1, T2, T3, T4>(T1 value0, T2 value1, T3 value2, T4 value3)
        {
            return value0.GetHashCode() ^ (value1.GetHashCode() << 2) ^ (value2.GetHashCode() >> 2) ^ (value3.GetHashCode() >> 1);
        }
    }
}
#endif
