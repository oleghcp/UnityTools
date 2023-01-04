using System;
using UnityEngine.Serialization;
using UnityUtility.Tools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct Diapason
    {
        [FormerlySerializedAs("x")]
        public float Min;
        [FormerlySerializedAs("y")]
        public float Max;

#if UNITY_EDITOR
        public static string MinFieldName = nameof(Min);
        public static string MaxFieldName = nameof(Max);
#endif

        public Diapason(float min, float max)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

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
            return Helper.GetHashCode(Min.GetHashCode(), Max.GetHashCode());
        }

        public static implicit operator (float min, float max)(Diapason value)
        {
            return (value.Min, value.Max);
        }

        public static implicit operator Diapason((float min, float max) value)
        {
            return new Diapason(value.min, value.max);
        }
    }
}
