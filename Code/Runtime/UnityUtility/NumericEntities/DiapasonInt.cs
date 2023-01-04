using System;
using UnityEngine.Serialization;
using UnityUtility.Tools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct DiapasonInt
    {
        [FormerlySerializedAs("x")]
        public int From;
        [FormerlySerializedAs("y")]
        public int Before;

#if UNITY_EDITOR
        public static string FromFieldName = nameof(From);
        public static string BeforeFieldName = nameof(Before);
#endif

        public DiapasonInt(int from, int before)
        {
            if (from > before)
                throw new ArgumentOutOfRangeException(nameof(from), $"{nameof(from)} value cannot be greater than or equals to {nameof(before)} value.");

            From = from;
            Before = before;
        }

        public void Deconstruct(out int from, out int before)
        {
            from = From;
            before = Before;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(From.GetHashCode(), Before.GetHashCode());
        }

        public static implicit operator (int from, int before)(DiapasonInt value)
        {
            return (value.From, value.Before);
        }

        public static implicit operator DiapasonInt((int from, int before) value)
        {
            return new DiapasonInt(value.from, value.before);
        }
    }
}
