using System;
using UnityEngine;

namespace UU.BitMasks
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BitMaskEnumFlagsAttribute : PropertyAttribute
    {
        internal readonly Type EnumType;

        public BitMaskEnumFlagsAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}
