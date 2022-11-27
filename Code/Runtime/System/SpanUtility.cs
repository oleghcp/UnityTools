using UnityUtility;
using UnityUtility.Tools;

namespace System
{
    internal static class SpanUtility
    {
        public static void Shuffle<T>(Span<T> span, IRng generator) where T : unmanaged
        {
            int last = span.Length;

            while (last > 1)
            {
                int cur = generator.Next(last--);
                Helper.Swap(ref span[cur], ref span[last]);
            }
        }

        public static void Shuffle<T>(Span<T> span) where T : unmanaged
        {
            int last = span.Length;

            while (last > 1)
            {
                int cur = UnityEngine.Random.Range(0, last--);
                Helper.Swap(ref span[cur], ref span[last]);
            }
        }

        public static T Min<T>(Span<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw Errors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) < 0)
                    num = span[i];
            }

            return num;
        }

        public static T Max<T>(Span<T> span) where T : unmanaged, IComparable<T>
        {
            if (span.Length <= 0)
                throw Errors.NoElements();

            T num = span[0];

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].CompareTo(num) > 0)
                    num = span[i];
            }

            return num;
        }
    }
}
