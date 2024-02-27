using System;
using UnityEngine.Serialization;
using OlegHcp.Tools;

namespace OlegHcp.NumericEntities
{
    [Serializable]
    public struct DiapasonInt
    {
        [FormerlySerializedAs("x"), FormerlySerializedAs("From")]
        public int Min;
        [FormerlySerializedAs("y"), FormerlySerializedAs("Before")]
        public int Max;

#if UNITY_EDITOR
        internal static string MinFieldName = nameof(Min);
        internal static string MaxFieldName = nameof(Max);
#endif

        public DiapasonInt(int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), $"{nameof(min)} value cannot be greater than {nameof(max)} value.");

            Min = min;
            Max = max;
        }

        public void Deconstruct(out int from, out int before)
        {
            from = Min;
            before = Max;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(Min.GetHashCode(), Max.GetHashCode());
        }

        public static implicit operator (int min, int max)(DiapasonInt value)
        {
            return (value.Min, value.Max);
        }

        public static implicit operator DiapasonInt((int, int) value)
        {
            return new DiapasonInt(value.Item1, value.Item2);
        }

        public static explicit operator DiapasonInt(Diapason value)
        {
            return new DiapasonInt((int)value.Min, (int)value.Max);
        }
    }
}
