using System;
using UnityUtility.CSharp;
using UnityUtility.Tools;

namespace UnityUtility.Shooting
{
    internal static class ProjectileHelper
    {
        public static void SetRicochetOptionsCount(ref RicochetOptions[] array, int count)
        {
            if (count == 0)
                array = Array.Empty<RicochetOptions>();
            else
                Array.Resize(ref array, count);
        }

        public static void AddRicochetOptions(ref RicochetOptions[] array, in RicochetOptions options)
        {
            Array.Resize(ref array, array.Length + 1);
            array.FromEnd(0) = options;
        }

        public static void RemoveRicochetOptionsAt(ref RicochetOptions[] array, int index)
        {
            if ((uint)index >= (uint)array.Length)
                throw Errors.IndexOutOfRange();

            if (index < array.Length - 1)
                array[index] = array.FromEnd(0);

            SetRicochetOptionsCount(ref array, array.Length - 1);
        }
    }
}
