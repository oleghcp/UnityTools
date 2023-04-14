using UnityEngine;

namespace UnityUtility.Engine
{
    public static class ColorExtensions
    {
        public static Color AlterR(this in Color value, float r)
        {
            return new Color(r, value.g, value.b, value.a);
        }

        public static Color32 AlterR(this in Color32 value, byte r)
        {
            return new Color32(r, value.g, value.b, value.a);
        }

        public static Color AlterG(this in Color value, float g)
        {
            return new Color(value.r, g, value.b, value.a);
        }

        public static Color32 AlterG(this in Color32 value, byte g)
        {
            return new Color32(value.r, g, value.b, value.a);
        }

        public static Color AlterB(this in Color value, float b)
        {
            return new Color(value.r, value.g, b, value.a);
        }

        public static Color32 AlterB(this in Color32 value, byte b)
        {
            return new Color32(value.r, value.g, b, value.a);
        }

        public static Color AlterA(this in Color value, float a)
        {
            return new Color(value.r, value.g, value.b, a);
        }

        public static Color32 AlterA(this in Color32 value, byte a)
        {
            return new Color32(value.r, value.g, value.b, a);
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
