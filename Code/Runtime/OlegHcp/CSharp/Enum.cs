using System;
using System.Reflection;

namespace OlegHcp.CSharp
{
    public static class Enum<TEnum> where TEnum : struct, Enum
    {
        public static readonly int Count;

        static Enum()
        {
            BindingFlags mask = BindingFlags.Static | BindingFlags.Public;
            Count = typeof(TEnum).GetFields(mask).Length;
        }
    }
}
