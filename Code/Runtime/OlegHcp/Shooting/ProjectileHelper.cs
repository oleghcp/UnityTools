#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System;
using OlegHcp.CSharp;
using OlegHcp.Tools;

namespace OlegHcp.Shooting
{
    internal static class ProjectileHelper
    {
        public static void SetRicochetOptionsCount(ref HitOptions[] array, int count)
        {
            if (count == 0)
                array = Array.Empty<HitOptions>();
            else
                Array.Resize(ref array, count);
        }

        public static void AddRicochetOptions(ref HitOptions[] array, in HitOptions options)
        {
            Array.Resize(ref array, array.Length + 1);
            array.FromEnd(0) = options;
        }

        public static void RemoveRicochetOptionsAt(ref HitOptions[] array, int index)
        {
            if ((uint)index >= (uint)array.Length)
                throw ThrowErrors.IndexOutOfRange();

            if (index < array.Length - 1)
                array[index] = array.FromEnd(0);

            SetRicochetOptionsCount(ref array, array.Length - 1);
        }
    }
}
#endif
