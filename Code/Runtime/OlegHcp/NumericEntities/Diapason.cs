using System;
using OlegHcp.Tools;

namespace OlegHcp.NumericEntities
{
#if UNITY
    [Serializable]
#endif
    public struct Diapason
    {
#if UNITY
        [UnityEngine.Serialization.FormerlySerializedAs("x")]
#endif
        public float Min;
#if UNITY
        [UnityEngine.Serialization.FormerlySerializedAs("y")]
#endif
        public float Max;

#if UNITY_EDITOR
        internal static string MinFieldName = nameof(Min);
        internal static string MaxFieldName = nameof(Max);
#endif

        public Diapason(float min, float max)
        {
            if (min > max)
                throw ThrowErrors.MinMax(nameof(min), nameof(max));

            Min = min;
            Max = max;
        }

        public void Deconstruct(out float min, out float max)
        {
            min = Min;
            max = Max;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public static implicit operator (float min, float max)(Diapason value)
        {
            return (value.Min, value.Max);
        }

        public static implicit operator Diapason((float, float) value)
        {
            return new Diapason(value.Item1, value.Item2);
        }

        public static explicit operator Diapason(DiapasonInt value)
        {
            return new Diapason(value.Min, value.Max);
        }
    }
}
