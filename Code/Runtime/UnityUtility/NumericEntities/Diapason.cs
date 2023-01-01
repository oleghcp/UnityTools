using System;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct Diapason
    {
        public float Min;
        public float Max;

#if UNITY_EDITOR
        public static string MinFieldName = nameof(Min);
        public static string MaxFieldName = nameof(Max);
#endif

        public void Deconstruct(out float min, out float max)
        {
            min = Min;
            max = Max;
        }
    }

    [Serializable]
    public struct DiapasonInt
    {
        public int From;
        public int Before;

#if UNITY_EDITOR
        public static string FromFieldName = nameof(From);
        public static string BeforeFieldName = nameof(Before);
#endif

        public void Deconstruct(out int from, out int before)
        {
            from = From;
            before = Before;
        }
    }
}
