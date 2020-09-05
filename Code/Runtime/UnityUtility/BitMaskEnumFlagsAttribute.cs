using System;
using UnityEngine;

namespace UnityUtility
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
