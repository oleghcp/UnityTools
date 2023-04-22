using UnityEngine;

namespace UnityUtility.Engine
{
    public static class ColorExtensions
    {
        public static Color AlterR(this Color value, float r)
        {
            value.r = r;
            return value;
        }

        public static Color32 AlterR(this Color32 value, byte r)
        {
            value.r = r;
            return value;
        }

        public static Color AlterG(this Color value, float g)
        {
            value.g = g;
            return value;
        }

        public static Color32 AlterG(this Color32 value, byte g)
        {
            value.g = g;
            return value;
        }

        public static Color AlterB(this Color value, float b)
        {
            value.b = b;
            return value;
        }

        public static Color32 AlterB(this Color32 value, byte b)
        {
            value.b = b;
            return value;
        }

        public static Color AlterA(this Color value, float a)
        {
            value.a = a;
            return value;
        }

        public static Color32 AlterA(this Color32 value, byte a)
        {
            value.a = a;
            return value;
        }

        public static void Deconstruct(this in Color value, out float r, out float g, out float b, out float a)
        {
            r = value.r;
            g = value.g;
            b = value.b;
            a = value.a;
        }

        public static void Deconstruct(this in Color32 value, out byte r, out byte g, out byte b, out byte a)
        {
            r = value.r;
            g = value.g;
            b = value.b;
            a = value.a;
        }
    }
}
