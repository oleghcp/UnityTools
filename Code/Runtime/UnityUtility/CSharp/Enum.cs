using System;

namespace UnityUtility.CSharp
{
    public static class Enum<TEnum> where TEnum : Enum
    {
        public static readonly int Count;

        static Enum()
        {
            Count = Enum.GetNames(typeof(TEnum)).Length;
        }
    }
}
